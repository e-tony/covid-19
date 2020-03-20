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
    internal class JournalRenderer : INodeRenderer
    {
        public string NodeType    => "Journal";
        public string DisplayName => "Journal";
        public string LabelField  => "Name"; //You can have a fallback label field in case a field is empty by separating the field names with |
        public string Color       => "#fcba03";
        public string Icon        => "book";

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
            // This method is used to render the full page for this node, i.e. when navigating to /Journal/node_unique_identifier
            // Usually you can re-use the PreviewAsync method, unless you want to have different views being rendered
            return (await PreviewAsync(node)).Merge();
        }

        private IComponent CreateView(Node node)
        {
            // To access properties of the node, you can strongly-typed methods such as node.GetString("stringFieldName") or node.GetInt("intFieldName")
            // For accessing the possible 'Label Fields' of the node, use: Labels.Get(node, LabelField);
            return Stack()
                    .WidthStretch()
                    .Children(
                         Pivot().Height(80.vh())
                            .Pivot("authors",      () => Button("Authors").Link().Primary(),      () => Neighbors(() => API.Query.StartAt(node.UID).Out(Schema.N.Paper, Schema.E.HasPaper).Out(Schema.N.Author, Schema.E.HasAuthor).GetUIDsAsync(), new[] { Schema.N.Author }, true, FacetDisplayOptions.PopupWithButtonIconOnly))
                            .Pivot("publications", () => Button("Publications").Link().Primary(), () => Neighbors(() => API.Query.StartAt(node.UID).Out(Schema.N.Paper, Schema.E.HasPaper).GetUIDsAsync(), new[] { Schema.N.Paper }, true, FacetDisplayOptions.PopupWithButtonIconOnly))
                            );
        }
    }
}