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
using System.Reactive;
using System.Reactive.Linq;
using HandbookApp.Services;
using HandbookApp.States;
using ReactiveUI;
using Splat;


namespace HandbookApp.ViewModels
{

    public class ReactiveMainViewModel : ReactiveObject, IRoutableViewModel
    {

        private string _numBooks;
        public string NumBooks
        {
            get { return _numBooks; }
            set { this.RaiseAndSetIfChanged(ref _numBooks, value); }
        }

        public string UrlPathSegment
        {
            get
            {
                return "Reactive Main Page";
            }
        }

        public IScreen HostScreen { get; protected set; }
        
        public ReactiveCommand<Unit> Update;
        public ReactiveCommand<Object> GoMainPage;
        
        public ReactiveMainViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            Update = ReactiveCommand.CreateAsyncObservable(x => updateImpl());
  
            GoMainPage = ReactiveCommand.CreateAsyncObservable(_ => HostScreen.Router.Navigate.ExecuteAsync(new MainViewModel(HostScreen)));

            App.Store
                .DistinctUntilChanged(state => new { state.Books })
                .Subscribe(state => setNumBooks(state));
        }

        private void setNumBooks(AppState state)
        {
            NumBooks = state.Books.Count.ToString();
        }
        
        private IObservable<Unit> updateImpl()
        {
            JsonServerService.JsonServerUpdate();
            return Observable.Start(() => { return Unit.Default; });
        }

    }
}
