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
using System.Linq;
using System.Reactive.Linq;
using HandbookApp.States;
using HandbookApp.Utilities;
using HandbookApp.ViewModels;
using ReactiveUI;
using Xamarin.Forms;

namespace HandbookApp.Views
{
    public class ReactiveMainPage : BasePage<ReactiveMainViewModel>
    {
        private Button incrementButton;
        private Button decrementButton;
        private Button updateButton;
        private Button goMainPageButton;

        private Entry articleIdEntry;
        private Entry articleTitleEntry;

        private Label numArticles;
        private Label numBookpages;
        private Label numBooks;
        private Label header;

        private StackLayout articlesSL;

        public ReactiveMainPage()
        {
        }

        protected override void SetupViewElements()
        {
            base.SetupViewElements();

            Title = "HandbookApp";

            Content = new ScrollView {
                Content = new StackLayout {
                    Padding = new Thickness(20d),
                    Children = {
                        (header = new Label { Text = Title, HorizontalOptions=LayoutOptions.Center }),
                        (numArticles = new Label { Text = "", HorizontalOptions=LayoutOptions.Center }),
                        (numBookpages = new Label { Text = "", HorizontalOptions=LayoutOptions.Center }),
                        (numBooks = new Label { Text = "", HorizontalOptions=LayoutOptions.Center }),
                        (updateButton = new Button { Text = "Update" }),
                        (incrementButton = new Button { Text = "Increment" }),
                        (decrementButton = new Button { Text = "Decrement" }),
                        (goMainPageButton = new Button { Text = "MainPage" }),
                        (articleIdEntry = new Entry()),
                        (articleTitleEntry = new Entry()),
                        (articlesSL = new StackLayout { Orientation=StackOrientation.Vertical, VerticalOptions=LayoutOptions.FillAndExpand }),
                    }
                }
            };
        }

        protected override void SetupObservables()
        {
            this.Bind(ViewModel, vm => vm.ArticleId, c => c.articleIdEntry.Text)
                .DisposeWith(subscriptionDisposibles);

            this.Bind(ViewModel, vm => vm.ArticleTitle, c => c.articleTitleEntry.Text)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.Increment, c => c.incrementButton)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.Update, c => c.updateButton)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.Decrement, c => c.decrementButton)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.GoMainPage, c => c.goMainPageButton)
                .DisposeWith(subscriptionDisposibles);
        }

        protected override void SetupSubscriptions()
        {
            //this.WhenAnyObservable(x => x.ViewModel.Increment)
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(async _ => await DisplayAlert("Increment", "Increment Article", "Done"))
            //    .DisposeWith(subscriptionDisposibles);

            this.WhenAnyValue(x => x.ViewModel.NumArticles)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => numArticles.Text = x)
                .DisposeWith(subscriptionDisposibles);

            this.WhenAnyValue(x => x.ViewModel.NumBookpages)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => numBookpages.Text = x)
                .DisposeWith(subscriptionDisposibles);

            this.WhenAnyValue(x => x.ViewModel.NumBooks)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => numBooks.Text = x)
                .DisposeWith(subscriptionDisposibles);

            App.Store
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(state => setStackLayoutChildren(state));
        }

        private void setStackLayoutChildren(AppState state)
        {
            var elements = state.Articles.Values.OrderBy(x => x.Id)
                .Take(10) /* Limit work on Main Thread */
                .Select(x => new StackLayout {
                    Orientation = StackOrientation.Vertical,
                    Children = {
                        new Label {
                            Text = x.Id
                        },
                        new Label {
                            Text = x.Title
                        }
                    }
                });
            articlesSL.Children.Clear();
            foreach (var e in elements)
            {
                articlesSL.Children.Add(e);
            }
        }
    }
}
