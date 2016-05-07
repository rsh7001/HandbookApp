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

namespace HandbookApp.Reducers
{
    public static class BookReducers
    {

        public static ImmutableList<Book> BookReducer(ImmutableList<Book> previousState, IAction action)
        {
            if (action is AddBookAction)
            {
                return AddBookReducer(previousState, (AddBookAction)action);
            }

            if (action is DeleteBookAction)
            {
                return DeleteBookReducer(previousState, (DeleteBookAction)action);
            }

            return previousState;
        }

        public static ImmutableList<Book> AddBookReducer(ImmutableList<Book> previousState, AddBookAction action)
        {
            var bookItem = new Book {
                Id = action.BookId,
                Title = action.Title,
                StartingBookpage = action.StartingBookpage,
                OrderIndex = action.OrderIndex
            };

            var index = previousState.IndexOf<Book>(bookItem);

            if(index == -1)
            {
                return previousState.Add(bookItem);
            }

            if(!previousState[index].Equals(bookItem))
            {
                return previousState.Replace(previousState[index], bookItem);
            }

            return previousState;
        }

        public static ImmutableList<Book> DeleteBookReducer(ImmutableList<Book> previousState, DeleteBookAction action)
        {
            return previousState.RemoveAll(x => x.Id == action.BookId);
        }
    }
}
