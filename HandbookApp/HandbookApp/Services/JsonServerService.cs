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
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HandbookApp.Actions;
using HandbookApp.States;
using Newtonsoft.Json;
using System.Reactive.Linq;
using System.Reactive;


namespace HandbookApp.Services
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
    }

    public class NewMessage
    {
        public string ID { get; set; }
        public string Action { get; set; }
    }

    public static class JsonServerService
    {
        public static string BaseAddress = "http://192.168.72.70:50051/";
        public static string UpdateMessagesApi = "messages/";
        public const string ApplicationHeaderJson = "application/json";

        public static async Task JsonServerUpdate()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationHeaderJson));

                try
                {
                    HttpResponseMessage response = await client.GetAsync(UpdateMessagesApi);
                    await processResponseMessage(response);
                }
                catch (Exception)
                {
                    // Log error
                }
            }
        }

        private static async Task processResponseMessage(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    List<ServerMessage> messages = JsonConvert.DeserializeObject<List<ServerMessage>>(responseJson);

                    var addArticles = messages
                        .Where(x => x.Action == "AddArticleAction")
                        .Select(x => new Article() { Id = x.ArticleID, Title = x.ArticleTitle, Content = x.ArticleContent });
                    var updateArticleAction = new AddArticleRangeAction { Articles = addArticles.ToList() };
                    App.Store.Dispatch(updateArticleAction);

                    var addPages = messages
                        .Where(x => x.Action == "AddBookpageAction")
                        .Select(x => new Bookpage() { Id = x.BookpageID, ArticleId = x.BookpageArticleID, LinksTitle = x.BookpageLinkTitle, Links = x.BookpageLinkIDs });
                    var updateBookpageAction = new AddBookpageRangeAction { Bookpages = addPages.ToList() };
                    App.Store.Dispatch(updateBookpageAction);
                }
                catch (Exception)
                {
                    // Log error
                }
            }
            else
            {
                // Log error
            }
        }

        private static void processServerMessage(ServerMessage m)
        {
            if (m.Action == "AddArticleAction")
            {
                System.Diagnostics.Debug.WriteLine(m.Action);
                System.Diagnostics.Debug.WriteLine(m.ArticleID);
                System.Diagnostics.Debug.WriteLine(m.ArticleTitle);
                AddArticleAction action = new AddArticleAction {
                    ArticleId = m.ArticleID,
                    Content = m.ArticleContent,
                    Title = m.ArticleTitle
                };
                App.Store.Dispatch(action);
            }
            else if (m.Action == "AddBookpageAction")
            {
                AddBookpageAction action = new AddBookpageAction {
                    PageId = m.BookpageID,
                    PageArticleId = m.BookpageArticleID,
                    PageLinksTitle = m.BookpageLinkTitle,
                    Links = m.BookpageLinkIDs
                };
                //App.Store.Dispatch(action);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Unknown Server Action");
            }
            System.Diagnostics.Debug.WriteLine("Added");
        }
    }
}
