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


namespace HandbookApp.ViewModels
{
    [DataContract]
    public class UnauthorizedErrorViewModel : CustomBaseViewModel
    {

        [IgnoreDataMember]
        public ReactiveCommand<Unit> ClearUnauthorized;

        [IgnoreDataMember]
        private IObservable<bool> cannavigatetomain;

        [IgnoreDataMember]
        private IObservable<bool> hasunauthorizederror;

        [IgnoreDataMember]
        private IObservable<bool> isloggedin;


        public UnauthorizedErrorViewModel(IScreen hostscreen = null, params object[] args) : base(hostscreen)
        {
            _urlPathSegment = "Unauthorized Error";
            _viewModelName = this.GetType().ToString();

            ClearUnauthorized = ReactiveCommand.CreateAsyncTask(x => clearHasUnauthorizedErrorImpl());
        }

        protected override void setupObservables()
        {
            base.setupObservables();

            hasunauthorizederror = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.HasUnauthorizedError })
                .Select(d => d.CurrentState.HasUnauthorizedError);

            isloggedin = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.IsLoggedIn })
                .Select(d => d.CurrentState.IsLoggedIn);

            cannavigatetomain = navigatedaway
                .CombineLatest(navigating, (x, y) => !x && !y)
                .CombineLatest(hasunauthorizederror, (c, d) => c && !d)
                .CombineLatest(isloggedin, (e, f) => e && !f);
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

        
        private async Task clearHasUnauthorizedErrorImpl()
        {
            var t1 = Task.Run(() => { App.Store.Dispatch(new LogoutAction()); });
            var t2 = Task.Run(() => { App.Store.Dispatch(new ClearHasUnauthorizedErrorAction()); });
            await Task.Run(() => { Task.WaitAll(t1, t2); });
        }
    }
}
