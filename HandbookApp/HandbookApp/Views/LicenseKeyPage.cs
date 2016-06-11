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
using System.Text;
using System.Threading.Tasks;
using HandbookApp.Utilities;
using HandbookApp.ViewModels;
using ReactiveUI;
using Xamarin.Forms;


namespace HandbookApp.Views
{
    public class LicenseKeyPage : BasePage<LicenseKeyViewModel>
    {
        private Button goBackButton;

        protected override void SetupViewElements()
        {
            base.SetupViewElements();

            Content = new StackLayout {
                Padding = new Thickness(20d),
                Children = {
                    (goBackButton = new Button { Text = "Back to Settings" })
                }
            };
        }

        protected override void SetupObservables()
        {
            this.BindCommand(ViewModel, vm => vm.GoBack, c => c.goBackButton)
                .DisposeWith(subscriptionDisposibles);
        }
    }
}