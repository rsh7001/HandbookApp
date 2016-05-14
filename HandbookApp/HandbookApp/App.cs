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
using HandbookApp.Reducers;
using HandbookApp.States;
using HandbookApp.ViewModels;
using Redux;
using Xamarin.Forms;

namespace HandbookApp
{
    public class App : Application
    {
        public static IStore<AppState> Store { get; private set; }
        
        public App()
        {
            var initialState = new AppState {
                Books = ImmutableDictionary<string, Book>.Empty,
                Fullpages = ImmutableDictionary<string, Fullpage>.Empty
            };

            Store = new Store<AppState>(ApplicationReducers.ReduceApplication, initialState);

            var bootstrapper = new AppBootstrapper();

            var mainPage = bootstrapper.CreateMainPage();

            MainPage = mainPage;
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
