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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;
using HandbookApp.Models.ServerRequests;
using Splat;


namespace HandbookApp.Services
{
    public class LoggerService : ILogger
    {
        
        public LogLevel Level { get; set; }

        public void Write(string message, LogLevel logLevel)
        {
            if((int)logLevel < (int) Level)
                return;

            var dt = DateTimeOffset.UtcNow;

            Task.Run(() => {
            
                var item = new AppLogItemMessage {
                    LogDateTime = dt.ToString("O"),
                    LogName = logLevel.ToString(),
                    LogDataJson = message
                };

                LogStoreService.LogStore = LogStoreService.LogStore.Add(item);

            });

            Debug.WriteLine("{0}::{1:o}: {2}", LogStoreService.LogStore.Count, dt, message);

        }

    }
}
