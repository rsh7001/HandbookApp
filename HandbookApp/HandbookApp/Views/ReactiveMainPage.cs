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
using System.Reactive.Linq;
using HandbookApp.Utilities;
using HandbookApp.ViewModels;
using ReactiveUI;
using Xamarin.Forms;

namespace HandbookApp.Views
{
    public class ReactiveMainPage : BasePage<ReactiveMainViewModel>
    {
        private Button updateButton;
        private Button goMainPageButton;

        private Label numBooks;
        private Label header;
        
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
                        (numBooks = new Label { Text = "", HorizontalOptions=LayoutOptions.Center }),
                        (updateButton = new Button { Text = "Update" }),
                        (goMainPageButton = new Button { Text = "MainPage" }),
                    }
                }
            };
        }

        protected override void SetupObservables()
        {
            this.BindCommand(ViewModel, vm => vm.Update, c => c.updateButton)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.GoMainPage, c => c.goMainPageButton)
                .DisposeWith(subscriptionDisposibles);
        }

        protected override void SetupSubscriptions()
        {
            this.WhenAnyValue(x => x.ViewModel.NumBooks)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => numBooks.Text = x)
                .DisposeWith(subscriptionDisposibles);
        }
    }
}
