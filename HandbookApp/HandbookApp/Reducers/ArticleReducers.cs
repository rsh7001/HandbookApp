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

    public static class ArticleReducers
    {
        
        public static ImmutableDictionary<string, Article> AddArticleReducer(ImmutableDictionary<string, Article> previousState, AddArticleAction action)
        {
            var articleItem = new Article {
                ArticleId = action.ArticleId,
                Title = action.Title,
                Content = action.Content
            };

            if (!previousState.ContainsKey(action.ArticleId))
            {
                return previousState.Add(action.ArticleId, articleItem);
            }

            if(!previousState[action.ArticleId].Equals(articleItem))
            {
                return previousState.SetItem(action.ArticleId, articleItem);
            }
                
            return previousState;
        }


        public static ImmutableDictionary<string, Article> DeleteArticleReducer(ImmutableDictionary<string, Article> previousState, DeleteArticleAction action)
        {
            if(previousState.ContainsKey(action.ArticleId))
            {
                return previousState.Remove(action.ArticleId);
            }
            
            return previousState;
        }


        public static ImmutableDictionary<string, Article> ArticleReducer(ImmutableDictionary<string, Article> previousState, IAction action)
        {
            if (action is AddArticleAction)
            {
                return AddArticleReducer(previousState, (AddArticleAction)action);
            }

            if (action is DeleteArticleAction)
            {
                return DeleteArticleReducer(previousState, (DeleteArticleAction)action);
            }

            return previousState;
        }

    }
}
