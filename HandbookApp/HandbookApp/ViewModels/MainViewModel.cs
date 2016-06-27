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
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using HandbookApp.Actions;
using HandbookApp.States;
using HandbookApp.Utilities;
using MvvmHelpers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace HandbookApp.ViewModels
{

    public class MainViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; protected set; }

        public string UrlPathSegment
        {
            get { return "Main"; }
        }

        public extern string LastUpdateTime { [ObservableAsProperty]get; }
        public extern bool IsUpdatingData { [ObservableAsProperty]get; }
        [Reactive]public bool IsLoggedIn { get; set; }
        //public extern bool LoggedIn { [ObservableAsProperty]get; }
        public extern bool IsLicensed { [ObservableAsProperty]get; }
        public extern List<Book> Handbooks { [ObservableAsProperty]get; }

        public extern bool IsDataUpdated { [ObservableAsProperty]get; }

        public ReactiveCommand<Unit> DoUpdate;
        
        public MainViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            DoUpdate = ReactiveCommand.CreateAsyncObservable(x => updateImpl());

            App.Store
                .DistinctUntilChanged(state => new { state.CurrentState })
                .Select(u => "Last Updated: " + u.CurrentState.LastUpdateTime.ToString("u"))
                .ToPropertyEx(
                    source: this,
                    property: x => x.LastUpdateTime,
                    scheduler: RxApp.MainThreadScheduler
                );

            App.Store
                .DistinctUntilChanged(state => new { state.CurrentState })
                .Select(u => u.CurrentState.IsLicensed)
                .ToPropertyEx(
                    source: this,
                    property: x => x.IsLicensed,
                    scheduler: RxApp.MainThreadScheduler
                );

            //App.Store
            //    .DistinctUntilChanged(state => new { state.CurrentState })
            //    .Select(u => u.CurrentState.LoggedIn)
            //    .ToPropertyEx(
            //        source: this,
            //        property: x => x.LoggedIn,
            //        scheduler: RxApp.MainThreadScheduler
            //    );

            App.Store
                .DistinctUntilChanged(state => new { state.CurrentState })
                .Select(u => u.CurrentState.IsUpdatingData)
                .ToPropertyEx(
                    source: this,
                    property: x => x.IsUpdatingData,
                    scheduler: RxApp.MainThreadScheduler
                );

            App.Store
                .DistinctUntilChanged(state => new { state.Books })
                .Select(d => d.Books.Values.OrderBy(y => y.OrderIndex).ToList())
                .ToPropertyEx(
                    source: this,
                    property: x => x.Handbooks,
                    scheduler: RxApp.MainThreadScheduler
                );

            App.Store
                .DistinctUntilChanged(state => new { state.CurrentState })
                .Select(u => u.CurrentState.IsDataUpdated)
                .ToPropertyEx(
                    source: this,
                    property: x => x.IsDataUpdated,
                    scheduler: RxApp.MainThreadScheduler
                );

            setupSubscriptions();
        }

        private void setupSubscriptions()
        {
            App.Store
                .DistinctUntilChanged(state => new { state.CurrentState })
                .Select(u => u.CurrentState.IsLoggedIn)
                .Subscribe(x => IsLoggedIn = x);
        }

        private IObservable<Unit> updateImpl()
        {
            App.Store.Dispatch(AzureActionCreators.ServerUpdateAction());
            return Observable.Start(() => { return Unit.Default; });
        }

        public void OpenThisBook(string url)
        {
            HostScreen.Router.Navigate.Execute(new BookpageViewModel(url, HostScreen));
        }
         
    }
}
