using System.Collections.Generic;
using J = Newtonsoft.Json.JsonPropertyAttribute;
using N = Newtonsoft.Json.NullValueHandling;

namespace Covid.Schemas
{
    public class PaperCsv
    {
        public string Sha                      { get; set; }
        public string Source_x                 { get; set; }
        public string Title                    { get; set; }
        public string Doi                      { get; set; }
        public string Pmcid                    { get; set; }
        public string Pubmed_id                { get; set; }
        public string License                  { get; set; }
        public string Abstract                 { get; set; }
        public string Publish_time             { get; set; }
        public string Authors                  { get; set; }
        public string Journal                  { get; set; }
        public string MicrosoftAcademicPaperID { get; set; }
        public string WHOCovidence             { get; set; }
        public string Has_full_text            { get; set; }
    }

    public class PaperJson
    {
        [J("paper_id")] public string PaperId { get; set; }
        [J("metadata")] public Metadata Metadata { get; set; }
        [J("abstract")] public Abstract[] Abstract { get; set; }
        [J("body_text")] public BodyText[] BodyText { get; set; }
        [J("bib_entries")] public Dictionary<string, BibEntry> BibEntries { get; set; }
        [J("ref_entries")] public Dictionary<string, RefEntry> RefEntries { get; set; }
        [J("back_matter")] public BodyText[] BackMatter { get; set; }
    }

    public partial class Abstract
    {
        [J("text")] public string Text { get; set; }
        [J("cite_spans")] public Span[] CiteSpans { get; set; }
        [J("ref_spans")] public Span[] RefSpans { get; set; }
        [J("section")] public string Section { get; set; }
    }

    public class BibEntry
    {
        [J("ref_id")] public string RefId { get; set; }
        [J("title")] public string Title { get; set; }
        [J("authors")] public BibEntryAuthor[] Authors { get; set; }
        [J("year")] public long? Year { get; set; }
        [J("venue")] public string Venue { get; set; }
        [J("volume")] public string Volume { get; set; }
        [J("issn")] public string Issn { get; set; }
        [J("pages")] public string Pages { get; set; }
        [J("other_ids")] public OtherIds OtherIds { get; set; }
    }

    public class BibEntryAuthor
    {
        [J("first")] public string First { get; set; }
        [J("middle")] public string[] Middle { get; set; }
        [J("last")] public string Last { get; set; }
        [J("suffix")] public string Suffix { get; set; }
    }

    public class OtherIds
    {

    }

    public class BodyText
    {
        [J("text")] public string Text { get; set; }
        [J("cite_spans")] public Span[] CiteSpans { get; set; }
        [J("ref_spans")] public Span[] RefSpans { get; set; }
        [J("section")] public string Section { get; set; }
    }

    public class Span
    {
        [J("start")] public long Start { get; set; }
        [J("end")] public long End { get; set; }
        [J("text")] public string Text { get; set; }
        [J("ref_id")] public string RefId { get; set; }
    }

    public class Metadata
    {
        [J("title")] public string Title { get; set; }
        [J("authors")] public MetadataAuthor[] Authors { get; set; }
    }

    public class MetadataAuthor
    {
        [J("first")] public string First { get; set; }
        [J("middle")] public string[] Middle { get; set; }
        [J("last")] public string Last { get; set; }
        [J("suffix")] public string Suffix { get; set; }
        [J("affiliation")] public MetadataAffiliation Affiliation { get; set; }
        [J("email")] public string Email { get; set; }
    }

    public class MetadataAffiliation
    {
        [J("laboratory", NullValueHandling = N.Ignore)] public string Laboratory { get; set; }
        [J("institution", NullValueHandling = N.Ignore)] public string Institution { get; set; }
        [J("location", NullValueHandling = N.Ignore)] public MetadataLocation Location { get; set; }
    }

    public class MetadataLocation
    {
        [J("settlement")] public string Settlement { get; set; }
        [J("country")] public string Country { get; set; }
    }

    public class RefEntry
    {
        [J("text")] public string Text { get; set; }
        [J("latex")] public object Latex { get; set; }
        [J("type")] public string Type { get; set; }
    }
}
