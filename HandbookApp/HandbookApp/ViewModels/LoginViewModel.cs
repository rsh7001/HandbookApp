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
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using HandbookApp.Actions;
using HandbookApp.Utilities;
using Microsoft.WindowsAzure.MobileServices;

namespace HandbookApp.ViewModels
{
    public class LoginViewModel : ReactiveObject, IRoutableViewModel
    {
        [Reactive] public string PageTitle { get; set; }

        public string UrlPathSegment
        {
            get
            {
                return "Login";
            }
        }

        public IScreen HostScreen
        {
            get; protected set;
        }

        public ReactiveCommand<Unit> LoginGoogleProvider;
        public ReactiveCommand<Unit> LoginMicrosoftProvider;
        public ReactiveCommand<Unit> LoginFacebookProvider;
        public ReactiveCommand<Unit> LoginTwitterProvider;
        public ReactiveCommand<Unit> GoBack;

        private IObservable<bool> isloggedin;

        public LoginViewModel(IScreen hostscreen = null)
        {
            HostScreen = hostscreen ?? Locator.Current.GetService<IScreen>();

            PageTitle = "LoginPage";

            LoginGoogleProvider = ReactiveCommand.CreateAsyncTask(x => loginGoogleProviderImpl());
            LoginMicrosoftProvider = ReactiveCommand.CreateAsyncTask(x => loginMicrosoftProviderImpl());
            LoginFacebookProvider = ReactiveCommand.CreateAsyncTask(x => loginFacebookProviderImpl());
            LoginTwitterProvider = ReactiveCommand.CreateAsyncTask(x => loginTwitterProviderImpl());
            
            isloggedin = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.IsLoggedIn })
                .Select(d => d.CurrentState.IsLoggedIn);

            GoBack = ReactiveCommand.CreateAsyncTask(x => goBackImpl());

            isloggedin
                .Where(y => y == true)
                .DistinctUntilChanged()
                .InvokeCommand(this, x => x.GoBack);
        }

        private async Task goBackImpl()
        {
            var vm = HostScreen.Router.NavigationStack.First();
            await HostScreen.Router.NavigateAndReset.ExecuteAsyncTask(vm);
            App.Store.Dispatch(new ClearOnLoginPageAction());
        }

        private async Task loginTwitterProviderImpl()
        {
            await App.Store.Dispatch(AzureActionCreators.LoginAction(MobileServiceAuthenticationProvider.Twitter));
        }

        private async Task loginFacebookProviderImpl()
        {
            await App.Store.Dispatch(AzureActionCreators.LoginAction(MobileServiceAuthenticationProvider.Facebook));
        }

        private async Task loginMicrosoftProviderImpl()
        {
            await App.Store.Dispatch(AzureActionCreators.LoginAction(MobileServiceAuthenticationProvider.MicrosoftAccount));
        }

        private async Task loginGoogleProviderImpl()
        {
            await App.Store.Dispatch(AzureActionCreators.LoginAction(MobileServiceAuthenticationProvider.Google));
        }        
    }
}
