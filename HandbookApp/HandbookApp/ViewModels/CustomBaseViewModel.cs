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
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;


namespace HandbookApp.ViewModels
{
    [DataContract]
    public abstract class CustomBaseViewModel : ReactiveObject, IRoutableViewModel, ICustomBaseViewModel, IEnableLogger
    {
        [DataMember]
        protected readonly CompositeDisposable subscriptionDisposibles = new CompositeDisposable();

        [IgnoreDataMember]
        protected string _urlPathSegment;
        [IgnoreDataMember]
        public string UrlPathSegment { get { return _urlPathSegment; } }

        [IgnoreDataMember]
        public IScreen HostScreen { get; protected set; }

        [DataMember]
        [Reactive]
        protected bool _navigating { get; set; }
        [DataMember]
        [Reactive]
        protected bool _navigatedAway { get; set; }

        [IgnoreDataMember]
        protected IObservable<bool> navigating;
        [IgnoreDataMember]
        protected IObservable<bool> navigatedaway;


        [IgnoreDataMember]
        protected string _viewModelName;


        protected CustomBaseViewModel(IScreen hostscreen = null)
        {
            HostScreen = hostscreen ?? Locator.Current.GetService<IScreen>();

            _navigatedAway = false;
            _navigating = false;

            _viewModelName = _viewModelName ?? "";
            _urlPathSegment = _urlPathSegment ?? "";

            setupObservables();
        }

        public virtual void OnAppearing()
        {
            this.Log().Info("{0}: {1}: OnAppearing", _viewModelName, _urlPathSegment);

            _navigating = true;
            _navigatedAway = false;

            setupSubscriptions();

            _navigating = false;
        }
        
        public virtual void OnDisappearing()
        {
            subscriptionDisposibles.Clear();
            this.Log().Info("{0}: {1}: OnDisappearing", _viewModelName, _urlPathSegment);
        }


        protected virtual void setupSubscriptions() { }


        protected virtual void setupObservables()
        {
            navigating = this.WhenAnyValue(x => x._navigating)
                .DistinctUntilChanged();

            navigatedaway = this.WhenAnyValue(x => x._navigatedAway)
                .DistinctUntilChanged();
        }


        protected virtual void NavigateTo<TViewModel>(params object[] args)
            where TViewModel : class
        {
            if (_navigating || _navigatedAway)
                return;

            _navigating = true;
            TViewModel vm = (TViewModel)Activator.CreateInstance(typeof(TViewModel), HostScreen, args);
            HostScreen.Router.Navigate.Execute(vm);
            _navigatedAway = true;
            _navigating = false;
        }


        protected virtual async Task NavigateToMainAsync()
        {
            if (_navigating || _navigatedAway)
            {
                return;
            }
            _navigating = true;
            var vm = HostScreen.Router.NavigationStack.First();
            HostScreen.Router.NavigationStack.Clear();
            await HostScreen.Router.NavigateAndReset.ExecuteAsyncTask(vm);
            _navigatedAway = true;
            _navigating = false;
        }
    }
}
