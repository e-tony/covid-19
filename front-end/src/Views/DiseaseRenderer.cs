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
    internal class DiseaseRenderer : INodeRenderer
    {
        public string NodeType    => "Disease";
        public string DisplayName => "Disease";
        public string LabelField  => "Label"; //You can have a fallback label field in case a field is empty by separating the field names with |
        public string Color       => "#e31b5f";
        public string Icon        => "disease";

        public async Task<CardContent> CompactView(Node node)
        {
            // This method is called when rendering search results (and similar uses)
            return CardContent(Header(this, node), null, new Footer(null, NodeCardCommands.For(node)));
        }

        public async Task<CardContent> PreviewAsync(Node node)
        {
            // This method is used to render the modal preview. 
            // You can also do your own preview handling, in this case return null instead.
            return CardContent(Header(this, node), CreateView(node), null);
        }

        public async Task<IComponent> ViewAsync(Node node, Parameters state)
        {
            // This method is used to render the full page for this node, i.e. when navigating to /Author/node_unique_identifier
            // Usually you can re-use the PreviewAsync method, unless you want to have different views being rendered
            return (await PreviewAsync(node)).Merge();
        }

        private IComponent CreateView(Node node)
        {
            // To access properties of the node, you can strongly-typed methods such as node.GetString("stringFieldName") or node.GetInt("intFieldName")
            // For accessing the possible 'Label Fields' of the node, use: Labels.Get(node, LabelField);

            var link= string.IsNullOrEmpty(node.GetString("Id")) ? (IComponent)TextBlock("") : Link(node.GetString("Id"), Button("Open").Link().SetIcon(LineAwesome.ExternalLinkAlt)).OpenInNewTab();

            return Stack()
                    .WidthStretch()
                    .Children(
                         Label("DOID Link").Inline().SetMinLabelWidth(120.px()).SetContent(link),
                         Label("Synonyms").Inline().SetMinLabelWidth(120.px()).SetContent(OverflowSet().Items(node.UnsafeGetAs<ReadOnlyArray<string>>("Synonyms").Select(s => TextBlock(s).PaddingRight(8.px())).ToArray())),
                         Label("XRefs").Inline().SetMinLabelWidth(120.px()).SetContent(OverflowSet().Items(node.UnsafeGetAs<ReadOnlyArray<string>>("XRefs").Select(s => TextBlock(s).PaddingRight(8.px())).ToArray())),
                         Pivot().Height(80.vh())
                            .Pivot("publications", () => Button("Publications").Link().Primary(), () => Neighbors(() => API.Query.StartAt(node.UID).Out(Schema.N.Paper, Schema.E.DiseaseAppearsIn).GetUIDsAsync(), new[] { Schema.N.Paper }, true, FacetDisplayOptions.PopupWithButtonIconOnly))
                            .Pivot("authors",      () => Button("Authors").Link().Primary(),      () => Neighbors(() => API.Query.StartAt(node.UID).Out(Schema.N.Paper, Schema.E.DiseaseAppearsIn).Out(Schema.N.Author, Schema.E.HasAuthor).GetUIDsAsync(), new[] { Schema.N.Author }, true, FacetDisplayOptions.PopupWithButtonIconOnly))
                            );

        }
    }
}