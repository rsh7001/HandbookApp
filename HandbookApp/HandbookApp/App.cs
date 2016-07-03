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
            var initialState = new AppState {
                Books = ImmutableDictionary<string, Book>.Empty,
                Fullpages = ImmutableDictionary<string, Fullpage>.Empty,
                CurrentState = new HandbookState {
                    OnLoginPage = false,
                    IsLoggedIn = false,
                    CheckingLogin = false,
                    IsUserSet = false,
                    UserId = null,
                    AuthToken = null,
                    
                    OnLicenceKeyPage = false,
                    IsLicensed = false,
                    IsLicenceKeySet = false,
                    CheckingLicenceKey = false,
                    LicenceKey = null,

                    IsUpdatingData = false,
                    IsDataUpdated = false,
                    IsDataLoaded = false,
                    LastUpdateTime = new System.DateTimeOffset(1970, 1, 1, 0, 0, 0, new System.TimeSpan(-5, 0, 0))
                    
                }
            };

            Store = new Store<AppState>(ApplicationReducers.ReduceApplication, initialState);

            var bootstrapper = new AppBootstrapper();

            var logger = new AppDebugger { Level = LogLevel.Debug };
            Locator.CurrentMutable.RegisterConstant(logger, typeof(ILogger));

            ServerService = new AzureMobileService();

            //var vm = new MainViewModel();
            //var view = new MainView();
            //view.ViewModel = vm;

            //MainPage = new NavigationPage(view);
            MainPage = bootstrapper.CreateMainPage();
            
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
