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
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;

namespace HandbookApp.ViewModels
{
    public class MainViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; protected set; }

        public string UrlPathSegment
        {
            get
            {
                return "Main Page";
            }
        }

        public ReactiveCommand<Unit> GoBack;
        public ReactiveCommand<Object> GoCHONYHandbook;

        public MainViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            GoBack = HostScreen.Router.NavigateBack;

            GoCHONYHandbook = ReactiveCommand.CreateAsyncObservable(_ => HostScreen.Router.Navigate.ExecuteAsync(new BookpageViewModel("main_section", HostScreen)));
        }

        
    }
}
