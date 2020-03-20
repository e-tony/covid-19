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
    internal class LocationRenderer : INodeRenderer
    {
        public string NodeType    => "Location";
        public string DisplayName => "Location";
        public string LabelField  => "FullName"; //You can have a fallback label field in case a field is empty by separating the field names with |
        public string Color       => "#0082ad";
        public string Icon        => "map-signs";

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
            // This method is used to render the full page for this node, i.e. when navigating to /Location/node_unique_identifier
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
                            .Pivot("authors",      () => Button("Authors").Link().Primary(),      () => Neighbors(() => API.Query.StartAt(node.UID).Out(Schema.N.Affiliation, Schema.E.HasInstitution).Out(Schema.N.Author, Schema.E.AffiliatedTo).GetUIDsAsync(), new[] { Schema.N.Author }, true, FacetDisplayOptions.PopupWithButtonIconOnly))
                            .Pivot("institutions", () => Button("Institutions").Link().Primary(), () => Neighbors(() => API.Query.StartAt(node.UID).Out(Schema.N.Affiliation, Schema.E.HasInstitution).GetUIDsAsync(), new[] { Schema.N.Affiliation }, true, FacetDisplayOptions.PopupWithButtonIconOnly))
                            );
        }
    }
}