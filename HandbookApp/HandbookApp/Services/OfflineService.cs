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
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Akavache;
using HandbookApp.Models.ServerRequests;
using HandbookApp.States;
using Splat;

namespace HandbookApp.Services
{
    public static class OfflineService
    {
        static bool initialized = false;
        
        public static void Initialize()
        {
            BlobCache.ApplicationName = "HandbookApp";
            initialized = true;
        }

        public static ImmutableList<AppLogItemMessage> LoadOfflineLogStore()
        {
            if (!initialized)
                Initialize();

            return BlobCache.UserAccount.GetObject<ImmutableList<AppLogItemMessage>>("logstore").Catch(Observable.Return(ImmutableList<AppLogItemMessage>.Empty)).Wait();
        }

        public static void SaveLogStore()
        {
            if(!initialized)
                Initialize();

            var logstores = LogStoreService.LogStore;
            BlobCache.UserAccount.InsertObject("logstore", logstores).Wait();
        }

        public static AppState LoadOfflineAppState()
        {
            if (!initialized)
                Initialize();
            

            var initialCurrentState = BlobCache.UserAccount.GetObject<HandbookState>("currentstate").Catch(Observable.Return(AppState.CreateEmptyHandbookState())).Wait();
            var initialBooks = BlobCache.UserAccount.GetObject<ImmutableDictionary<string, Book>>("books").Catch(Observable.Return(AppState.CreateEmptyBooks())).Wait();
            var initialFullpages = BlobCache.UserAccount.GetObject<ImmutableDictionary<string, Fullpage>>("fullpages").Catch(Observable.Return(AppState.CreateEmptyFullpages())).Wait();
            var initialCurrentPostUpdateState = BlobCache.UserAccount.GetObject<PostUpdateState>("currentpostupdatestate").Catch(Observable.Return(AppState.CreateEmptyPostUpdateState())).Wait();

            initialCurrentState.CheckingLogin = false;
            initialCurrentState.HasLicensedError = false;
            initialCurrentState.HasUnauthorizedError = false;
            initialCurrentState.PostingUpdateData = false;
            initialCurrentState.RefreshingToken = false;
            initialCurrentState.UpdatingData = false;
            initialCurrentState.VerifyingLicenceKey = false;

            var initialState = new AppState {
                Books = initialBooks,
                Fullpages = initialFullpages,
                CurrentPostUpdateState = initialCurrentPostUpdateState,
                CurrentState = initialCurrentState
            };

            LogHost.Default.Info("OfflineService: LoadOfflineAppState: Loaded");

            return initialState;
        }

        public async static Task SaveAppState()
        {
            if(!initialized)
                Initialize();

            var currentHandbookState = App.Store.GetState().CurrentState.Clone();
            var currentPostUpdateState = App.Store.GetState().CurrentPostUpdateState.Clone();
            var books = App.Store.GetState().Books;
            var fullpages = App.Store.GetState().Fullpages;

            LogHost.Default.Info("OfflineService: SaveAppState: Start Save AppState");
            await Task.Run(() => {
                BlobCache.UserAccount.InsertObject("currentpostupdatestate", currentPostUpdateState).Wait();
                BlobCache.UserAccount.InsertObject("currentstate", currentHandbookState).Wait();
                BlobCache.UserAccount.InsertObject("books", books).Wait();
                BlobCache.UserAccount.InsertObject("fullpages", fullpages).Wait();
                BlobCache.UserAccount.Flush().Wait();
            });

            LogHost.Default.Info("OfflineService: SaveAppState: Done Save AppState");

            SaveLogStore();
        }
    }
}
