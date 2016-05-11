//
//  Copyright 2016  R. Stanley Hum <r.stanley.hum@gmail.com>
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using HandbookApp.Utilities;
using HandbookApp.ViewModels;
using ReactiveUI;
using Xamarin.Forms;


namespace HandbookApp.Views
{
    public class BookpagePage : BasePage<BookpageViewModel>
    {
        private Button goBackButton;

        private Label header;
        private Label title;
        private Label articleId;
        private Label linksTitle;
        private Label linksCount;
        private StackLayout links;

        public BookpagePage()
        {
        }

        protected override void SetupViewElements()
        {
            base.SetupViewElements();

            Content = new ScrollView {
                Content = new StackLayout {
                    Padding = new Thickness(20d),
                    Children = {
                        (header = new Label { Text = "", HorizontalOptions=LayoutOptions.Center }),
                        (title = new Label { Text = "", HorizontalOptions=LayoutOptions.Center, IsVisible=false }),
                        (articleId = new Label { Text = "", HorizontalOptions=LayoutOptions.Center, IsVisible=false }),
                        (linksTitle = new Label { Text = "", HorizontalOptions=LayoutOptions.Center, IsVisible=false }),
                        (linksCount = new Label { Text = "", HorizontalOptions=LayoutOptions.Center, IsVisible=false }),
                        (goBackButton = new Button { Text = "GoBack" }),
                        (links = new StackLayout())
                    }
                }
            };
        }

        protected override void SetupObservables()
        {
            this.BindCommand(ViewModel, vm => vm.GoBack, c => c.goBackButton)
                .DisposeWith(subscriptionDisposibles);
        }

        protected override void SetupSubscriptions()
        {
            this.WhenAnyValue(x => x.ViewModel.BookpageName)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => { header.Text = x; header.IsVisible = !string.IsNullOrWhiteSpace(x); })
                .DisposeWith(subscriptionDisposibles);

            this.WhenAnyValue(x => x.ViewModel.BookpageTitle)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => { title.Text = x; title.IsVisible = !string.IsNullOrWhiteSpace(x); })
                .DisposeWith(subscriptionDisposibles);

            this.WhenAnyValue(x => x.ViewModel.BookpageArticleId)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => { articleId.Text = x; articleId.IsVisible = !string.IsNullOrWhiteSpace(x); })
                .DisposeWith(subscriptionDisposibles);

            this.WhenAnyValue(x => x.ViewModel.BookpageLinksTitle)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => { linksTitle.Text = x; linksTitle.IsVisible = !string.IsNullOrWhiteSpace(x); })
                .DisposeWith(subscriptionDisposibles);

            this.WhenAnyValue(x => x.ViewModel.BookpageLinks)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => { linksCount.Text = x.Count.ToString(); linksCount.IsVisible = true; })
                .DisposeWith(subscriptionDisposibles);

            this.WhenAnyValue(x => x.ViewModel.BookpageLinks)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => setLinksChildren(x))
                .DisposeWith(subscriptionDisposibles);
           
        }

        private void setLinksChildren(List<Tuple<string, string>> linkslst)
        {
            links.Children.Clear();
            foreach (var link in linkslst)
            {
                Label lbl = new Label {
                    Text = link.Item2,
                };
                lbl.GestureRecognizers.Add(new TapGestureRecognizer {
                    Command = ViewModel.GoToPage,
                    CommandParameter = link.Item1
                });
                links.Children.Add( lbl );
            }
            links.IsVisible = links.Children.Count != 0;

        }
    }
}
