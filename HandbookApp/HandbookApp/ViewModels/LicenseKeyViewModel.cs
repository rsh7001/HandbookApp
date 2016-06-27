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
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Xamarin.Forms;

namespace HandbookApp.ViewModels
{
    public class LicenseKeyViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; protected set; }
        
        [Reactive]public bool IsLoggedIn { get; set; }
        [Reactive]public bool IsLicenceKeySet { get; set; }
        [Reactive]public bool IsLicensed { get; set; }
        [Reactive]public bool CheckingLicenseKey { get; set; }

        public string UrlPathSegment
        {
            get { return "LicenseKey"; }
        }

        public LicenseKeyViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            SetupSubscriptions();

            System.Diagnostics.Debug.WriteLine(App.Store.GetState().CurrentState.ToString());
        }

        private void SetupSubscriptions()
        {
            App.Store
                .DistinctUntilChanged(state => new { state.CurrentState })
                .Select(q => q.CurrentState.IsLoggedIn)
                .Subscribe(x => IsLoggedIn = x);
            
            App.Store
                .DistinctUntilChanged(state => new { state.CurrentState })
                .Select(q => q.CurrentState.IsLicensed)
                .Subscribe(x => IsLicensed = x);

            App.Store
                .DistinctUntilChanged(state => new { state.CurrentState })
                .Select(q => q.CurrentState.IsLicenceKeySet)
                .Subscribe(x => IsLicenceKeySet = x);
        }
    }
}
