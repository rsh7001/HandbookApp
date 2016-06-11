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
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using HandbookApp.Utilities;
using HandbookApp.ViewModels;
using ReactiveUI;
using Xamarin.Forms;

namespace HandbookApp.Views
{
    public class SettingsPage : BasePage<SettingsViewModel>
    {
        private Button goBackButton;
        private Button goLoginPageButton;
        private Button logoutButton;
        private Button goLicenseKeyPageButton;
        private Button clearLicenseKeyButton;

        private Button toggleLoginButton;
        private Button toggleLicenseKeyButton;
        private Button toggleUpdatingButton;

        private ActivityIndicator loadingIndicator;


        protected override void SetupViewElements()
        {
            base.SetupViewElements();

            Content = new StackLayout {
                Padding = new Thickness(20d),
                Children = {
                    (loadingIndicator = new ActivityIndicator { IsVisible = false }),
                    (toggleLoginButton = new Button { Text = "Toggle Login" }),
                    (toggleLicenseKeyButton = new Button { Text = "Toggle License Key" }),
                    (toggleUpdatingButton = new Button { Text = "Toggle Updating" }),
                    (goLoginPageButton = new Button { Text = "Login" }),
                    (logoutButton = new Button { Text = "Logout" }),
                    (goLicenseKeyPageButton = new Button { Text = "Set License Key" }),
                    (clearLicenseKeyButton = new Button { Text = "Clear License Key" }),
                    (goBackButton = new Button { Text = "Back to Main Page" })

                }
            };
        }

        protected override void SetupObservables()
        {
            this.BindCommand(ViewModel, vm => vm.GoBack, c => c.goBackButton)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.GoLoginPage, c => c.goLoginPageButton)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.Logout, c => c.logoutButton)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.GoLicenseKeyPage, c => c.goLicenseKeyPageButton)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.ClearLicenseKey, c => c.clearLicenseKeyButton)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.TestLogin, c => c.toggleLoginButton)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.TestLicenseKey, c => c.toggleLicenseKeyButton)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.TestUpdating, c => c.toggleUpdatingButton)
                .DisposeWith(subscriptionDisposibles);

        }

        protected override void SetupSubscriptions()
        {
            this.WhenAnyValue(x => x.ViewModel.IsLoggedIn)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => { goLoginPageButton.IsVisible = !x; logoutButton.IsVisible = x; })
                .DisposeWith(subscriptionDisposibles);

            this.WhenAnyValue(x => x.ViewModel.IsLicensed)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => { goLicenseKeyPageButton.IsVisible = !x; clearLicenseKeyButton.IsVisible = x; })
                .DisposeWith(subscriptionDisposibles);

            this.WhenAnyValue(x => x.ViewModel.IsLoggedIn, x => x.ViewModel.IsLicensed, (isloggedin, islicensed) => isloggedin && islicensed)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => { loadingIndicator.IsVisible = x; loadingIndicator.IsRunning = x; })
                .DisposeWith(subscriptionDisposibles);

            this.WhenAnyValue(x => x.ViewModel.IsUpdating)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => { loadingIndicator.IsVisible = x; loadingIndicator.IsRunning = x; })
                .DisposeWith(subscriptionDisposibles);

        }

    }
}
