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

namespace HandbookApp.Reducers
{
    public static class BookReducers
    {

        public static ImmutableDictionary<string, Book> BookReducer(ImmutableDictionary<string, Book> previousState, IAction action)
        {
            if (action is AddBookAction)
            {
                return AddBookReducer(previousState, (AddBookAction)action);
            }

            if (action is DeleteBookAction)
            {
                return DeleteBookReducer(previousState, (DeleteBookAction)action);
            }

            if (action is AddBookRangeAction)
            {
                return AddBookRangeReducer(previousState, (AddBookRangeAction)action);
            }

            return previousState;
        }

        private static ImmutableDictionary<string, Book> AddBookRangeReducer(ImmutableDictionary<string, Book> previousState, AddBookRangeAction action)
        {
            if (action.Books.Count != 0)
            {
                var itemlist = action.Books
                    .Select(x => new KeyValuePair<string, Book>(x.Id, x));
                return previousState.SetItems(itemlist);
            }

            return previousState;
        }

        public static ImmutableDictionary<string, Book> AddBookReducer(ImmutableDictionary<string, Book> previousState, AddBookAction action)
        {
            var bookItem = new Book {
                Id = action.BookId,
                Title = action.Title,
                StartingBookpage = action.StartingBookpage,
                OrderIndex = action.OrderIndex
            };

            if(!previousState.ContainsKey(action.BookId))
            {
                return previousState.Add(action.BookId, bookItem);
            }

            if(!previousState[action.BookId].Equals(bookItem))
            {
                return previousState.SetItem(action.BookId, bookItem);
            }

            return previousState;
        }

        public static ImmutableDictionary<string, Book> DeleteBookReducer(ImmutableDictionary<string, Book> previousState, DeleteBookAction action)
        {
            if(previousState.ContainsKey(action.BookId))
            {
                return previousState.Remove(action.BookId);
            }

            return previousState;
        }
    }
}
