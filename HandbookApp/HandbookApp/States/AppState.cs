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
    public class AppState
    {
        public ImmutableDictionary<string, Book> Books { get; set;}
        public ImmutableDictionary<string, Fullpage> Fullpages { get; set; }
        public PostUpdateState CurrentPostUpdateState { get; set; }
        public HandbookState CurrentState { get; set; }

        public static AppState CreateEmpty()
        {
            return new AppState {
                Books = ImmutableDictionary<string, Book>.Empty,
                Fullpages = ImmutableDictionary<string, Fullpage>.Empty,
                CurrentPostUpdateState = PostUpdateState.CreateEmpty(),
                CurrentState = HandbookState.CreateEmpty()
            };
        }

        public static ImmutableDictionary<string, Book> CreateEmptyBooks()
        {
            return ImmutableDictionary<string, Book>.Empty;
        }

        public static ImmutableDictionary<string, Fullpage> CreateEmptyFullpages()
        {
            return ImmutableDictionary<string, Fullpage>.Empty;
        }
        
        public static PostUpdateState CreateEmptyPostUpdateState()
        {
            return PostUpdateState.CreateEmpty();
        }

        public static HandbookState CreateEmptyHandbookState()
        {
            return HandbookState.CreateEmpty();
        }
    }


}
