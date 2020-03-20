using System;
using System.Linq;
using System.Threading.Tasks;
using Mosaik;
using Mosaik.Components;
using Mosaik.Schema;
using Mosaik.Views;
using static Tesserae.UI;
using static Mosaik.Components.UI;
using Mosaik.UI.Core;
using Mosaik.Settings;

namespace Covid
{
    public static class CovidApp
    {
        public static void Main()
        {
            var apiURL = "https://covid.curiosity.ai/api";

            // App.Initialize has to have this signature, as it is replaced by the server with the public address environment variable
            App.Initialize("COVID-19 Papers", apiURL, Configure, OnLoad);
        }

        private static void Configure(App.ISettings settings)
        {
            // You can configure the system default settings here
            // Check the ISettings interface for more details of what can be configured

            settings.DefaultLanguage  = Language.English;
            settings.CustomHome = () => new HomeView();

            settings.Login.GuestUserName = "guest";
            settings.Login.GuestPassword = "guestpassword";

            settings.Login.Brand = Div(_("d-inline-block text-center text-primary"), SetStyle(Image(_("pl-4", src: "./assets/img/virus.svg")), s => s.width = s.height = "10rem"), H2(_("font-weight-bold pt-4", text: "COVID-19 Papers")), Div(_("pt-5 text-muted "), Span(_(text:"If you don't have an account yet, you can log in as a guest by clicking the link below.")), Br(_()), Br(_()), Span(_(text:"For your own private account, reach out to us at ")), A(_(text: "hello@curiosity.ai", href:"mailto://hello@curiosity.ai"))));

            settings.Navbar.Brand = Span(_(), SetStyle(Image(_("mr-2", src: "./assets/img/virus.svg")), s => { s.width = "28px"; s.height = "28px"; }), Span(_(text: "COVID-19 Papers")));
            settings.Search.DefaultIgnoreTypes = new[] { "_Token", "_Abbreviation", "_Notification", "_User", "_AccessGroup", "_RestrictionGroup", "_Topic", "_TopicRule", "_TopicAnalysis" };
            settings.Search.SaveViewsOf = new [] { Schema.N.Paper, Schema.N.Author, Schema.N.Affiliation };

            settings.Search.DefaultRelatedFacets = new[] { Schema.N.Author, Schema.N.Affiliation, Schema.N.Location, Schema.N.Journal, "_Topic", Schema.N.Disease };

            settings.Sidebar.Teams             = false;
            settings.Sidebar.MyFiles           = false;
            settings.Sidebar.ShowCuriosityLogo = true;
            settings.Sidebar.UploadFile        = false;
            settings.Sidebar.Analysis          = false;
            settings.Sidebar.Settings          = false;
        }

        private static void OnLoad()
        {
            App.Navbar.SearchBox.Filters.Clear();

            App.Navbar.SearchBox.Filters.AddRange(new[]
            {
                new SearchRequestFilter("Papers",     "search",    "#0049b2", "Search on papers",  (sr) => { sr.SetBeforeTypesFacet(Schema.N.Paper); return true; }),
                new SearchRequestFilter("Diseases",   "disease",  "#0049b2", "Search on diseases",  (sr) => { sr.SetBeforeTypesFacet(Schema.N.Disease); return true; }),
                new SearchRequestFilter("Everything", "search",    "#0049b2", "Search on papers, authors, journals & affiliations",  (sr) => { sr.SetBeforeTypesFacet(Schema.N.Affiliation, Schema.N.Author, Schema.N.Journal, Schema.N.Location, Schema.N.Paper, Schema.N.Disease); return true; }),
                new SearchRequestFilter("Bookmarks",  "bookmark",  "#0049b2", "Search on my bookmarks", (sr) => { BookmarkView.NavigateToSearch(sr); return false; }),
                new SearchRequestFilter("History",    "history",   "#0049b2", "Search on my history",   (sr) => { TimelineView.NavigateToSearch(sr); return false; }),
            });

            App.Navbar.SearchBox.RefreshFilters();

            App.Navbar.SearchBox.RemoveFilter("Files");
            App.Navbar.SearchBox.RemoveFilter("My Files");

            // Any code to run after the system loads should go here
            // You can also register custom routes here as well, using Router.Register(...)
        }
    }
}
