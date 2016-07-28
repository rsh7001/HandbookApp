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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HandbookApp.Actions;
using HandbookApp.States;
using Newtonsoft.Json;
using Redux;
using Splat;

namespace HandbookApp.Reducers
{
    public static class FullpageReducers
    {
        public static ImmutableDictionary<string, Fullpage> FullpageReducer(ImmutableDictionary<string, Fullpage> previousState, IAction action)
        {
            if (action is AddFullpageRangeAction)
            {
                return addFullpageRangeReducer(previousState, (AddFullpageRangeAction)action);
            }

            if (action is DeleteFullpageRangeAction)
            {
                return deleteFullpageRangeReducer(previousState, (DeleteFullpageRangeAction) action);
            }

            return previousState;
        }

        private static ImmutableDictionary<string, Fullpage> deleteFullpageRangeReducer(ImmutableDictionary<string, Fullpage> previousState, DeleteFullpageRangeAction action)
        {
            LogHost.Default.Info("DeleteFullpageRangeReducer: {0}", JsonConvert.SerializeObject(action.FullpageIds));
            return previousState.RemoveRange(action.FullpageIds);
        }

        private static ImmutableDictionary<string, Fullpage> addFullpageRangeReducer(ImmutableDictionary<string, Fullpage> previousState, AddFullpageRangeAction action)
        {
            if(action.Fullpages.Count != 0)
            {
                var itemlist = action.Fullpages
                    .Select(x => new KeyValuePair<string, Fullpage>(x.Id, x));
                LogHost.Default.Info("AddFullpageRangeReducer: {0}", JsonConvert.SerializeObject(action.Fullpages.Select(x => x.Id).ToList()));
                return previousState.SetItems(itemlist);
            }

            return previousState;
        }
    }
}
