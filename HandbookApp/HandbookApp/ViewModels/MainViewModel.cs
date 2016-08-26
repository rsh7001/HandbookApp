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
using System.Reactive.Linq;
using System.Runtime.Serialization;
using HandbookApp.Actions;
using HandbookApp.States;
using HandbookApp.Utilities;
using JWT;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;


namespace HandbookApp.ViewModels
{
    [DataContract]
    public class MainViewModel : CustomBaseViewModel
    {
        private static TimeSpan throttleTime = TimeSpan.FromMilliseconds(100);
        private static TimeSpan updateInterval = TimeSpan.FromMinutes(2);
        private static TimeSpan refreshTokenCheckInterval = TimeSpan.FromDays(1);
        private const  string JWTExpiryTimeKey = "exp";
        private const  int MinimumRefreshTokenPeriodInDays = 15;

        [IgnoreDataMember] public extern string LastUpdateTime { [ObservableAsProperty]get; }

        [DataMember][Reactive] public List<MainViewBookTileViewModel> Handbooks { get; set; }
        [DataMember][Reactive] public bool BackgroundTaskRunning { get; set; }

        [IgnoreDataMember] private IObservable<bool> canGoToLicenseKeyView;
        [IgnoreDataMember] private IObservable<bool> canGoToLoginView;
        [IgnoreDataMember] private IObservable<bool> canGoToLicensingErrorView;
        [IgnoreDataMember] private IObservable<bool> canGoToUnauthorizedErrorView;
        [IgnoreDataMember] private IObservable<bool> canDoUpdate;
        [IgnoreDataMember] private IObservable<bool> canCheckLicenceKey;
        [IgnoreDataMember] private IObservable<bool> islicensed;
        [IgnoreDataMember] private IObservable<bool> isloggedin;
        [IgnoreDataMember] private IObservable<bool> islicencekeyset;
        [IgnoreDataMember] private IObservable<bool> haslicensederror;
        [IgnoreDataMember] private IObservable<bool> hasunauthorizederror;
        [IgnoreDataMember] private IObservable<bool> checkinglk;
        [IgnoreDataMember] private IObservable<bool> checklgin;
        [IgnoreDataMember] private IObservable<bool> updatingdata;
        [IgnoreDataMember] private IObservable<bool> dataupdated;
        [IgnoreDataMember] private IObservable<bool> isbackgroundtaskrunning;

        [IgnoreDataMember] private IObservable<List<Book>> books;


