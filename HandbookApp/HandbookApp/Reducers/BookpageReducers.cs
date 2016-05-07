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

using HandbookApp.Actions;
using HandbookApp.States;


namespace HandbookApp.Reducers
{
    public static class BookpageReducers
    {

        public static ImmutableDictionary<string, Bookpage> AddBookpageReducer(ImmutableDictionary<string, Bookpage> previousState, AddBookpageAction action)
        {
            var handbookPageItem = new Bookpage {
                Id = action.PageId,
                Title = action.PageTitle,
                ArticleId = action.PageArticleId,
                LinksTitle = action.PageLinksTitle,
                Links = action.Links
            };

            if (!previousState.ContainsKey(action.PageId))
            {
                return previousState.Add(action.PageId, handbookPageItem);
            }

            if (!previousState[action.PageId].Equals(handbookPageItem))
            {
                return previousState.SetItem(action.PageId, handbookPageItem);
            }

            return previousState;
        }


        public static ImmutableDictionary<string, Bookpage> DeleteBookpageReducer(ImmutableDictionary<string, Bookpage> previousState, DeleteBookpageAction action)
        {
            if(previousState.ContainsKey(action.PageId))
            {
                return previousState.Remove(action.PageId);
            }

            return previousState;
        }


        public static ImmutableDictionary<string, Bookpage> HandbookPageReducer(ImmutableDictionary<string, Bookpage> previousState, IAction action)
        {
            if (action is AddBookpageAction)
            {
                return AddBookpageReducer(previousState, (AddBookpageAction)action);
            }

            if (action is DeleteBookpageAction)
            {
                return DeleteBookpageReducer(previousState, (DeleteBookpageAction)action);
            }

            return previousState;
        }
    }
}
