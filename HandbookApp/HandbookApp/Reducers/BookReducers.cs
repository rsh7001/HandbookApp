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
using Redux;
using HandbookApp.States;
using HandbookApp.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using Splat;
using Newtonsoft.Json;

namespace HandbookApp.Reducers
{
    public static class BookReducers
    {

        public static ImmutableDictionary<string, Book> BookReducer(ImmutableDictionary<string, Book> previousState, IAction action)
        {
            if (action is AddBookRangeAction)
            {
                return addBookRangeReducer(previousState, (AddBookRangeAction)action);
            }

            if (action is DeleteBookRangeAction)
            {
                return deleteBookRangeReducer(previousState, (DeleteBookRangeAction) action);
            }
            return previousState;
        }

        private static ImmutableDictionary<string, Book> deleteBookRangeReducer(ImmutableDictionary<string, Book> previousState, DeleteBookRangeAction action)
        {
            LogHost.Default.Info("DeleteBookRangeAction: {0}", JsonConvert.SerializeObject(action.BookIds));
            return previousState.RemoveRange(action.BookIds);
        }

        private static ImmutableDictionary<string, Book> addBookRangeReducer(ImmutableDictionary<string, Book> previousState, AddBookRangeAction action)
        {
            if (action.Books.Count != 0)
            {
                var itemlist = action.Books
                    .Select(x => new KeyValuePair<string, Book>(x.Id, x));
                LogHost.Default.Info("AddBookRangeAction: {0}", JsonConvert.SerializeObject(action.Books.Select(x => x.Id).ToList()));
                return previousState.SetItems(itemlist);
            }

            return previousState;
        }
    }
}
