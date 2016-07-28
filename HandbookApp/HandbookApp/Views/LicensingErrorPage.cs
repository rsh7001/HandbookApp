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

using HandbookApp.ViewModels;
using ReactiveUI;
using Xamarin.Forms;


namespace HandbookApp.Views
{
    class LicensingErrorPage : BasePage<LicensingErrorViewModel>
    {
        private Button logoutButton;
        private Button resetLicenceKeyButton;

        private Label titleLabel;
        private Label instructionsLabel;


        protected override void SetupViewElements()
        {
            base.SetupViewElements();

            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            Content = new StackLayout {
                Padding = new Thickness(20d),
                Children = {
                    (titleLabel = new Label { Text = "Licensing Verification Failure", HorizontalOptions = LayoutOptions.Center }),
                    (instructionsLabel = new Label { Text = "You are reaching this page because the Content Server does not recognize your licence key. Each licence key is associated with the first login account that you used. Either the licence key is incorrect or you should try a different login account provider. If you haven't changed your licence key or your account login recently then please contact your app administrator.", Margin = new Thickness(5, 20, 5, 5) }),
                    (logoutButton = new Button { Text = "Logout and Try a different account" }),
                    (resetLicenceKeyButton = new Button { Text = "Reset Licence Key" })
                }
            };
        }

        protected override void SetupObservables()
        {
            this.BindCommand(ViewModel, vm => vm.Logout, c => c.logoutButton);
            this.BindCommand(ViewModel, vm => vm.ResetLicenceKey, c => c.resetLicenceKeyButton);
        }
    }
}
