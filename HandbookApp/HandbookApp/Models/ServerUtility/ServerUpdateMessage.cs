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
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HandbookApp.Models.ServerUtility
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ServerUpdateMessage
    {
        public const string AddFullpageActionId = "AddFullpageAction";
        public const string DeleteFullpageActionId = "DeleteFullpageAction";
        public const string AddBookActionId = "AddBookAction";
        public const string DeleteBookActionId = "DeleteBookAction";

        [JsonProperty]
        public int Id { get; set; }

        public DateTime Time { get; set; }

        [JsonProperty]
        public string Action { get; set; }

        [JsonProperty]
        public string ArticleID { get; set; }

        [JsonProperty]
        public string ArticleTitle { get; set; }

        [JsonProperty]
        public string ArticleContent { get; set; }

        [JsonProperty]
        public string BookpageID { get; set; }

        [JsonProperty]
        public string BookpageArticleID { get; set; }

        [JsonProperty]
        public string BookpageLinkTitle { get; set; }

        [JsonProperty]
        public List<string> BookpageLinkIDs { get; set; }

        [JsonProperty]
        public string BookID { get; set; }

        [JsonProperty]
        public string BookTitle { get; set; }

        [JsonProperty]
        public string BookStartingID { get; set; }

        [JsonProperty]
        public int BookOrder { get; set; }

        [JsonProperty]
        public string FullPageID { get; set; }

        [JsonProperty]
        public string FullPageContent { get; set; }

        [JsonProperty]
        public string FullPageTitle { get; set; }
    }
}
