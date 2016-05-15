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
using HandbookApp.Views;
using ReactiveUI;
using ReactiveUI.XamForms;
using Splat;
using Xamarin.Forms;


namespace HandbookApp.ViewModels
{
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        public RoutingState Router { get; private set; }

        public AppBootstrapper(IMutableDependencyResolver dependencyResolver = null, RoutingState testRouter = null)
        {
            Router = testRouter ?? new RoutingState();
            dependencyResolver = dependencyResolver ?? Locator.CurrentMutable;

            RegisterParts(dependencyResolver);

            LogHost.Default.Level = LogLevel.Debug;

            Router.Navigate.Execute(new ReactiveMainViewModel(this));
        }

        private void RegisterParts(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));

            dependencyResolver.Register(() => new ReactiveMainPage(), typeof(IViewFor<ReactiveMainViewModel>));
            dependencyResolver.Register(() => new MainPage(), typeof(IViewFor<MainViewModel>));
            dependencyResolver.Register(() => new BookpagePage(), typeof(IViewFor<BookpageViewModel>));
            dependencyResolver.Register(() => new SettingsPage(), typeof(IViewFor<SettingsViewModel>));
        }

        public Page CreateMainPage()
        {
            return new RoutedViewHost();
        }
    }
}
