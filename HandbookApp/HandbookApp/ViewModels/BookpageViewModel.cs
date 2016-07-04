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
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Xamarin.Forms;

namespace HandbookApp.ViewModels
{
    public class BookpageViewModel : ReactiveObject, IRoutableViewModel
    {
        [Reactive] public WebViewSource PageSource { get; set; }

        private string _urlPathSegment;
        public string UrlPathSegment
        {
            get { return _urlPathSegment; }
        }

        public IScreen HostScreen
        {
            get; protected set;
        }

        public ReactiveCommand<Unit> GoBack;
        public ReactiveCommand<Unit> GoBookpage;

        public BookpageViewModel(string url, IScreen hostscreen = null)
        {
            HostScreen = hostscreen ?? Locator.Current.GetService<IScreen>();
            
            GoBack = ReactiveCommand.CreateAsyncTask(x => goBackImpl());
            GoBookpage = ReactiveCommand.CreateAsyncTask(x => goBookpage((string) x));

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

        private async Task goBookpage(string url)
        {
            var vm = new BookpageViewModel(url, HostScreen);
            await HostScreen.Router.Navigate.ExecuteAsyncTask(vm);
        }
    }
}
