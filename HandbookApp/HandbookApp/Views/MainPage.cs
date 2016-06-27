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
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using HandbookApp.States;
using HandbookApp.Utilities;
using HandbookApp.ViewModels;
using ReactiveUI;
using Xamarin.Forms;

namespace HandbookApp.Views
{
    public class MainPage : BasePage<MainViewModel>
    {
        private ListView booksList;
        private Label title;
        private Label updateTime;
        private ActivityIndicator updatingSpinner;
        
        private IObservable<EventPattern<ItemTappedEventArgs>> itemtapped;
        
        public MainPage()
        {
            SetupViewElements();
        }

        protected override void SetupViewElements()
        {
            base.SetupViewElements();

            string Title = "CHONY Handbook App";

            var handbookDateTemplate = new DataTemplate(() => {

                var titleLabel = new Label {
                    FontAttributes = FontAttributes.Bold,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };

                titleLabel.SetBinding(Label.TextProperty, "Title");

                return new ViewCell { View = titleLabel };
            });

            Content = new ScrollView {
                Content = new StackLayout {
                    Padding = new Thickness(20d),
                    Children = {
                        (updateTime = new Label {
                            FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                            TextColor = Color.Red,
                            HorizontalOptions =LayoutOptions.Start
                        }),
                        (title = new Label {
                            Text = Title,
                            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                            HorizontalOptions =LayoutOptions.Center
                        }),
                        (updatingSpinner = new ActivityIndicator { IsVisible = true, IsRunning = false }),
                        (booksList = new ListView { ItemTemplate = handbookDateTemplate, SeparatorVisibility = SeparatorVisibility.None }),
                    }
                }
            };
        }

        protected override void SetupObservables()
        {
            itemtapped = Observable.FromEventPattern<ItemTappedEventArgs>(
                ev => booksList.ItemTapped += ev,
                ev => booksList.ItemTapped -= ev);

            this.OneWayBind(ViewModel, vm => vm.IsUpdatingData, c => c.updatingSpinner.IsRunning);
            this.OneWayBind(ViewModel, vm => vm.LastUpdateTime, c => c.updateTime.Text);
            this.OneWayBind(ViewModel, vm => vm.Handbooks, c => c.booksList.ItemsSource);

        }

        protected override void SetupSubscriptions()
        {
            itemtapped
                .ObserveOn(RxApp.MainThreadScheduler)
                .Throttle(TimeSpan.FromMilliseconds(10))
                .Select(x => ((Book)x.EventArgs.Item).StartingBookpage)
                .Subscribe(x => ViewModel.OpenThisBook(x))
                .DisposeWith(subscriptionDisposibles);

            App.Store
                .DistinctUntilChanged(s => s.CurrentState.IsLoggedIn)
                .Select(d => d.CurrentState.IsLoggedIn)
                .Subscribe(x => checkLoggedIn(x));
        }

        private void checkLoggedIn(bool x)
        {
            System.Diagnostics.Debug.WriteLine("CheckLoggedIn");
            System.Diagnostics.Debug.WriteLine("AppStore: " + x.ToString());
            System.Diagnostics.Debug.WriteLine("ViewModel: " + ViewModel.IsLoggedIn.ToString());
            checkAuthenticateAndLoadData();
        }

        static int countLoadData = 0;

        private void checkAuthenticateAndLoadData()
        {
            countLoadData++;
            System.Diagnostics.Debug.WriteLine(countLoadData.ToString());
            if (!ViewModel.IsLicensed)
            {

                ViewModel.HostScreen.Router.Navigate.Execute(new LicenseKeyViewModel());
                return;
            }

            if (!ViewModel.IsLoggedIn)
            {
                ViewModel.HostScreen.Router.Navigate.Execute(new LoginViewModel());
                return;
            }

            booksList.SelectedItem = null;

            //if (!ViewModel.Updated && !ViewModel.Updating)
            //{
            //    ViewModel.DoUpdate.Execute(this);
            //}
            ViewModel.DoUpdate.Execute(this);

        }
    }
}
