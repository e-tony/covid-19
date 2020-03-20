using Mosaik;
using Mosaik.Schema;
using System;
using Tesserae;
using Tesserae.Components;
using static Retyped.dom;
using static Tesserae.UI;

namespace Covid
{
    internal class HomeView : IComponent
    {
        private Stack _MainStack;

        public HomeView()
        {
            var message = @"<span class='tss-fontsize-small'><span>This is a free search system provided by Curiosity for public use. The data you can explore here is from the </span><a href='https://www.kaggle.com/sudalairajkumar/novel-corona-virus-2019-dataset' target='_blank'>COVID-19 Open Research Dataset Challenge (CORD-19)</a><span> Kaggle challenge, but is not related to the original challenge authors.</span><br/><br/><span>It uses our AI-enabled graph and search technology to enable you to explore the dataset using machine-learning-based synonyms and find similar papers using graph embeddings.</span><br/><br/><span>This system is provided free of charge, without any warranty, either expressed or implied. Licenses for the underlying papers are noted in the individual references.</span><span> If you want your own account to explore the data, or more information, get in touch under </span><a href='mailto://hello@curiosity.ai' target='_blank'>hello@curiosity.ai</a><span> or follow us </span><a href='https://twitter.com/curiosity_ai' target='_blank'>on twitter.</a></span>";

            _MainStack = Stack()
                        .Vertical()
                        .Stretch()
                        .Children(
                             Raw(Image(_(src: "./assets/img/virus.svg"))).Width(300.px()).AlignCenter().MarginTop(100.px()),
                             TextBlock("Covid Dataset Search").AlignCenter().Large().SemiBold().MarginTop(20.px()),

                             Raw(Raw(message, true)).Width(640.px()).AlignCenter().MarginTop(20.px()),
                             
                             Stack().Horizontal().AlignCenter().MarginTop(20.px())
                                .Children(
                                     Card(Button("Browse Papers").Link().TextCenter().AlignCenter().WidthStretch().Medium()).OnClick((s, e) => { App.Navbar.SearchBox.SetFilter("Papers"); App.Navbar.SearchBox.TriggerSearch(); })
                                        .Margin(10.px()).Width(200.px()).Height(100.px()),
                                     Card(Button("Browse Diseases").Link().TextCenter().AlignCenter().WidthStretch().Medium()).OnClick((s, e) => { App.Navbar.SearchBox.SetFilter("Diseases"); App.Navbar.SearchBox.TriggerSearch(); })
                                        .Margin(10.px()).Width(200.px()).Height(100.px()),
                                     Card(Button("Explore Topics (beta)").Link().TextCenter().AlignCenter().WidthStretch().Medium()).OnClick((s, e) => Router.Navigate("#/analysis"))
                                        .Margin(10.px()).Width(200.px()).Height(100.px()),
                                     Card(Button("Abbreviations").Link().TextCenter().AlignCenter().WidthStretch().Medium()).OnClick((s, e) => Router.Navigate("#/abbreviations"))
                                        .Margin(10.px()).Width(200.px()).Height(100.px()),
                                     Card(Button("Recently Viewed").Link().TextCenter().AlignCenter().WidthStretch().Medium()).OnClick((s, e) => Router.Navigate("#/history"))
                                        .Margin(10.px()).Width(200.px()).Height(100.px())),

                             Stack().Vertical().Width(50.percent()).AlignCenter().MarginTop(100.px())
                                .Children(
                                    TextBlock("Acknowledgements").AlignCenter().SemiBold(),
                                    Raw(Image(_("w-100",src: @"https://www.kaggle.com/static/images/covid-19.png"))),
                                    TextBlock("The dataset used in this website was created by the Allen Institute for AI in partnership with the Chan Zuckerberg Initiative, Georgetown University’s Center for Security and Emerging Technology, Microsoft Research, and the National Library of Medicine - National Institutes of Health, in coordination with The White House Office of Science and Technology Policy.").Wrap()
                                )
                         );
        }

        public HTMLElement Render()
        {
            return _MainStack.Render();
        }
    }
}
