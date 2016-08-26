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
using System.Linq;
using HandbookApp.Actions;
using HandbookApp.Models.ServerUtility;
using HandbookApp.States;
using Newtonsoft.Json;
using Redux;
using Splat;

namespace HandbookApp.Reducers
{
    public static class PostUpdateStateReducers
    {

        public static PostUpdateState PostUpdateStateReducer(PostUpdateState previousState, IAction action)
        {
            if (action is AddPostUpdateAddBookIdsRangeAction)
            {
                return addPostUpdateAddBookIdsRangeReducer(previousState, (AddPostUpdateAddBookIdsRangeAction) action);
            }

            if (action is DeletePostUpdateAddBookIdsRangeAction)
            {
                return deletePostUpdateAddBookIdsRangeReducer(previousState, (DeletePostUpdateAddBookIdsRangeAction) action);
            }

            if (action is AddPostUpdateDeleteBookIdsRangeAction)
            {
                return addPostUpdateDeleteBookIdsRangeReducer(previousState, (AddPostUpdateDeleteBookIdsRangeAction)action);
            }

            if (action is DeletePostUpdateDeleteBookIdsRangeAction)
            {
                return deletePostUpdateDeleteBookIdsRangeReducer(previousState, (DeletePostUpdateDeleteBookIdsRangeAction)action);
            }


            if (action is AddPostUpdateAddFullpageIdsRangeAction)
            {
                return addPostUpdateAddFullpageIdsRangeReducer(previousState, (AddPostUpdateAddFullpageIdsRangeAction)action);
            }

            if (action is DeletePostUpdateAddFullpageIdsRangeAction)
            {
                return deletePostUpdateAddFullpageIdsRangeReducer(previousState, (DeletePostUpdateAddFullpageIdsRangeAction)action);
            }

            if (action is AddPostUpdateDeleteFullpageIdsRangeAction)
            {
                return addPostUpdateDeleteFullpageIdsRangeReducer(previousState, (AddPostUpdateDeleteFullpageIdsRangeAction)action);
            }

            if (action is DeletePostUpdateDeleteFullpageIdsRangeAction)
            {
                return deletePostUpdateDeleteFullpageIdsRangeReducer(previousState, (DeletePostUpdateDeleteFullpageIdsRangeAction)action);
            }


            if (action is RemoveLocalPostUpdatesDataAction)
            {
                return RemoveLocalPostUpdatesDataReducer(previousState, (RemoveLocalPostUpdatesDataAction) action);
            }

            return previousState;
        }

        private static PostUpdateState RemoveLocalPostUpdatesDataReducer(PostUpdateState previousState, RemoveLocalPostUpdatesDataAction action)
        {
            var ujm = new UpdateJsonMessage {
                AddBookItemIds = previousState.AddedBookIds.ToList(),
                DeleteBookItemIds = previousState.DeletedBooksIds.ToList(),
                AddFullpageItemIds = previousState.AddedFullpagesIds.ToList(),
                DeleteFullpageItemIds = previousState.DeletedFullpagesIds.ToList()
            };
            LogHost.Default.Info("RemoveLocalPostUpdateDataReducer: {0}", JsonConvert.SerializeObject(ujm));

            PostUpdateState newState = previousState.Clone();
            newState.AddedBookIds = previousState.AddedBookIds.RemoveRange(action.Data.AddBookItemIds);
            newState.DeletedBooksIds = previousState.DeletedBooksIds.RemoveRange(action.Data.DeleteBookItemIds);
            newState.AddedFullpagesIds = previousState.AddedFullpagesIds.RemoveRange(action.Data.AddFullpageItemIds);
            newState.DeletedFullpagesIds = previousState.DeletedFullpagesIds.RemoveRange(action.Data.DeleteFullpageItemIds);
            return newState;
        }

