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

using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using HandbookApp.Actions;
using HandbookApp.Reducers;
using HandbookApp.Services;
using HandbookApp.States;
using HandbookApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using Redux;
using Splat;
using Xamarin.Forms;


namespace HandbookApp
{
    public interface IAuthenticate
    {
        Task<bool> Authenticate(MobileServiceAuthenticationProvider provider);
    }


    public class App : Application, IEnableLogger
    {
        public static IStore<AppState>   Store         { get; private set; }
        public static IAuthenticate      Authenticator { get; private set; }

        public static AzureMobileService ServerService { get; private set; }
        

        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }


        public App()
        {
         
            ServerService = new AzureMobileService();

            OfflineService.Initialize();
            LogStoreService.InitializeLogStore();

            Locator.CurrentMutable.RegisterConstant(new LoggerService { Level = LogLevel.Debug }, typeof(ILogger));

            var initialState = OfflineService.LoadOfflineAppState();


            if(initialState.CurrentState.IsLoggedIn)
            {
                this.Log().Debug("InitialState.UserId: {0}", initialState.CurrentState.UserId);
                this.Log().Debug("InitialState.AuthToken: {0}", initialState.CurrentState.AuthToken);

                ServerService.SetAzureUserCredentials(initialState.CurrentState.UserId, initialState.CurrentState.AuthToken);
            }
            
            Store = new Store<AppState>(ApplicationReducers.ReduceApplication, initialState);
            App.Store.Dispatch(new SetNeedsUpdateAction());

            var bootstrapper = new AppBootstrapper();

            MainPage = bootstrapper.CreateMainPage();
        }

        protected override void OnStart()
        {
            this.Log().Info("OnStart()");
        }

        protected async override void OnSleep()
        {
            this.Log().Info("OnSleep()");

            await OfflineService.SaveAppState();

            this.Log().Info("Done()");
        }

        protected override void OnResume()
        {
            this.Log().Info("OnResume");
        }
    }
}
