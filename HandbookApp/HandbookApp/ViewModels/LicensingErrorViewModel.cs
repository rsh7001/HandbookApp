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
using System.Runtime.Serialization;
using System.Threading.Tasks;
using HandbookApp.Actions;
using HandbookApp.Utilities;
using ReactiveUI;
using Splat;

namespace HandbookApp.ViewModels
{
    [DataContract]
    public class LicensingErrorViewModel : CustomBaseViewModel
    {
        [IgnoreDataMember]
        public ReactiveCommand<Unit> Logout;
        [IgnoreDataMember]
        public ReactiveCommand<Unit> ResetLicenceKey;
        
        [IgnoreDataMember]
        private IObservable<bool> cannavigatetomain;
        [IgnoreDataMember]
        private IObservable<bool> haslicensederror;


        public LicensingErrorViewModel(IScreen hostscreen = null, params object[] args) : base(hostscreen)
        {
            _viewModelName = this.GetType().ToString();
            _urlPathSegment = "Licensing Failure";

            Logout = ReactiveCommand.CreateAsyncTask(x => logoutImpl());
            ResetLicenceKey = ReactiveCommand.CreateAsyncTask(x => resetLicenceKeyImpl());
        }


        protected override void setupObservables()
        {
            base.setupObservables();

            haslicensederror = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.HasLicensedError })
                .Select(d => d.CurrentState.HasLicensedError);

            cannavigatetomain = navigatedaway
                .CombineLatest(navigating, (x, y) => !x && !y)
                .CombineLatest(haslicensederror, (a, b) => a && !b);
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

        private async Task resetLicenceKeyImpl()
        {
            await Task.Run(() => { App.Store.Dispatch(new ClearLicenceKeyAction()); } );
            await Task.Run(() => { App.Store.Dispatch(new ClearHasLicensedErrorAction()); });
        }

        private async Task logoutImpl()
        {
            await Task.Run(() => { App.Store.Dispatch(new LogoutAction()); });
            await Task.Run(() => { App.Store.Dispatch(new ClearHasLicensedErrorAction()); });
        }
    }
}
