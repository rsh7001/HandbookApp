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


namespace HandbookApp.States
{
    public class PostUpdateState
    {
        public ImmutableList<string> AddedBookIds { get; set; }
        public ImmutableList<string> DeletedBooksIds { get; set; }
        public ImmutableList<string> AddedFullpagesIds { get; set; }
        public ImmutableList<string> DeletedFullpagesIds { get; set; }

        public PostUpdateState() { }

        protected PostUpdateState(PostUpdateState old)
        {
            this.AddedBookIds = old.AddedBookIds;
            this.DeletedBooksIds = old.DeletedBooksIds;
            this.AddedFullpagesIds = old.AddedFullpagesIds;
            this.DeletedFullpagesIds = old.DeletedFullpagesIds;
        }


        public PostUpdateState Clone()
        {
            return new PostUpdateState(this);
        }


        public static PostUpdateState CreateEmpty()
        {
            return new PostUpdateState {
                AddedBookIds = ImmutableList<string>.Empty,
                DeletedBooksIds = ImmutableList<string>.Empty,
                AddedFullpagesIds = ImmutableList<string>.Empty,
                DeletedFullpagesIds = ImmutableList<string>.Empty
            };
        }

        public static ImmutableList<string> CreateEmptyIds()
        {
            return ImmutableList<string>.Empty;
        }
    }


}
