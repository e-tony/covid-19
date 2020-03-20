using System.Linq;
using System.Threading.Tasks;
using Mosaik.Components;
using Mosaik.Schema;
using Mosaik.Views;
using Tesserae;
using Tesserae.Components;
using static Tesserae.UI;
using static Mosaik.Components.UI;
using Mosaik;

namespace Covid
{
    internal class PaperRenderer : INodeRenderer
    {
        public string NodeType    => "Paper";
        public string DisplayName => "Paper";
        public string LabelField  => "Title|Doi"; //You can have a fallback label field in case a field is empty by separating the field names with |
        public string Color       => "#0000f7";
        public string Icon        => "file-alt";

        public async Task<CardContent> CompactView(Node node)
        {
            // This method is called when rendering search results (and similar uses)
            return CardContent(Header(this, node), TextBlock(node.GetString("Abstract")).XSmall(), new Footer(Label(TextBlock("Authors:").XSmall().SemiBold().PaddingRight(8.px())).AlignCenter().Inline().SetContent(NeighborsAsLabels(node.UID, Schema.N.Author, Schema.E.HasAuthor, textSize:TextSize.XSmall)), NodeCardCommands.For(node)));
        }

        public async Task<CardContent> PreviewAsync(Node node)
        {
            // This method is used to render the modal preview. 
            // You can also do your own preview handling, in this case return null instead.
            return CardContent(Header(this, node), CreateView(node), null);
        }

        public async Task<IComponent> ViewAsync(Node node, Parameters state)
        {
            // This method is used to render the full page for this node, i.e. when navigating to /Paper/node_unique_identifier
            // Usually you can re-use the PreviewAsync method, unless you want to have different views being rendered
            return (await PreviewAsync(node)).Merge();
        }

        private IComponent CreateView(Node node)
        {
            // To access properties of the node, you can strongly-typed methods such as node.GetString("stringFieldName") or node.GetInt("intFieldName")
            // For accessing the possible 'Label Fields' of the node, use: Labels.Get(node, LabelField);
            var pmcid    = node.GetString("Pmcid");
            var pubmedid = node.GetString("PubmedId");
            var msid     = node.GetString("MicrosoftAcademicPaperID");

            var pmc    = string.IsNullOrEmpty(pmcid)    ? (IComponent)TextBlock("") : Link($"https://www.ncbi.nlm.nih.gov/pmc/articles/{pmcid}", Button("PubMed Central").Link().SetIcon(LineAwesome.ExternalLinkAlt)).OpenInNewTab();
            var pubmed = string.IsNullOrEmpty(pubmedid) ? (IComponent)TextBlock("") : Link($"https://www.ncbi.nlm.nih.gov/pubmed/{pubmedid}",   Button("PubMed").Link().SetIcon(LineAwesome.ExternalLinkAlt)).OpenInNewTab();
            var msa    = string.IsNullOrEmpty(msid)     ? (IComponent)TextBlock("") : Link($"https://academic.microsoft.com/paper/{msid}",      Button("Microsoft Academic").Link().SetIcon(LineAwesome.ExternalLinkAlt)).OpenInNewTab();
            var empty = TextBlock(string.IsNullOrEmpty(msid) && string.IsNullOrEmpty(pmcid) && string.IsNullOrEmpty(pubmedid) ? "No links" : "");

            var fullText = string.IsNullOrEmpty(node.GetString("FullText")) ? (IComponent)TextBlock("") : Label("Full Text").SetContent(DocumentFromField(node, "FullText", Mosaik.Components.DocumentFromField.Mode.FastDocument));

            return Stack()
                    .WidthStretch()
                    .Children(
                         Pivot().Height(80.vh())
                            .Pivot("overview", () => Button("Overview").Link().Primary(), () => Stack()
                                                                                                    .Stretch()
                                                                                                    .Children(
                                                                                                        Label("Authors: ").Inline().SetMinLabelWidth(120.px()).SetContent(NeighborsAsLabels(node.UID, Schema.N.Author, Schema.E.HasAuthor, showIfNone: () => TextBlock("No authors"))),
                                                                                                        Label("Journal: ").Inline().SetMinLabelWidth(120.px()).SetContent(NeighborsAsLabels(node.UID, Schema.N.Journal, Schema.E.PublishedIn, showIfNone: () => TextBlock("No journal"))),
                                                                                                        Label("License: ").Inline().SetMinLabelWidth(120.px()).SetContent(TextBlock(node.GetString("License") ?? "No license info")),
                                                                                                        Label("External Links: ").Inline().SetMinLabelWidth(120.px()).SetContent(Stack().Horizontal().Children(pmc,pubmed,msa, empty)),
                                                                                                        //3005079553/
                                                                                                        Label("DOI: ").Inline().SetMinLabelWidth(120.px()).SetContent(Link(node.GetString("Doi").Contains("://") ? node.GetString("Doi") : ("https://" + node.GetString("Doi")), Button("Open DOI").Link().SetIcon(LineAwesome.ExternalLinkAlt)).OpenInNewTab()),
                                                                                                        Label("Full Paper: ").Inline().SetMinLabelWidth(120.px()).SetContent(Link("https://sci-hub.tw/" + node.GetString("Doi"), Button("Unlock").Link().SetIcon(LineAwesome.LockOpen)).OpenInNewTab()),
                                                                                                        Label("Abstract").SetContent(DocumentFromField(node, "Abstract", Mosaik.Components.DocumentFromField.Mode.FastDocument)),
                                                                                                        fullText))
                            .Pivot("similar", () => Button("Similar").Link().Primary(), () => SimilarSearchArea(Schema.N.Paper).OnSearch(sr =>
                                    {
                                        sr.SimilarityRanking.SimilarTo = new[] { node.UID };
                                        sr.ExcludeUIDs = new[] { node.UID };
                                    }).Stretch())
                            );
        }
    }
}