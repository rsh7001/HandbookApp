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
using HandbookApp.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace HandbookApp.ViewModels
{
    public class SettingsViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; protected set; }

        public string UrlPathSegment
        {
            get { return "Settings"; }
        }

        [Reactive] public bool IsLoggedIn { get; set; }
        [Reactive] public bool IsLicensed { get; set; }
        
        public extern bool IsUpdating { [ObservableAsProperty]get; }

        public ReactiveCommand<Unit>   GoBack;
        public ReactiveCommand<Object> GoLoginPage;
        public ReactiveCommand<Object> GoLicenseKeyPage;
        public ReactiveCommand<Unit>   Logout;
        public ReactiveCommand<Unit>   ClearLicenseKey;

        public ReactiveCommand<Unit> TestLogin;
        public ReactiveCommand<Unit> TestLicenseKey;
        public ReactiveCommand<Unit> TestUpdating;


        public SettingsViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            GoBack = HostScreen.Router.NavigateBack;

            GoLoginPage = ReactiveCommand.CreateAsyncObservable(x => HostScreen.Router.Navigate.ExecuteAsync(new LoginViewModel(HostScreen)));

            GoLicenseKeyPage = ReactiveCommand.CreateAsyncObservable(x => HostScreen.Router.Navigate.ExecuteAsync(new LicenseKeyViewModel(HostScreen)));

            TestLogin = ReactiveCommand.CreateAsyncObservable(x => toggleLoginImpl());

            TestLicenseKey = ReactiveCommand.CreateAsyncObservable(x => toggleLicenseKey());

            TestUpdating = ReactiveCommand.CreateAsyncObservable(x => toggleUpdating());
            TestUpdating.IsExecuting.ToPropertyEx(this, x => x.IsUpdating);

            Logout = ReactiveCommand.CreateAsyncObservable(x => logoutImpl());

            ClearLicenseKey = ReactiveCommand.CreateAsyncObservable(x => ClearLicenseKeyImpl());

            IsLoggedIn = false;
            IsLicensed = false;

            //this.WhenAnyValue(x => x.IsLoggedIn, x => x.IsLicensed, x => x.IsUpdating, (isloggedin, islicenced, isupdating) => isloggedin && islicenced && !isupdating)
            //    .Subscribe(x => TestUpdating.Execute(x));
                
        }

        private IObservable<Unit> toggleUpdating()
        {
            System.Diagnostics.Debug.WriteLine("Before");
            JsonServerService.JsonServerUpdate();
            System.Diagnostics.Debug.WriteLine("After");
            return Observable.Start(() => { return Unit.Default; });
        }

        private IObservable<Unit> ClearLicenseKeyImpl()
        {
            IsLicensed = false;
            return Observable.Start(() => { return Unit.Default; });
        }

        private IObservable<Unit> toggleLicenseKey()
        {
            IsLicensed = !IsLicensed;
            return Observable.Start(() => { return Unit.Default; });
        }

        private IObservable<Unit> logoutImpl()
        {
            IsLoggedIn = false;
            return Observable.Start(() => { return Unit.Default; });
        }

        private IObservable<Unit> toggleLoginImpl()
        {
            IsLoggedIn = !IsLoggedIn;
            return Observable.Start(() => { return Unit.Default; });
        }


    }
}
