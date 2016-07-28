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

namespace HandbookApp.Models
{
    public class ServerExceptions
    {
        public class Unauthorized : Exception { }

        public class NetworkFailure : Exception { }

        public class ActionFailure : Exception { }

        public class UnknownFailure : Exception
        {
            public UnknownFailure(Exception ex) : base("Unknown Failure with Inner Exception", ex) { }
        }
    }
}
