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
    public static class HandbookPageReducers
    {

        public static ImmutableDictionary<string, HandbookPage> AddHandbookPageReducer(ImmutableDictionary<string, HandbookPage> previousState, AddHandbookPageAction action)
        {
            var handbookPageItem = new HandbookPage {
                PageId = action.PageId,
                PageTitle = action.PageTitle,
                PageArticleId = action.PageArticleId,
                PageLinksTitle = action.PageLinksTitle,
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


        public static ImmutableDictionary<string, HandbookPage> DeleteHandbookPageReducer(ImmutableDictionary<string, HandbookPage> previousState, DeleteHandbookPageAction action)
        {
            if(previousState.ContainsKey(action.PageId))
            {
                return previousState.Remove(action.PageId);
            }

            return previousState;
        }


        public static ImmutableDictionary<string, HandbookPage> HandbookPageReducer(ImmutableDictionary<string, HandbookPage> previousState, IAction action)
        {
            if (action is AddHandbookPageAction)
            {
                return AddHandbookPageReducer(previousState, (AddHandbookPageAction)action);
            }

            if (action is DeleteHandbookPageAction)
            {
                return DeleteHandbookPageReducer(previousState, (DeleteHandbookPageAction)action);
            }

            return previousState;
        }
    }
}
