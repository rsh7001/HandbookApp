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
using HandbookApp.Models.ServerUtility;
using Redux;

namespace HandbookApp.Actions
{
    public class AddPostUpdateAddBookIdsRangeAction : IAction
    {
        public List<string> BookIds { get; set;}
    }

    public class DeletePostUpdateAddBookIdsRangeAction : IAction
    {
        public List<string> BookIds { get; set; }
    }

    public class AddPostUpdateDeleteBookIdsRangeAction : IAction
    {
        public List<string> BookIds { get; set; }
    }

    public class DeletePostUpdateDeleteBookIdsRangeAction : IAction
    {
        public List<string> BookIds { get; set; }
    }


    public class AddPostUpdateAddFullpageIdsRangeAction : IAction
    {
        public List<string> FullpageIds { get; set; }
    }

    public class DeletePostUpdateAddFullpageIdsRangeAction : IAction
    {
        public List<string> FullpageIds { get; set; }
    }

    public class AddPostUpdateDeleteFullpageIdsRangeAction : IAction
    {
        public List<string> FullpageIds { get; set; }
    }

    public class DeletePostUpdateDeleteFullpageIdsRangeAction : IAction
    {
        public List<string> FullpageIds { get; set; }
    }

    public class RemoveLocalPostUpdatesDataAction : IAction
    {
        public UpdateJsonMessage Data { get; set; }
    }
}
