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


namespace HandbookApp.States
{
    public class HandbookState
    {
        public bool IsDataUpdated { get; set; }
        public DateTimeOffset LastUpdateTime { get; set; }

        public bool IsLicensed { get; set; }

        public bool IsLoggedIn { get; set; }

        public bool IsLicenceKeySet { get; set; }
        public string LicenceKey { get; set; }

        public bool IsUserSet { get; set; }
        public string UserId { get; set; }
        public string AuthToken { get; set; }

        public bool HasLicensedError { get; set; }
        public bool HasUnauthorizedError { get; set; }

        public bool CheckingLogin { get; set; }
        public bool VerifyingLicenceKey { get; set; }
        public bool UpdatingData { get; set; }
        public bool PostingUpdateData { get; set; }
        public bool RefreshingToken { get; set; }
        public bool LoadingAppLog { get; set; }

        public bool ResettingUpdates { get; set; }

        public bool Reloaded { get; set; }
        
        
        public HandbookState()
        {

        }

        protected HandbookState(HandbookState old)
        {
            this.IsDataUpdated = old.IsDataUpdated;
            this.LastUpdateTime = old.LastUpdateTime;

            this.IsLicensed = old.IsLicensed;

            this.IsLoggedIn = old.IsLoggedIn;

            this.IsLicenceKeySet = old.IsLicenceKeySet;
            this.LicenceKey = "";
            if (!String.IsNullOrEmpty(old.LicenceKey))
            {
                char[] buffer = new char[old.LicenceKey.Length];
                old.LicenceKey.CopyTo(0, buffer, 0, old.LicenceKey.Length);
                this.LicenceKey = new string(buffer);
            }

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

            this.HasLicensedError = old.HasLicensedError;
            this.HasUnauthorizedError = old.HasUnauthorizedError;

            this.CheckingLogin = old.CheckingLogin;
            this.VerifyingLicenceKey = old.VerifyingLicenceKey;
            this.UpdatingData = old.UpdatingData;
            this.PostingUpdateData = old.PostingUpdateData;
            this.RefreshingToken = old.RefreshingToken;
            this.LoadingAppLog = old.LoadingAppLog;

            this.ResettingUpdates = old.ResettingUpdates;

            this.Reloaded = old.Reloaded;
        }


        public HandbookState Clone()
        {
            return new HandbookState(this);
        }


        public static HandbookState CreateEmpty()
        {
            return new HandbookState {
                IsDataUpdated = false,
                LastUpdateTime = new System.DateTimeOffset(1970, 1, 1, 0, 0, 0, new System.TimeSpan(-5, 0, 0)),

                IsLicensed = false,

                IsLoggedIn = false,

                IsLicenceKeySet = false,
                LicenceKey = "",

                IsUserSet = false,
                UserId = "",
                AuthToken = "",

                HasLicensedError = false,
                HasUnauthorizedError = false,

                CheckingLogin = false,
                VerifyingLicenceKey = false,
                UpdatingData = false,
                PostingUpdateData = false,
                RefreshingToken = false,
                LoadingAppLog = false,

                ResettingUpdates = false,

                Reloaded = false
            };
        }
    }
}
