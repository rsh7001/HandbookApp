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
using System.Linq;

namespace HandbookApp.States
{
    public class HandbookPage : IEquatable<HandbookPage>
    {
        public string PageId { get; set; }
        public string PageTitle { get; set; }
        public string PageArticleId { get; set; }
        public string PageLinksTitle { get; set; }
        public List<string> Links { get; set; }

        public bool Equals(HandbookPage a)
        {
            if(a == null)
            {
                return false;
            }
            return (PageId == a.PageId) && (PageTitle == a.PageTitle) && (PageArticleId == a.PageArticleId) && (Links.SequenceEqual(a.Links));

        }
    }
}
