using Covid.Schemas;
using CsvHelper;
using CsvHelper.Configuration;
using Mosaik.Library;
using Mosaik.Server.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Covid
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var root = new RootCommand() { Name = "covid", TreatUnmatchedTokensAsErrors = true };

            root.AddFromMethod(typeof(Program).GetMethod(nameof(Program.Papers)), "Ingest the COVID-19 Open Research Dataset Challenge (CORD-19) dataset into a Mosaik server. You can download the original dataset here: https://www.kaggle.com/allen-institute-for-ai/CORD-19-research-challenge/tasks. Extract it to a folder and pass the path with the --folder argument.");
            root.AddFromMethod(typeof(Program).GetMethod(nameof(Program.Diseases)), "Ingest the https://github.com/DiseaseOntology/HumanDiseaseOntology/ dataset. Call with --file \\path\\to\\doid.json");
            return await root.InvokeAsync(args);
        }

        public static async Task Papers(string token, string server, string folder)
        {
            var csvFile   = Path.Combine(folder, @"\2020-03-13\all_sources_metadata_2020-03-13.csv");
            if (!File.Exists(csvFile)) 
            {
                Console.WriteLine("Error --------------");
                Console.WriteLine("Metadata file not found on the folder: " + csvFile);
                Console.WriteLine("Check the --folder argument, it should point to the unzipped root folder of the dataset.");
                return;
            }

            var shaToFile = Directory.EnumerateFiles(Path.Combine(folder, @"\2020-03-13\"), "*.json", new EnumerationOptions() { RecurseSubdirectories = true })
                                     .ToDictionary(f => Path.GetFileNameWithoutExtension(f), f => f);


            using (var graph = await Graph.WithServer(server, token).WithConsoleLogging().ConnectAsync())
            using (var session = graph.OpenSession().AutoCommit(batchSize: 1000))
            using (var reader = new StreamReader(csvFile))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true, PrepareHeaderForMatch = (h,i) => FixHeader(h),  LineBreakInQuotedFieldIsBadData = false }))
            {
                await CreateGraphSchema(graph);

                var records = csv.GetRecords<PaperCsv>();
                foreach (var r in records)
                {
                    if (string.IsNullOrWhiteSpace(r.Doi)) continue;

                    var paper = new Paper()
                    {
                        Sha = r.Sha,
                        Source = r.Source_x,
                        Title = r.Title,
                        Doi = r.Doi,
                        Pmcid = r.Pmcid,
                        PubmedId = r.Pubmed_id,
                        License = r.License,
                        Abstract = r.Abstract,
                        PublishTime = ParseDate(r.Publish_time),
                        Authors = r.Authors,
                        Journal = r.Journal,
                        MicrosoftAcademicPaperID = r.MicrosoftAcademicPaperID,
                        WHOCovidence = r.WHOCovidence,
                    };

                    PaperJson json = null;
                    if (shaToFile.TryGetValue(r.Sha, out var jsonFile))
                    {
                        json = JsonConvert.DeserializeObject<PaperJson>(File.ReadAllText(jsonFile));
                        paper.FullText = string.Join("\n", json.BodyText.Select(bt => $"{bt.Section}\n{bt.Text}"));
                    }

                    var paperUID = session.AddOrUpdate(paper);

                    if (!string.IsNullOrWhiteSpace(r.Journal))
                    {
                        var journalUID = session.AddOrUpdate(new Journal { Name = r.Journal });
                        session.AddUniqueEdge(paperUID, E.PublishedIn, journalUID);
                        session.AddUniqueEdge(journalUID, E.HasPaper, paperUID);
                    }

                    if (json is object)
                    {
                        foreach (var author in json.Metadata.Authors)
                        {
                            if ((author.First.Length + author.Last.Length) < 4) continue;

                            var authorUID = session.AddOrUpdate(new Author()
                            {
                                First = author.First,
                                Middle = author.Middle,
                                Last = author.Last,
                                Email = author.Email,
                                Suffix = author.Suffix,
                            });

                            session.AddUniqueEdge(paperUID, E.HasAuthor, authorUID);
                            session.AddUniqueEdge(authorUID, E.AuthorOf, paperUID);


                            if (author.Affiliation is object && (!string.IsNullOrWhiteSpace(author.Affiliation.Laboratory) || !string.IsNullOrWhiteSpace(author.Affiliation.Institution)))
                            {
                                var affiliationUID = session.AddOrUpdate(new Affiliation()
                                {
                                    Laboratory = author.Affiliation.Laboratory ?? "",
                                    Institution = author.Affiliation.Institution ?? "",
                                });

                                if (author.Affiliation.Location is object && !string.IsNullOrWhiteSpace(author.Affiliation.Location.Settlement) && !string.IsNullOrWhiteSpace(author.Affiliation.Location.Country))
                                {
                                    var locationUID = session.AddOrUpdate(new Location()
                                    {
                                        Settlement = author.Affiliation.Location.Settlement ?? "",
                                        Country = author.Affiliation.Location.Country ?? "",
                                    });
                                    session.AddUniqueEdge(affiliationUID, E.LocatedIn, locationUID);
                                    session.AddUniqueEdge(locationUID, E.HasInstitution, affiliationUID);
                                    session.AddUniqueEdge(authorUID, E.BasedOn, locationUID);
                                    session.AddUniqueEdge(locationUID, E.HasAuthor, authorUID);
                                }

                                session.AddUniqueEdge(authorUID, E.HasAffiliation, affiliationUID);
                                session.AddUniqueEdge(affiliationUID, E.AffiliatedTo, authorUID);
                            }
                        }
                    }
                    else
                    {
                        foreach (var a in r.Authors.StartsWith("[") ? JsonConvert.DeserializeObject<string[]>(r.Authors.Replace(@"\xa0", " ").Replace("None", "null")) : r.Authors.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var author = HttpUtility.HtmlDecode(a).Trim(';');

                            if (author is null || author.Length < 4) continue;
                            var parts = author.Split(new[] { ',' }, 2, StringSplitOptions.RemoveEmptyEntries);
                            string first, last, middle;

                            if (parts.Length == 1)
                            {
                                first = parts[0];
                                last = "";
                                middle = "";
                            }
                            else
                            {
                                first = parts[0];
                                last = parts[1].Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries).First();
                                middle = parts[1].Substring(parts[1].IndexOf(last) + last.Length);
                            }

                            var authorUID = session.AddOrUpdate(new Author()
                            {
                                First = first,
                                Middle = middle.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                                Last = last
                            });

                            session.AddUniqueEdge(paperUID, E.HasAuthor, authorUID);
                            session.AddUniqueEdge(authorUID, E.AuthorOf, paperUID);
                        }
                    }
                }
            }
        }

        public static async Task Diseases(string token, string server, string file)
        {
            using (var graph = await Graph.WithServer(server, token).WithConsoleLogging().ConnectAsync())
            using (var session = graph.OpenSession().AutoCommit(batchSize: 1000))
            {
                await CreateGraphSchema(graph);

                var ontology = JsonConvert.DeserializeObject<DiseasesOntologyJson>(await File.ReadAllTextAsync(file));

                foreach (var node in ontology.Graphs.First().Nodes)
                {
                    if (node.Type != "CLASS") continue;

                    var d = new Disease()
                    {
                        Id = node.Id?.ToString(),
                        Label = node.Lbl,
                        Synonyms = node.Meta?.Synonyms?.Select(s => s.Val)?.Distinct()?.ToArray() ?? new string[0],
                        XRefs = node.Meta?.Xrefs?.Select(x => x.Val)?.Distinct()?.ToArray() ?? new string[0]
                    };

                    if (string.IsNullOrWhiteSpace(d.Label) ||
                        string.IsNullOrWhiteSpace(d.Id) ||
                        d.Label.Contains("_") ||
                        d.Label.ToLowerInvariant().Contains("obsolete")) continue;

                    if (d.Label == "disease" || d.Label == "syndrome") continue;

                    var uid = session.AddOrUpdate(d);

                    session.AddAlias(uid, Mosaik.Core.Language.English, alias: d.Label, ignoreCase: false);

                    foreach(var s in d.Synonyms)
                    {
                        session.AddAlias(uid, Mosaik.Core.Language.English, alias: s, ignoreCase: false);
                    }
                }
                
            }
        }

        private static async Task CreateGraphSchema(IGraph graph)
        {
            await graph.Schema.ExistsAsync(Schema.NewEdge(E.HasAffiliation), SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync(Schema.NewEdge(E.AffiliatedTo), SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync(Schema.NewEdge(E.LocatedIn), SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync(Schema.NewEdge(E.HasInstitution), SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync(Schema.NewEdge(E.BasedOn), SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync(Schema.NewEdge(E.HasAuthor), SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync(Schema.NewEdge(E.AuthorOf), SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync(Schema.NewEdge(E.PublishedIn), SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync(Schema.NewEdge(E.HasPaper), SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync(Schema.NewEdge(E.MentionsDisease), SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync(Schema.NewEdge(E.DiseaseAppearsIn), SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync(Schema.NewEdge(E.MentionsGene), SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync(Schema.NewEdge(E.GeneAppearsIn), SchemaCreationMode.CreateOnly);

            await graph.Schema.ExistsAsync<Paper>(SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync<Author>(SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync<Affiliation>(SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync<Location>(SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync<Journal>(SchemaCreationMode.CreateOnly);
            await graph.Schema.ExistsAsync<Disease>(SchemaCreationMode.CreateOnly);
        }

        private static DateTimeOffset ParseDate(string publish_time)
        {
            if(publish_time.Length == 4)
            {
                return DateTimeOffset.ParseExact(publish_time, "yyyy", CultureInfo.InvariantCulture);
            }
            else if (string.IsNullOrWhiteSpace(publish_time))
            {
                return DateTimeOffset.FromUnixTimeSeconds(0);
            }
            else if (DateTimeOffset.TryParse(publish_time, out var date))
            {
                return date;
            }
            else
            {
                return DateTimeOffset.FromUnixTimeSeconds(0);
            }
        }

        private static string FixHeader(string h)
        {
            h = h.Replace(" ", "").Replace("#", "");
            return h.Substring(0, 1).ToUpper() + h.Substring(1);
        }
    }
}
