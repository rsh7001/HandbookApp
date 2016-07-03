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
using HandbookApp.ViewModels;
using HandbookApp.Utilities;
using ReactiveUI;
using Xamarin.Forms;


namespace HandbookApp.Views
{
    public class LicenseKeyPage : BasePage<LicenseKeyViewModel>
    {
        private ActivityIndicator updatingSpinner;

        private Label title;
        private Label instructionsLabel;

        private Entry licenseKeyEntry;
        private Button setLicenseKeyButton;

        protected override void SetupViewElements()
        {
            base.SetupViewElements();

            Content = new StackLayout {
                Padding = new Thickness(20d),
                Children = {
                    (title = new Label {Text = "CHONY Handbook App", HorizontalOptions=LayoutOptions.Center }),
                    (updatingSpinner = new ActivityIndicator { IsVisible = true, IsRunning = false }),
                    (instructionsLabel = new Label { Text = "Please enter your license key.", Margin = new Thickness(5, 20, 5, 5) }),
                    (licenseKeyEntry = new Entry { Placeholder="License Key" }),
                    (setLicenseKeyButton = new Button { Text = "Set License Key" }),
                }
            };
        }

        protected override void SetupObservables()
        {
            this.Bind(ViewModel, vm => vm.LicenceKey, c => c.licenseKeyEntry.Text);
            this.BindCommand(ViewModel, vm => vm.SetLicensed, c => c.setLicenseKeyButton);
            this.OneWayBind(ViewModel, vm => vm.PageTitle, c => c.Title);
        }
    }
}