        private static PostUpdateState deletePostUpdateDeleteFullpageIdsRangeReducer(PostUpdateState previousState, DeletePostUpdateDeleteFullpageIdsRangeAction action)
        {
            LogHost.Default.Info("DeletePostUpdateDeleteFullpageIdsRangeReducer: {0}", JsonConvert.SerializeObject(action.FullpageIds));
            PostUpdateState newState = previousState.Clone();
            newState.DeletedFullpagesIds = previousState.DeletedFullpagesIds.RemoveRange(action.FullpageIds);
            return newState;
        }

        private static PostUpdateState addPostUpdateDeleteFullpageIdsRangeReducer(PostUpdateState previousState, AddPostUpdateDeleteFullpageIdsRangeAction action)
        {
            LogHost.Default.Info("AddPostUpdateDeleteFullpageIdsRangeReducer: {0}", JsonConvert.SerializeObject(action.FullpageIds));
            PostUpdateState newState = previousState.Clone();
            newState.DeletedFullpagesIds = previousState.DeletedFullpagesIds.AddRange(action.FullpageIds);
            return newState;
        }

        private static PostUpdateState deletePostUpdateAddFullpageIdsRangeReducer(PostUpdateState previousState, DeletePostUpdateAddFullpageIdsRangeAction action)
        {
            LogHost.Default.Info("DeletePostUpdateAddFullpageIdsRangeReducer: {0}", JsonConvert.SerializeObject(action.FullpageIds));
            PostUpdateState newState = previousState.Clone();
            newState.AddedFullpagesIds = previousState.AddedFullpagesIds.RemoveRange(action.FullpageIds);
            return newState;
        }

        private static PostUpdateState addPostUpdateAddFullpageIdsRangeReducer(PostUpdateState previousState, AddPostUpdateAddFullpageIdsRangeAction action)
        {
            LogHost.Default.Info("AddPostUpdateAddFullpageIdsRangeReducer: {0}", JsonConvert.SerializeObject(action.FullpageIds));
            PostUpdateState newState = previousState.Clone();
            newState.AddedFullpagesIds = previousState.AddedFullpagesIds.AddRange(action.FullpageIds);
            return newState;
        }

        private static PostUpdateState deletePostUpdateDeleteBookIdsRangeReducer(PostUpdateState previousState, DeletePostUpdateDeleteBookIdsRangeAction action)
        {
            LogHost.Default.Info("DeletePostUpdateDeleteBookIdsRangeReducer: {0}", JsonConvert.SerializeObject(action.BookIds));
            PostUpdateState newState = previousState.Clone();
            newState.DeletedBooksIds = previousState.DeletedBooksIds.RemoveRange(action.BookIds);
            return newState;
        }

        private static PostUpdateState addPostUpdateDeleteBookIdsRangeReducer(PostUpdateState previousState, AddPostUpdateDeleteBookIdsRangeAction action)
        {
            LogHost.Default.Info("AddPostUpdateDeleteBookIdsRangeReducer: {0}", JsonConvert.SerializeObject(action.BookIds));
            PostUpdateState newState = previousState.Clone();
            newState.DeletedBooksIds = previousState.DeletedBooksIds.AddRange(action.BookIds);
            return newState;
        }

        private static PostUpdateState deletePostUpdateAddBookIdsRangeReducer(PostUpdateState previousState, DeletePostUpdateAddBookIdsRangeAction action)
        {
            LogHost.Default.Info("DeletePostUpdateAddBookIdsRangeReducer: {0}", JsonConvert.SerializeObject(action.BookIds));
            PostUpdateState newState = previousState.Clone();
            newState.AddedBookIds = previousState.AddedBookIds.RemoveRange(action.BookIds);
            return newState;
        }

        private static PostUpdateState addPostUpdateAddBookIdsRangeReducer(PostUpdateState previousState, AddPostUpdateAddBookIdsRangeAction action)
        {
            LogHost.Default.Info("AddPostUpdateAddBookIdsRangeReducer: {0}", JsonConvert.SerializeObject(action.BookIds));
            PostUpdateState newState = previousState.Clone();
            newState.AddedBookIds = previousState.AddedBookIds.AddRange(action.BookIds);
            return newState;
        }
    }
}
