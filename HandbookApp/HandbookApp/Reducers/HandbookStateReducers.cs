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
using HandbookApp.States;
using HandbookApp.Actions;
using Redux;
using Xamarin.Forms;
using Splat;

namespace HandbookApp.Reducers
{
    public static class HandbookStateReducers
    {
        public static HandbookState HandbookStateReducer(HandbookState previousState, IAction action)
        {
            if (action is SetOnLoginPageAction)
            {
                return setOnLoginPageReducer(previousState, (SetOnLoginPageAction) action);
            }

            if (action is ClearOnLoginPageAction)
            {
                return clearOnLoginPageReducer(previousState, (ClearOnLoginPageAction) action);
            }

            if (action is LoginAction)
            {
                return loginReducer(previousState, (LoginAction) action);
            }

            if (action is LogoutAction)
            {
                return logoutReducer(previousState, (LogoutAction) action);
            }

            if (action is SetCheckingLoginAction)
            {
                return setCheckingLoginReducer(previousState, (SetCheckingLoginAction) action);
            }

            if (action is SetOnLicenceKeyAction)
            {
                return setOnLicenceKeyReducer(previousState, (SetOnLicenceKeyAction) action);
            }

            if (action is ClearOnLicenceKeyAction)
            {
                return clearOnLicenceKeyReducer(previousState, (ClearOnLicenceKeyAction) action);
            }

            if (action is SetLicenceKeyAction)
            {
                return setLicenceKeyReducer(previousState, (SetLicenceKeyAction) action);
            }

            if (action is ClearLicenceKeyAction)
            {
                return clearLicenceKeyReducer(previousState, (ClearLicenceKeyAction) action);
            }

            if (action is CheckingLicenceKeyAction)
            {
                return checkingLicenceKeyReducer(previousState, (CheckingLicenceKeyAction) action);
            }

            if (action is CancelCheckingLicenceKeyAction)
            {
                return cancelCheckingLicenceKeyReducer(previousState, (CancelCheckingLicenceKeyAction) action);
            }


            if (action is SetLicensedAction)
            {
                return setLicensedReducer(previousState, (SetLicensedAction) action);
            }

            if (action is ClearLicensedAction)
            {
                return clearLicensedReducer(previousState, (ClearLicensedAction) action);
            }


            if (action is SetUpdatingDataAction)
            {
                return setUpdatingDataReducer(previousState, (SetUpdatingDataAction) action);
            }

            if (action is ClearUpdatingDataAction)
            {
                return clearUpdatingDataReducer(previousState, (ClearUpdatingDataAction) action);
            }

            if (action is SetLastUpdateTimeAction)
            {
                return setLastUpdateTimeReducer(previousState, (SetLastUpdateTimeAction) action);
            }

            return previousState;
        }

        private static HandbookState setOnLicenceKeyReducer(HandbookState previousState, SetOnLicenceKeyAction action)
        {
            LogHost.Default.Info("setOnLicenceKeyReducer");
            HandbookState newState = previousState.Clone();
            LogHost.Default.Info("setOnLicenceKeyReducer2");
            newState.OnLicenceKeyPage = true;
            LogHost.Default.Info("setOnLicenceKeyReducer3");
            return newState;
        }

        private static HandbookState clearOnLicenceKeyReducer(HandbookState previousState, ClearOnLicenceKeyAction action)
        {
            LogHost.Default.Info("clearOnLicenceKeyReducer");
            HandbookState newState = previousState.Clone();
            newState.OnLicenceKeyPage = false;
            return newState;
        }

        private static HandbookState clearOnLoginPageReducer(HandbookState previousState, ClearOnLoginPageAction action)
        {
            LogHost.Default.Info("clearOnLoginPageReducer");
            HandbookState newState = previousState.Clone();
            newState.OnLoginPage = false;
            return newState;
        }

        private static HandbookState setOnLoginPageReducer(HandbookState previousState, SetOnLoginPageAction action)
        {
            LogHost.Default.Info("setOnLoginPageReducer");
            HandbookState newState = previousState.Clone();
            newState.OnLoginPage = true;
            return newState;
        }

        private static HandbookState setLastUpdateTimeReducer(HandbookState previousState, SetLastUpdateTimeAction action)
        {
            LogHost.Default.Info("setLastUpdateTimeReducer");
            HandbookState newState = previousState.Clone();
            newState.IsUpdatingData = false;
            newState.IsDataUpdated = true;
            newState.IsDataLoaded = true;
            newState.LastUpdateTime = action.UpdateTime;
            return newState;
        }

