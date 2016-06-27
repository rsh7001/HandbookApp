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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using HandbookApp.Actions;
using HandbookApp.States;
using Microsoft.WindowsAzure.MobileServices;
using ModernHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;

namespace HandbookApp.Services
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ServerUpdateMessage
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

    public class AzureMobileService : IDisposable
    {
        private static string baseAddress = "http://192.168.72.70:56399/";
        //private static string baseAddress = "https://stanleyhum.azurewebsites.net/";
        private static string updateMessagesApi = "api/updates/";
        private const string applicationHeaderJson = "application/json";
        private const int timeoutDurationInMilliseconds = 19000; // TODO: Needs 19 seconds timeout to go to external website and download need to retry on first load
        private HttpClient _httpClient;

        private MobileServiceClient _azureClient;
        private IDictionary<string, string> _currentHeaders;
        private IDictionary<string, string> _currentParameters;

        public MobileServiceClient Client
        {
            get { return _azureClient; }
        }


        public AzureMobileService()
        {
            //_azureClient = new MobileServiceClient(Constants.MobileURL);
            _azureClient = new MobileServiceClient(baseAddress);
            _currentHeaders = new Dictionary<string, string>();
            _currentParameters = new Dictionary<string, string>();
            _httpClient = new HttpClient(new NativeMessageHandler());
            _httpClient.BaseAddress = new Uri(baseAddress);
            _httpClient.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(applicationHeaderJson));
        }

        private IObservable<HttpResponseMessage> serverUpdateCommand()
        {
            return Observable.FromAsync(() => _httpClient.PostAsync(updateMessagesApi, null));
        }

        //public IObservable<JToken> ServerUpdateCommand()
        //{
        //    return Observable.FromAsync(() => _azureClient.InvokeApiAsync("api/updates"));
        //    return Observable.FromAsync(() => _azureClient.InvokeApiAsync(
        //        apiName: updateMessagesApi,
        //        content: new StringContent(""),
        //        method: HttpMethod.Post,
        //        requestHeaders: _currentHeaders,
        //        parameters: _currentParameters
        //        ));
        //}

        public void JsonServerUpdate()
        {
            serverUpdateCommand()
                .Timeout(TimeSpan.FromMilliseconds(timeoutDurationInMilliseconds))
                .Finally(() => {
                    clearServerUpdate();
                })
                .Subscribe(
                    res => {
                        processHttpMessage(res);
                    },
                    ex => { System.Diagnostics.Debug.WriteLine(ex.Message); },
                    () => {
                        finishedServerUpdate();
                    }
                );
        }

        private void clearServerUpdate()
        {
            App.Store.Dispatch(new ClearUpdatingDataAction());
        }

        private static void finishedServerUpdate()
        {
            App.Store.Dispatch(new SetLastUpdateTimeAction { UpdateTime = DateTimeOffset.Now });
        }

        private static void processHttpMessage(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                App.Store.Dispatch(new LogoutAction());
                return;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                System.Diagnostics.Debug.WriteLine("Bad request");
                return;
            }

            response.Content.ReadAsStringAsync().ToObservable()
                .Subscribe(
                    res => { processJsonString(res); },
                    ex => { System.Diagnostics.Debug.WriteLine(ex.Message); },
                    () => { /** TODO: **/ }
                );
        }

        private static void processJTokenMessage(JToken response)
        {
            List<ServerUpdateMessage> messages = response.ToObject<List<ServerUpdateMessage>>();

            processMessages(messages);
        }

        private static void processJsonString(string responseJson)
        {
            List<ServerUpdateMessage> messages = JsonConvert.DeserializeObject<List<ServerUpdateMessage>>(responseJson);

            processMessages(messages);
        }

        private static void processMessages(List<ServerUpdateMessage> messages)
        {
            var addFullpages = messages
                .Where(x => x.Action == "AddFullpageAction")
                .Select(x => new Fullpage() { Id = x.FullPageID, Title = x.FullPageTitle, Content = new Xamarin.Forms.HtmlWebViewSource() { Html = x.FullPageContent } });
            var updateFullpageAction = new AddFullpageRangeAction { Fullpages = addFullpages.ToList() };
            App.Store.Dispatch(updateFullpageAction);

            var addBooks = messages
                .Where(x => x.Action == "AddBookAction")
                .Select(x => new Book() { Id = x.BookID, Title = x.BookTitle, StartingBookpage = x.BookStartingID, OrderIndex = x.BookOrder });
            var updateBookAction = new AddBookRangeAction { Books = addBooks.ToList() };
            App.Store.Dispatch(updateBookAction);
            App.Store.Dispatch(new ClearUpdatingDataAction());
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _azureClient.Dispose();
        }
    }
}
