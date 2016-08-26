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
using HandbookApp.Actions;
using HandbookApp.Utilities;
using Microsoft.WindowsAzure.MobileServices;
using System.Runtime.Serialization;
using Splat;

namespace HandbookApp.ViewModels
{
    [DataContract]
    public class LoginViewModel : CustomBaseViewModel
    {
        [IgnoreDataMember]
        public ReactiveCommand<Unit> LoginGoogleProvider;
        [IgnoreDataMember]
        public ReactiveCommand<Unit> LoginMicrosoftProvider;
        [IgnoreDataMember]
        public ReactiveCommand<Unit> LoginFacebookProvider;
        [IgnoreDataMember]
        public ReactiveCommand<Unit> LoginTwitterProvider;

        [IgnoreDataMember]
        private IObservable<bool> isloggedin;
        [IgnoreDataMember]
        private IObservable<bool> cannavigatetomain;


        public LoginViewModel(IScreen hostscreen = null, params object[] args) : base(hostscreen)
        {
            _viewModelName = this.GetType().ToString();
            _urlPathSegment = "Login";

            LoginGoogleProvider = ReactiveCommand.CreateAsyncTask(x => loginGoogleProviderImpl());
            LoginMicrosoftProvider = ReactiveCommand.CreateAsyncTask(x => loginMicrosoftProviderImpl());
            LoginFacebookProvider = ReactiveCommand.CreateAsyncTask(x => loginFacebookProviderImpl());
            LoginTwitterProvider = ReactiveCommand.CreateAsyncTask(x => loginTwitterProviderImpl());
        }


        protected override void setupObservables()
        {
            base.setupObservables();

            isloggedin = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.IsLoggedIn })
                .Select(d => d.CurrentState.IsLoggedIn);

            cannavigatetomain = navigatedaway
                .CombineLatest(navigating, (x, y) => !x && !y)
                .CombineLatest(isloggedin, (a, b) => a && b);
        }


        protected override void setupSubscriptions()
        {
            base.setupSubscriptions();

            cannavigatetomain
                .Throttle(TimeSpan.FromMilliseconds(100))
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async x => {
                    if (x)
                    {
                        await NavigateToMainAsync();
                    }
                })
                .DisposeWith(subscriptionDisposibles);
        }

        private async Task loginTwitterProviderImpl()
        {
            await App.Store.Dispatch(ServerActionCreators.LoginAction(MobileServiceAuthenticationProvider.Twitter));
        }

        private async Task loginFacebookProviderImpl()
        {
            await App.Store.Dispatch(ServerActionCreators.LoginAction(MobileServiceAuthenticationProvider.Facebook));
        }

        private async Task loginMicrosoftProviderImpl()
        {
            await App.Store.Dispatch(ServerActionCreators.LoginAction(MobileServiceAuthenticationProvider.MicrosoftAccount));
        }

        private async Task loginGoogleProviderImpl()
        {
            await App.Store.Dispatch(ServerActionCreators.LoginAction(MobileServiceAuthenticationProvider.Google));
        }
    }
}
