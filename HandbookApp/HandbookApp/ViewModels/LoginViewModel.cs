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
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Xamarin.Forms;
using HandbookApp.Actions;
using HandbookApp.Utilities;
using Microsoft.WindowsAzure.MobileServices;



namespace HandbookApp.ViewModels
{
    public class LoginViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; protected set; }

        public string UrlPathSegment
        {
            get { return "Login"; }
        }

        [Reactive] public bool IsLoggedIn { get; set; }
        //public extern bool IsLoggedIn { [ObservableAsProperty]get; }
        public extern bool CheckingLogin { [ObservableAsProperty]get; }

        public ReactiveCommand<Unit> LoginGoogleProvider;
        public ReactiveCommand<Unit> LoginMicrosoftProvider;
        public ReactiveCommand<Unit> LoginFacebookProvider;
        public ReactiveCommand<Unit> LoginTwitterProvider;

        public ReactiveCommand<Object> GoSetLicenseKeyPage;

        public ReactiveCommand<Unit> GoBack;

        //private IObservable<bool> canGoBack;
        
        public LoginViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            //canGoBack = this.WhenAnyValue(x => x.IsLoggedIn);

            //GoBack = ReactiveCommand.CreateAsyncObservable(canGoBack, x => gobackImpl());
            GoBack = HostScreen.Router.NavigateBack;
            
            LoginGoogleProvider = ReactiveCommand.CreateAsyncObservable(x => loginGoogleProviderImpl());
            LoginMicrosoftProvider = ReactiveCommand.CreateAsyncObservable(x => loginMicrosoftProviderImpl());
            LoginFacebookProvider = ReactiveCommand.CreateAsyncObservable(x => loginFacebookProviderImpl());
            LoginTwitterProvider = ReactiveCommand.CreateAsyncObservable(x => loginTwitterProviderImpl());

            GoSetLicenseKeyPage = ReactiveCommand.CreateAsyncObservable(x => gotoLicenseKeyImpl());

            //App.Store
            //    .DistinctUntilChanged(state => new { state.CurrentState })
            //    .Select(d => d.CurrentState.IsLoggedIn)
            //    .ToPropertyEx(
            //        source: this,
            //        property: x => x.IsLoggedIn,
            //        scheduler: RxApp.MainThreadScheduler);

            App.Store
                .DistinctUntilChanged(state => new { state.CurrentState })
                .Select(d => d.CurrentState.CheckingLogin)
                .ToPropertyEx(
                    source: this,
                    property: x => x.CheckingLogin,
                    scheduler: RxApp.MainThreadScheduler);

            setupSubscriptions();
        }

        //private IObservable<Unit> gobackImpl()
        //{
        //    HostScreen.Router.NavigateBack.Execute(this);
        //    return Observable.Start(() => { return Unit.Default; });
        //}

        private void setupSubscriptions()
        {
            App.Store
                .DistinctUntilChanged(state => new { state.CurrentState })
                .Select(u => u.CurrentState.IsLoggedIn)
                .Subscribe(x => IsLoggedIn = x);
        }

        private IObservable<Unit> loginTwitterProviderImpl()
        {
            App.Store.Dispatch(AzureActionCreators.LoginAction(MobileServiceAuthenticationProvider.Twitter));
            return Observable.Start(() => { return Unit.Default; });
        }

        private IObservable<Unit> loginFacebookProviderImpl()
        {
            App.Store.Dispatch(AzureActionCreators.LoginAction(MobileServiceAuthenticationProvider.Facebook));
            return Observable.Start(() => { return Unit.Default; });
        }

        private IObservable<Unit> loginMicrosoftProviderImpl()
        {
            App.Store.Dispatch(AzureActionCreators.LoginAction(MobileServiceAuthenticationProvider.MicrosoftAccount));
            return Observable.Start(() => { return Unit.Default; });
        }

        private IObservable<Unit> loginGoogleProviderImpl()
        {
            App.Store.Dispatch(AzureActionCreators.LoginAction(MobileServiceAuthenticationProvider.Google));
            return Observable.Start(() => { return Unit.Default; });
        }

        private IObservable<Object> gotoLicenseKeyImpl()
        {
            return HostScreen.Router.Navigate.ExecuteAsync(new LicenseKeyViewModel(HostScreen));
        }
        
    }
}
