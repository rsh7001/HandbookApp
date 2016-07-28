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

using HandbookApp.Utilities;
using HandbookApp.ViewModels;
using ReactiveUI;
using Xamarin.Forms;


namespace HandbookApp.Views
{
    public class UnauthorizedErrorPage : BasePage<UnauthorizedErrorViewModel>
    {
        private Button goBackMainPageButton;

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
                    (titleLabel = new Label { Text = "Unauthorized Access Error", HorizontalOptions = LayoutOptions.Center }),
                    (instructionsLabel = new Label { Text = "You are reaching this page because the Content Server does not recognize who you are. You will need to login again with the account that you first logged in with. Normally this does not happen if you check the app once a day. If you check your app once a day, please contact your app administrator.", Margin = new Thickness(5, 20, 5, 5) }),
                    (goBackMainPageButton = new Button { Text = "Continue" })
                }
            };
        }


        protected override void SetupObservables()
        {
            this.BindCommand(ViewModel, vm => vm.ClearUnauthorized, c => c.goBackMainPageButton);
        }
    }
}