        public MainViewModel(IScreen hostscreen) : base(hostscreen)
        {
            _viewModelName = this.GetType().ToString();
            _urlPathSegment = "Main";
            

            Handbooks = new List<MainViewBookTileViewModel>();

            App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.LastUpdateTime })
                .Select(u => "Last Updated: " + u.CurrentState.LastUpdateTime.ToLocalTime().ToString("G"))
                .ToPropertyEx(
                    source: this,
                    property: x => x.LastUpdateTime,
                    scheduler: RxApp.MainThreadScheduler
                );
        }


        protected override void setupObservables()
        {
            base.setupObservables();
            
            isloggedin = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.IsLoggedIn })
                .Select(d => d.CurrentState.IsLoggedIn);

            islicencekeyset = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.IsLicenceKeySet })
                .Select(d => d.CurrentState.IsLicenceKeySet);

            haslicensederror = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.HasLicensedError })
                .Select(d => d.CurrentState.HasLicensedError);

            hasunauthorizederror = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.HasUnauthorizedError })
                .Select(d => d.CurrentState.HasUnauthorizedError);

            checkinglk = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.VerifyingLicenceKey })
                .Select(d => d.CurrentState.VerifyingLicenceKey);

            checklgin = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.CheckingLogin })
                .Select(d => d.CurrentState.CheckingLogin);

            updatingdata = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.UpdatingData })
                .Select(d => d.CurrentState.UpdatingData);


            islicensed = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.IsLicensed })
                .Select(d => d.CurrentState.IsLicensed);

            dataupdated = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.IsDataUpdated })
                .Select(d => d.CurrentState.IsDataUpdated);

            books = App.Store
                .DistinctUntilChanged(state => new { state.Books })
                .Select(d => d.Books.Values.OrderBy(y => y.OrderIndex).ToList());

            canDoUpdate = navigatedaway
                .CombineLatest(navigating, (x, y) => !x && !y)
                .CombineLatest(islicensed, (a, b) => a && b)
                .CombineLatest(updatingdata, (c, d) => c && !d)
                .CombineLatest(dataupdated, (e, f) => e && !f);

            canCheckLicenceKey = navigatedaway
                .CombineLatest(navigating, (x, y) => !x && !y)
                .CombineLatest(isloggedin, (e, f) => e && f)
                .CombineLatest(checkinglk, (v, w) => v && !w)
                .CombineLatest(islicensed, (a, b) => a && !b)
                .CombineLatest(islicencekeyset, (c, d) => c && d)
                .CombineLatest(haslicensederror, (g, h) => g && !h)
                .CombineLatest(hasunauthorizederror, (i, j) => i && !j);

            canGoToLoginView = navigatedaway
                .CombineLatest(navigating, (x, y) => !x && !y)
                .CombineLatest(isloggedin, (a, b) => a && !b)
                .CombineLatest(islicensed, (c, d) => c && !d)
                .CombineLatest(haslicensederror, (g, h) => g && !h)
                .CombineLatest(hasunauthorizederror, (i, j) => i && !j);

            canGoToLicenseKeyView = navigatedaway
                .CombineLatest(navigating, (x, y) => !x && !y)
                .CombineLatest(isloggedin, (a, b) => a && b)
                .CombineLatest(islicencekeyset, (e, f) => e && !f)
                .CombineLatest(islicensed, (c, d) => c && !d)
                .CombineLatest(haslicensederror, (g, h) => g && !h)
                .CombineLatest(hasunauthorizederror, (i, j) => i && !j);

            canGoToLicensingErrorView = navigatedaway
                .CombineLatest(navigating, (x, y) => !x && !y)
                .CombineLatest(haslicensederror, (g, h) => g && h)
                .CombineLatest(hasunauthorizederror, (i, j) => i && !j);

            canGoToUnauthorizedErrorView = navigatedaway
                .CombineLatest(navigating, (x, y) => !x && !y)
                .CombineLatest(haslicensederror, (g, h) => g && !h)
                .CombineLatest(hasunauthorizederror, (i, j) => i && j);

            isbackgroundtaskrunning = checkinglk
                .CombineLatest(checklgin, (x, y) => x || y)
                .CombineLatest(updatingdata, (a, b) => a || b);


            // Update Interval Timer
            Observable
                .Interval(updateInterval)
                .Subscribe(
                    x => {
                        checkUpdates();
                    });


            // RefreshToken Interval Timer
            Observable
                .Interval(refreshTokenCheckInterval)
                .Subscribe(
                    x => {
                        checkRefreshToken();
                    });
        }

        private void checkUpdates()
        {
            if (App.Store.GetState().CurrentState.IsLicensed)
            {
                App.Store.Dispatch(ServerActionCreators.ServerFullUpdateAction());
            }
        }

        private void checkRefreshToken()
        {
            if(!App.Store.GetState().CurrentState.IsLicensed)
            {
                return;
            }

            if(shouldGetRefreshToken(App.Store.GetState().CurrentState.AuthToken))
            {
                App.Store.Dispatch(ServerActionCreators.ServerRefreshTokenAction());
            }
        }

        private bool shouldGetRefreshToken(string authToken)
        {
            Dictionary<string, object> payload = JsonWebToken.DecodeToObject(authToken, string.Empty, false) as Dictionary<string, object>;
            object expiryTimeObject;
            payload.TryGetValue(JWTExpiryTimeKey, out expiryTimeObject);
            var expiryTime = (long) expiryTimeObject;
            DateTimeOffset dtDateTime = new DateTimeOffset(1970,1,1,0,0,0,0,TimeSpan.FromHours(0));
            dtDateTime = dtDateTime.AddSeconds(expiryTime).ToLocalTime();
            var duration = dtDateTime.Subtract(DateTimeOffset.UtcNow);
            return (duration.TotalDays < MinimumRefreshTokenPeriodInDays);
        }

        protected override void setupSubscriptions()
        {
            base.setupSubscriptions();

            books
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(
                    xs => {
                        var newlist = new List<MainViewBookTileViewModel>();
                        newlist.AddRange(xs.Select(x => new MainViewBookTileViewModel(x)));
                        Handbooks.Clear();
                        Handbooks = newlist;
                    }
                )
                .DisposeWith(subscriptionDisposibles);


            canDoUpdate
                .Throttle(throttleTime)
                .DistinctUntilChanged()
                .Subscribe(
                    x => {
                        if (x)
                        {
                            App.Store.Dispatch(ServerActionCreators.ServerFullUpdateAction());
                        }
                    }
                )
                .DisposeWith(subscriptionDisposibles);


            canCheckLicenceKey
                .Throttle(throttleTime)
                .DistinctUntilChanged()
                .Subscribe(
                    x => {
                        if (x)
                        {
                            App.Store.Dispatch(ServerActionCreators.ServerVerifyLicenceKeyAction());
                        }
                    }
                )
                .DisposeWith(subscriptionDisposibles);


            canGoToLoginView
                .Throttle(throttleTime)
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(
                    x => {
                        if (x)
                        {
                            NavigateTo<LoginViewModel>();
                        }
                    }
                )
                .DisposeWith(subscriptionDisposibles);

            canGoToLicenseKeyView
                .Throttle(throttleTime)
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(
                    x => {
                        if (x)
                        {
                            NavigateTo<LicenseKeyViewModel>();
                        }
                    }
                )
                .DisposeWith(subscriptionDisposibles);

            canGoToLicensingErrorView
                .Throttle(throttleTime)
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(
                    x => {
                        if (x)
                        {
                            NavigateTo<LicensingErrorViewModel>();
                        }
                    }
                )
                .DisposeWith(subscriptionDisposibles);

            canGoToUnauthorizedErrorView
                .Throttle(throttleTime)
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(
                    x => {
                        if (x)
                        {
                            NavigateTo<UnauthorizedErrorViewModel>();
                        }
                    }
                )
                .DisposeWith(subscriptionDisposibles);       

            isbackgroundtaskrunning
                .Throttle(throttleTime)
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(
                    x => {
                        BackgroundTaskRunning = x;
                    }
                )
                .DisposeWith(subscriptionDisposibles);

        }
    }
}
 