        private static HandbookState clearUpdatingDataReducer(HandbookState previousState, ClearUpdatingDataAction action)
        {
            LogHost.Default.Info("clearUpdatingDataReducer");
            HandbookState newState = previousState.Clone();
            newState.IsUpdatingData = false;
            return newState;
        }

        private static HandbookState setUpdatingDataReducer(HandbookState previousState, SetUpdatingDataAction action)
        {
            LogHost.Default.Info("setUpdatingDataReducer");
            HandbookState newState = previousState.Clone();
            newState.IsUpdatingData = true;
            return newState;
        }


        private static HandbookState clearLicensedReducer(HandbookState previousState, ClearLicensedAction action)
        {
            LogHost.Default.Info("clearLicnsedReducer");
            HandbookState newState = previousState.Clone();
            newState.IsLicensed = false;
            return newState;
        }

        private static HandbookState setLicensedReducer(HandbookState previousState, SetLicensedAction action)
        {
            LogHost.Default.Info("setLicensedReducer");
            HandbookState newState = previousState.Clone();
            newState.CheckingLicenceKey = false;
            newState.IsLicensed = true;
            return newState;
        }


        private static HandbookState cancelCheckingLicenceKeyReducer(HandbookState previousState, CancelCheckingLicenceKeyAction action)
        {
            LogHost.Default.Info("cancelCheckingLicenceKeyReducer");
            HandbookState newState = previousState.Clone();
            newState.CheckingLicenceKey = false;
            return newState;
        }

        private static HandbookState checkingLicenceKeyReducer(HandbookState previousState, CheckingLicenceKeyAction action)
        {
            LogHost.Default.Info("checkingLicenceKeyReducer");
            HandbookState newState = previousState.Clone();
            newState.CheckingLicenceKey = true;
            return newState;
        }

        private static HandbookState clearLicenceKeyReducer(HandbookState previousState, ClearLicenceKeyAction action)
        {
            LogHost.Default.Info("clearLicenceKeyReducer");
            HandbookState newState = previousState.Clone();
            newState.LicenceKey = null;
            newState.IsLicenceKeySet = false;
            newState.IsLicensed = false;
            newState.CheckingLicenceKey = false;
            return newState;
        }

        private static HandbookState setLicenceKeyReducer(HandbookState previousState, SetLicenceKeyAction action)
        {
            LogHost.Default.Info("setLicenceKeyReducer: {0}", action.LicenceKey);
            HandbookState newState = previousState.Clone();
            newState.LicenceKey = null;
            if (action.LicenceKey != null)
            {
                char[] buffer = new char[action.LicenceKey.Length];
                action.LicenceKey.CopyTo(0, buffer, 0, action.LicenceKey.Length);
                newState.LicenceKey = new string(buffer);
            }
            newState.IsLicenceKeySet = true;
            return newState;
        }


        private static HandbookState setCheckingLoginReducer(HandbookState previousState, SetCheckingLoginAction action)
        {
            HandbookState newState = previousState.Clone();
            newState.CheckingLogin = true;
            return newState;
        }

        private static HandbookState logoutReducer(HandbookState previousState, LogoutAction action)
        {
            LogHost.Default.Info("logoutReducer");
            HandbookState newState = previousState.Clone();
            newState.UserId = null;
            newState.AuthToken = null;

            newState.IsUserSet = false;
            newState.IsLoggedIn = false;
            newState.CheckingLogin = false;

            newState.IsDataUpdated = false;
            newState.IsUpdatingData = false;

            newState.IsLicensed = false;
            newState.CheckingLicenceKey = false;
            return newState;
        }

        private static HandbookState loginReducer(HandbookState previousState, LoginAction action)
        {
            LogHost.Default.Info("loginReducer");
            HandbookState newState = previousState.Clone();
            newState.UserId = null;
            if (action.UserId != null)
            {
                char[] buffer = new char[action.UserId.Length];
                action.UserId.CopyTo(0, buffer, 0, action.UserId.Length);
                newState.UserId = new string(buffer);
            }
            newState.AuthToken = null;
            if (action.AuthToken != null)
            {
                char[] buffer = new char[action.AuthToken.Length];
                action.AuthToken.CopyTo(0, buffer, 0, action.AuthToken.Length);
                newState.AuthToken = new string(buffer);
            }

            newState.CheckingLogin = false;
            newState.IsUserSet = true;
            newState.IsLoggedIn = true;

            newState.IsDataUpdated = false;
            newState.IsUpdatingData = false;

            newState.IsLicensed = false;
            newState.CheckingLicenceKey = false;
            return newState;
        }
    }
}
