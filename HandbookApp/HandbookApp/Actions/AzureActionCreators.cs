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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using HandbookApp.States;
using ModernHttpClient;
using Newtonsoft.Json;
using Redux;
using HandbookApp.Utilities;
using HandbookApp.Services;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Splat;

namespace HandbookApp.Actions
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ServerMessage
    {
        [JsonProperty]
        public int ID { get; set; }

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

    public static class AzureActionCreators
    {
        public static AsyncActionsCreator<AppState> LoginAction(MobileServiceAuthenticationProvider provider)
        {
            return async (dispatch, getState) => {


                dispatch(new SetCheckingLoginAction());

                var success = await App.Authenticator.Authenticate(provider);

                if (success)
                {
                    var user = App.ServerService.Client.CurrentUser;
                    dispatch(new LoginAction());
                }
                else
                {
                    dispatch(new LogoutAction());
                }
            };
        }

        public static AsyncActionsCreator<AppState> ServerUpdateAction()
        {
            return async (dispatch, getState) => {
                if (getState().CurrentState.IsDataUpdated || getState().CurrentState.IsUpdatingData)
                {
                    return;
                }

                dispatch(new SetUpdatingDataAction());
                
                App.ServerService.JsonServerUpdate();
                await Task.Delay(1);
            };
        }

        public static AsyncActionsCreator<AppState> CheckLicenceKeyAction()
        {
            return async (dispatch, getState) => {
                if (!getState().CurrentState.IsLoggedIn || getState().CurrentState.CheckingLicenceKey)
                {
                    return;
                }

                dispatch(new CheckingLicenceKeyAction());

                var success = await App.ServerService.CheckLicenceKey();
                
                if (success)
                {
                    dispatch(new SetLicensedAction());
                }
                else
                {
                    dispatch(new ClearLicensedAction());
                }

            };
        }
    }
}
