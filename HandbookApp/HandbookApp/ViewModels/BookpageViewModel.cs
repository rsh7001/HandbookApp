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
using Xamarin.Forms;


namespace HandbookApp.ViewModels
{
    [DataContract]
    public class BookpageViewModel : CustomBaseViewModel
    {
        [Reactive][DataMember] public WebViewSource PageSource { get; set; }

        [IgnoreDataMember] private IObservable<bool> reloaded;
        [IgnoreDataMember] private IObservable<bool> navigatetomain;

        [IgnoreDataMember]
        public ReactiveCommand<Unit> GoBack;

        [IgnoreDataMember]
        public ReactiveCommand<Unit> GoBookpage;


        public BookpageViewModel(string url, IScreen hostscreen = null) : base(hostscreen)
        {
            _viewModelName = this.GetType().ToString();
            
            GoBack = ReactiveCommand.CreateAsyncTask(x => goBackImpl());
            GoBookpage = ReactiveCommand.CreateAsyncTask(x => goBookpageImpl((string) x));

            this.Log().Info("{0}: {1}: Constructor", _viewModelName, url);

            if(url.StartsWith("http"))
            {
                var urlsource = new UrlWebViewSource();
                urlsource.Url = url;
                PageSource = urlsource;
                return;
            }

            if(App.Store.GetState().Fullpages.ContainsKey(url))
            {
                var fullpage = App.Store.GetState().Fullpages[url];
                PageSource = fullpage.Content;
                _urlPathSegment = fullpage.Title;
            }
            else
            {
                this.Log().Warn(string.Format("{0}: {1}: No Page",_viewModelName, url));
                _urlPathSegment = "No Page";
            }
        }

        private async Task goBackImpl()
        {
            int lastVmIndex = HostScreen.Router.NavigationStack.Count - 1;
            int backVmIndex = lastVmIndex - 1;
            IRoutableViewModel vm;

            if (backVmIndex <= 0)
            {
                vm = HostScreen.Router.NavigationStack.First();
            }
            else
            {
                vm = HostScreen.Router.NavigationStack[backVmIndex];
                HostScreen.Router.NavigationStack.RemoveAt(lastVmIndex);
                HostScreen.Router.NavigationStack.RemoveAt(backVmIndex);
            }
            await HostScreen.Router.NavigateBack.ExecuteAsyncTask(vm);
        }

        private async Task goBookpageImpl(string url)
        {
            var vm = new BookpageViewModel(url, HostScreen);
            await HostScreen.Router.Navigate.ExecuteAsyncTask(vm);
        }

        protected override void setupObservables()
        {
            base.setupObservables();

            reloaded = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.Reloaded })
                .Select(d => d.CurrentState.Reloaded);

            navigatetomain = navigatedaway
                .CombineLatest(navigating, (x, y) => !x && !y)
                .CombineLatest(reloaded, (a, b) => a && b);

        }

        protected override void setupSubscriptions()
        {
            base.setupSubscriptions();

            navigatetomain
                .Throttle(TimeSpan.FromMilliseconds(100))
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async x => {
                    if (x)
                    {
                        App.Store.Dispatch(new ClearReloadedAction());
                        await NavigateToMainAsync();
                    }
                })
                .DisposeWith(subscriptionDisposibles);
        }
    }
}
