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
using ReactiveUI.Fody.Helpers;
using Splat;


namespace HandbookApp.ViewModels
{
    [DataContract]
    public class LicenseKeyViewModel : CustomBaseViewModel
    {
        [Reactive][DataMember] public string LicenceKey { get; set; }

        [IgnoreDataMember]
        public ReactiveCommand<Unit> SetLicensed;


        [IgnoreDataMember]
        private IObservable<bool> cansetlicencekey;
        [IgnoreDataMember]
        private IObservable<bool> islicencekeyset;
        [IgnoreDataMember]
        private IObservable<bool> cannavigatetomain;


        public LicenseKeyViewModel(IScreen hostscreen = null, params object[] args) : base(hostscreen)
        {
            _viewModelName = this.GetType().ToString();
            _urlPathSegment = "Licence Key";

            SetLicensed = ReactiveCommand.CreateAsyncTask(cansetlicencekey, x => setLicensedImpl());
        }

        protected override void setupObservables()
        {
            base.setupObservables();

            islicencekeyset = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.IsLicenceKeySet })
                .Select(d => d.CurrentState.IsLicenceKeySet);

            cansetlicencekey = this.WhenAnyValue(x => x.LicenceKey, (lk) =>
                !String.IsNullOrWhiteSpace(lk) && lk.Length >= 6)
                .DistinctUntilChanged();

            cannavigatetomain = navigatedaway
                .CombineLatest(navigating, (x, y) => !x && !y)
                .CombineLatest(islicencekeyset, (a, b) => a && b);
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

        
        private async Task setLicensedImpl()
        {
            await Task.Run(() => { App.Store.Dispatch(new SetLicenceKeyAction { LicenceKey = LicenceKey }); });
        }
    }
}
