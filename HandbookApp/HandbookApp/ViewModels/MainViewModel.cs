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
using System.Threading.Tasks;
using HandbookApp.Actions;
using HandbookApp.States;
using HandbookApp.Utilities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace HandbookApp.ViewModels
{
    public class MainViewModel : ReactiveObject, IRoutableViewModel
    {
        public extern string LastUpdateTime { [ObservableAsProperty]get; }
        
        [Reactive] public List<MainViewBookTileViewModel> Handbooks { get; set; }
        [Reactive] public bool BackgroundTaskRunning { get; set; }

        private extern List<Book> MyHandbooks { [ObservableAsProperty]get; }

        [Reactive] private bool Navigating { get; set; }

        public string UrlPathSegment
        {
            get
            {
                return "Main";
            }
        }

        public IScreen HostScreen { get; protected set; }

        private IObservable<bool> canGoToLicenseKeyView;
        private IObservable<bool> canGoToLoginView;
        private IObservable<bool> canDoUpdate;
        private IObservable<bool> canCheckLicenceKey;

        private IObservable<bool> islicensed;
        private IObservable<bool> isloggedin;
        private IObservable<bool> islicencekeyset;
        
        private IObservable<bool> checkinglk;
        private IObservable<bool> checklgin;
        private IObservable<bool> updatingdata;
        private IObservable<bool> onloginpage;
        private IObservable<bool> onlicencekeypage;

        private IObservable<bool> navigating;
        private IObservable<bool> onmainpage;

        private IObservable<bool> needsupdate;

        private ReactiveCommand<Unit> DoUpdate;
        private ReactiveCommand<Unit> OpenLicenceKeyView;
        private ReactiveCommand<Unit> OpenLoginView;
        
        public MainViewModel(IScreen hostscreen = null)
        {
            HostScreen = hostscreen ?? Locator.Current.GetService<IScreen>();

            Handbooks = new List<MainViewBookTileViewModel>();
                    
                 
            App.Store
                .DistinctUntilChanged(state => new { state.CurrentState })
                .Select(u => "Last Updated: " + u.CurrentState.LastUpdateTime.ToString("u"))
                .ToPropertyEx(
                    source: this,
                    property: x => x.LastUpdateTime,
                    scheduler: RxApp.MainThreadScheduler
                );
            

            App.Store
                .DistinctUntilChanged(state => new { state.Books })
                .Select(d => d.Books.Values.OrderBy(y => y.OrderIndex).ToList())
                .ToPropertyEx(
                    source: this,
                    property: x => x.MyHandbooks,
                    scheduler: RxApp.MainThreadScheduler
                );


            setupObservables();

            DoUpdate = ReactiveCommand.CreateAsyncObservable(
                canDoUpdate, 
                x => updateImpl());

            OpenLicenceKeyView = ReactiveCommand.CreateAsyncTask(
                canGoToLicenseKeyView,
                x => openLicenceKeyView());

            OpenLoginView = ReactiveCommand.CreateAsyncTask(
                canGoToLoginView, 
                x => openLoginView());

            setupSubscriptions();
        }

        private void setupObservables()
        {
            checkinglk = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.CheckingLicenceKey })
                .Select(d => d.CurrentState.CheckingLicenceKey);

            checklgin = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.CheckingLogin })
                .Select(d => d.CurrentState.CheckingLogin);

            updatingdata = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.IsUpdatingData })
                .Select(d => d.CurrentState.IsUpdatingData);


            islicensed = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.IsLicensed })
                .Select(d => d.CurrentState.IsLicensed);

            isloggedin = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.IsLoggedIn })
                .Select(d => d.CurrentState.IsLoggedIn);

            islicencekeyset = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.IsLicenceKeySet })
                .Select(d => d.CurrentState.IsLicenceKeySet);


            onlicencekeypage = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.OnLicenceKeyPage })
                .Select(d => d.CurrentState.OnLicenceKeyPage);

            onloginpage = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.OnLoginPage })
                .Select(d => d.CurrentState.OnLoginPage);

            navigating = this.WhenAnyValue(x => x.Navigating)
                .DistinctUntilChanged();

            onmainpage = navigating
                .CombineLatest(onloginpage, (a,b) => !a && !b)
                .CombineLatest(onlicencekeypage, (c,d) => c && !d)
                .DistinctUntilChanged();

            Observable
                .Interval(TimeSpan.FromSeconds(15.0), RxApp.TaskpoolScheduler)
                .Subscribe(
                    x => { checkIfNeedsUpdate(); });
    
            canDoUpdate = islicensed;

            canCheckLicenceKey = navigating
                .CombineLatest(isloggedin, (x,y) => !x && y)
                .CombineLatest(islicensed, (a,b) => a && !b)
                .CombineLatest(islicencekeyset, (c,d) => c && d)
                .DistinctUntilChanged();
                
            canGoToLicenseKeyView = navigating
                .CombineLatest(isloggedin, (x, y) => !x && y)
                .CombineLatest(islicensed, (a, b) => a && !b)
                .CombineLatest(islicencekeyset, (c, d) => c && !d)
                .CombineLatest(onmainpage, (e, f) => e && f)
                .DistinctUntilChanged();
                
            canGoToLoginView = navigating
                .CombineLatest(isloggedin, (x,y) => !x && !y)
                .CombineLatest(islicensed, (a,b) => a && !b)
                .CombineLatest(onmainpage, (c,d) => c && d)
                .DistinctUntilChanged();
        }

        private void checkIfNeedsUpdate()
        {
            var lastupdatetime = App.Store.GetState().CurrentState.LastUpdateTime;
            var duration = DateTimeOffset.UtcNow - lastupdatetime;
            if (duration > TimeSpan.FromHours(6.0))
            {
                //App.Store.Dispatch(new )
            }
        }

        private void setupSubscriptions()
        {
            checkinglk
                .CombineLatest(checklgin, (x,y) => x || y)
                .CombineLatest(updatingdata, (a, b) => a || b)
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(z => BackgroundTaskRunning = z);

            this.WhenAnyValue(x => x.MyHandbooks)
                .DistinctUntilChanged()
                .Subscribe(
                    xs => {
                        var newlist = new List<MainViewBookTileViewModel>();
                        newlist.AddRange(xs.Select(x => new MainViewBookTileViewModel(x)));
                        Handbooks.Clear();
                        Handbooks = newlist;
                    });

            islicensed
                .Where(y => y == true)
                .DistinctUntilChanged()
                .InvokeCommand(this, x => x.DoUpdate);

            canCheckLicenceKey
                .Where(y => y == true)
                .DistinctUntilChanged()
                .Subscribe(z => { App.Store.Dispatch(AzureActionCreators.CheckLicenceKeyAction()); });

        }

        // TODO: Subscriptions which need navigation need to be setup in the OnAppearing portion of
        // the View because the _nav needs to be set up. This will change when I go back to 
        // using HostScreen in ReactiveUI
        //
        public void OnAppearingSetup()
        {
            canGoToLicenseKeyView
                .Where(y => y == true)
                .DistinctUntilChanged()
                .InvokeCommand(this, x => x.OpenLicenceKeyView);

            canGoToLoginView
                .Where(y => y == true)
                .DistinctUntilChanged()
                .InvokeCommand(this, x => x.OpenLoginView);
        }

        private IObservable<Unit> updateImpl()
        {
            App.Store.Dispatch(AzureActionCreators.ServerUpdateAction());
            return Observable.Start(() => { return Unit.Default; });
        }

        private async Task openLicenceKeyView()
        {
            if(Navigating)
                return;

            Navigating = true;
            App.Store.Dispatch(new SetOnLicenceKeyAction());
            var vm = new LicenseKeyViewModel(HostScreen);
            await HostScreen.Router.Navigate.ExecuteAsyncTask(vm);
            Navigating = false;
        }

        private async Task openLoginView()
        {
            if(Navigating)
                return;

            Navigating = true;
            App.Store.Dispatch(new SetOnLoginPageAction());
            var vm = new LoginViewModel(HostScreen);
            await HostScreen.Router.Navigate.ExecuteAsyncTask(vm);
            Navigating = false;
        }
    }
}
 