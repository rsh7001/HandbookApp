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
using HandbookApp.Utilities;
using HandbookApp.ViewModels;
using ReactiveUI;
using Xamarin.Forms;
using System.Reactive.Linq;


namespace HandbookApp.Views
{
    public class LoginPage : BasePage<LoginViewModel>
    {
        private ActivityIndicator updatingSpinner;
        private Button loginGoogleButton;
        private Button loginFacebookButton;
        private Button loginMicrosoftButton;
        private Button loginTwitterButton;
        
        private Label title;
        private Label instructionsLabel;

        protected override void SetupViewElements()
        {
            base.SetupViewElements();

            Content = new StackLayout {
                Padding = new Thickness(20d),
                Children = {
                    (title = new Label { Text = "CHONY Handbook App", HorizontalOptions = LayoutOptions.Center }),
                    (updatingSpinner = new ActivityIndicator { IsVisible = true, IsRunning = false }),
                    (instructionsLabel = new Label { Text = "Login with one of the following providers.", Margin = new Thickness(5, 20, 5, 5) }),
                    (loginGoogleButton = new Button { Text = "Google" }),
                    (loginFacebookButton = new Button { Text = "Facebook" }),
                    (loginMicrosoftButton = new Button { Text = "Microsoft" }),
                    (loginTwitterButton = new Button { Text = "Twitter" }),
                }
            };
        }

        protected override void SetupObservables()
        {
            this.BindCommand(ViewModel, vm => vm.LoginGoogleProvider, c => c.loginGoogleButton)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.LoginMicrosoftProvider, c => c.loginMicrosoftButton)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.LoginFacebookProvider, c => c.loginFacebookButton)
                .DisposeWith(subscriptionDisposibles);

            this.BindCommand(ViewModel, vm => vm.LoginTwitterProvider, c => c.loginTwitterButton)
                .DisposeWith(subscriptionDisposibles);
        }
    }
}
