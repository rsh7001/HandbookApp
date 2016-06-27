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
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Xamarin.Forms;

namespace HandbookApp.ViewModels
{
    public class BookpageViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; protected set; }

        private string _urlPathSegment;
        public string UrlPathSegment
        {
            get { return _urlPathSegment; }
        }
        
        [Reactive] public WebViewSource PageSource { get; set; }

        public ReactiveCommand<Unit> GoBack;
        
        public BookpageViewModel(string url, IScreen hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();
            GoBack = HostScreen.Router.NavigateBack;

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
    }
}
