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
using System.Text;
using System.Threading.Tasks;

namespace HandbookApp.States
{
    public class HandbookState
    {
        public bool OnLoginPage { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool CheckingLogin { get; set; }
        public bool IsUserSet { get; set; }
        public string UserId { get; set; }
        public string AuthToken { get; set; }
        
        public bool OnLicenceKeyPage { get; set; }
        public bool IsLicensed { get; set; }
        public bool IsLicenceKeySet { get; set; }
        public bool CheckingLicenceKey { get; set; }
        public string LicenceKey { get; set; }

        public bool IsUpdatingData { get; set; }
        public bool IsDataUpdated { get; set; }
        public bool IsDataLoaded { get; set; }
        public DateTimeOffset LastUpdateTime { get; set; }

        
        public HandbookState()
        {

        }

        protected HandbookState(HandbookState old)
        {
            this.OnLoginPage = old.OnLoginPage;
            this.IsLoggedIn = old.IsLoggedIn;
            this.CheckingLogin = old.CheckingLogin;
            this.IsUserSet = old.IsUserSet;
            this.UserId = "";
            if (!String.IsNullOrEmpty(old.UserId))
            {
                char[] buffer = new char[old.UserId.Length];
                old.UserId.CopyTo(0, buffer, 0, old.UserId.Length);
                this.UserId = new string(buffer);
            }
            this.AuthToken = "";
            if (!String.IsNullOrEmpty(old.AuthToken))
            {
                char[] buffer = new char[old.AuthToken.Length];
                old.AuthToken.CopyTo(0, buffer, 0, old.AuthToken.Length);
                this.AuthToken = new string(buffer);
            }

            this.OnLicenceKeyPage = old.OnLicenceKeyPage;
            this.IsLicensed = old.IsLicensed;
            this.IsLicenceKeySet = old.IsLicenceKeySet;
            this.CheckingLicenceKey = old.CheckingLicenceKey;
            this.LicenceKey = "";
            if (!String.IsNullOrEmpty(old.LicenceKey))
            {
                char[] buffer = new char[old.LicenceKey.Length];
                old.LicenceKey.CopyTo(0, buffer, 0, old.LicenceKey.Length);
                this.LicenceKey = new string(buffer);
            }

            this.IsUpdatingData = old.IsUpdatingData;
            this.IsDataUpdated = old.IsDataUpdated;
            this.IsDataLoaded = old.IsDataLoaded;
            this.LastUpdateTime = old.LastUpdateTime;
        }

        public HandbookState Clone()
        {
            return new HandbookState(this);
        }

        public override string ToString()
        {
            return "LoggedIn: " + IsLoggedIn.ToString() + "\nLicensed: " + IsLicensed.ToString();
        }
    }
}
