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
using System.Threading.Tasks;
using HandbookApp.Actions;
using HandbookApp.Utilities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace HandbookApp.ViewModels
{
    public class LicenseKeyViewModel : ReactiveObject, IRoutableViewModel
    {
        [Reactive]public string PageTitle { get; set; }

        private IObservable<bool> cansetlicencekey;
        private IObservable<bool> islicencekeyset;

        public string UrlPathSegment { get { return "Licence Key"; } }
        public IScreen HostScreen { get; protected set; }

        [Reactive] public string LicenceKey { get; set; }

        public ReactiveCommand<Unit> SetLicensed;
        public ReactiveCommand<Unit> GoBack;


        public LicenseKeyViewModel(IScreen hostscreen = null)
        {
            HostScreen = hostscreen ?? Locator.Current.GetService<IScreen>();

            PageTitle = "License Key";

            setupObservables();

            SetLicensed = ReactiveCommand.CreateAsyncObservable(cansetlicencekey, x => setLicensedImpl());
            GoBack = ReactiveCommand.CreateAsyncTask(x => goBackImpl());

            islicencekeyset
                .Where(y => y == true)
                .DistinctUntilChanged()
                .InvokeCommand(this, x => x.GoBack);
        }

        private void setupObservables()
        {
            islicencekeyset = App.Store
                .DistinctUntilChanged(state => new { state.CurrentState.IsLicenceKeySet })
                .Select(d => d.CurrentState.IsLicenceKeySet);

            cansetlicencekey = this.WhenAnyValue(x => x.LicenceKey, (lk) =>
                !String.IsNullOrWhiteSpace(lk) && lk.Length >= 6)
                .DistinctUntilChanged();
        }

        private async Task goBackImpl()
        {
            var vm = HostScreen.Router.NavigationStack.First();
            HostScreen.Router.NavigationStack.Clear();
            await HostScreen.Router.NavigateAndReset.ExecuteAsyncTask(vm);
            App.Store.Dispatch(new ClearOnLicenceKeyAction());
        }
       
        private IObservable<Unit> setLicensedImpl()
        {
            App.Store.Dispatch(new SetLicenceKeyAction { LicenceKey = LicenceKey });
            return Observable.Start(() => { return Unit.Default; });
        }
    }
}
