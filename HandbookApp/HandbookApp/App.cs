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

using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Collections.Immutable;
using HandbookApp.Reducers;
using HandbookApp.States;
using HandbookApp.ViewModels;
using Redux;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using HandbookApp.Services;
using HandbookApp.Views;
using Splat;
using Akavache;
using System;

namespace HandbookApp
{
    public interface IAuthenticate
    {
        Task<bool> Authenticate(MobileServiceAuthenticationProvider provider);
    }


    public class App : Application
    {
        public static IStore<AppState> Store { get; private set; }
        public static IAuthenticate Authenticator { get; private set; }
        
        public static AzureMobileService ServerService { get; private set; }
        
        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }

        public App()
        {
            BlobCache.ApplicationName = "AppStateExperiment2";

            var logger = new AppDebugger { Level = LogLevel.Debug };
            Locator.CurrentMutable.RegisterConstant(logger, typeof(ILogger));

            ServerService = new AzureMobileService();

            var initialCurrentState = BlobCache.UserAccount.GetObject<HandbookState>("currentstate").Catch(Observable.Return(getInitialCurrentState())).Wait();
            var initialBooks = BlobCache.UserAccount.GetObject<ImmutableDictionary<string, Book>>("books").Catch(Observable.Return(getInitialBooks())).Wait();
            var initialFullpages = BlobCache.UserAccount.GetObject<ImmutableDictionary<string, Fullpage>>("fullpages").Catch(Observable.Return(getInitialFullpages())).Wait();

            var initialState = new AppState {
                Books = initialBooks,
                Fullpages = initialFullpages,
                CurrentState = initialCurrentState
            };
            
            LogHost.Default.Info("Before Initialization of Store");

            Store = new Store<AppState>(ApplicationReducers.ReduceApplication, initialState);

            LogHost.Default.Info("After Initialization of Store");

            var bootstrapper = new AppBootstrapper();

            MainPage = bootstrapper.CreateMainPage();
            
        }

        private ImmutableDictionary<string, Fullpage> getInitialFullpages()
        {
            return ImmutableDictionary<string, Fullpage>.Empty;
        }

        private ImmutableDictionary<string, Book> getInitialBooks()
        {
            return ImmutableDictionary<string, Book>.Empty;
        }

        private HandbookState getInitialCurrentState()
        {
            return new HandbookState {
                OnLoginPage = false,
                IsLoggedIn = false,
                CheckingLogin = false,
                IsUserSet = false,
                UserId = "",
                AuthToken = "",

                OnLicenceKeyPage = false,
                IsLicensed = false,
                IsLicenceKeySet = false,
                CheckingLicenceKey = false,
                LicenceKey = "",

                IsUpdatingData = false,
                IsDataUpdated = false,
                IsDataLoaded = false,
                LastUpdateTime = new System.DateTimeOffset(1970, 1, 1, 0, 0, 0, new System.TimeSpan(-5, 0, 0))

            };
        }

        private AppState getInitialAppState()
        {
            var initialState = new AppState {
                Books = getInitialBooks(),
                Fullpages = getInitialFullpages(),
                CurrentState = getInitialCurrentState()
                };
            return initialState;
        }

        protected override void OnStart()
        {
            LogHost.Default.Info("OnStart()");

            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            LogHost.Default.Info("OnSleep()");
            var current = Store.GetState().CurrentState.Clone();
            BlobCache.UserAccount.InsertObject("currentstate", current).Wait();
            BlobCache.UserAccount.InsertObject("books", Store.GetState().Books).Wait();
            BlobCache.UserAccount.InsertObject("fullpages", Store.GetState().Fullpages).Wait();
            // This was causing problems because it shutdown all of BlobCache //BlobCache.Shutdown().Wait();
            LogHost.Default.Info("Finished Writing to BlobCache");
            LogHost.Default.Info("BlobCache Shutdown");
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            LogHost.Default.Info("OnResume");
            //var initialState = BlobCache.UserAccount.GetObject<AppState>("appstate").Catch(Observable.Return(getInitialAppState())).Wait();
            //Store = new Store<AppState>(ApplicationReducers.ReduceApplication, initialState);
            //LogHost.Default.Info("Reloaded");
            // Handle when your app resumes
        }
    }
}
