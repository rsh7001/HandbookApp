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
using System.Threading.Tasks;
using Redux;


namespace HandbookApp.Utilities
{
    public delegate Task AsyncActionsCreator<TState>(Dispatcher dispatcher, Func<TState> getState);

    public static class StoreExtensions
    {
        /// <summary>
        /// Extension on IStore to dispatch multiple actions via a thunk (from GuillaumeSalles/redux.NET/examples/async/Redux.Async example)
        /// </summary>
        public static Task Dispatch<TState>(this IStore<TState> store, AsyncActionsCreator<TState> actionsCreator)
        {
            return actionsCreator(store.Dispatch, store.GetState);
        }
    }
}
