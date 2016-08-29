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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HandbookApp.Models;
using HandbookApp.Models.ServerRequests;
using HandbookApp.Models.ServerResponses;
using HandbookApp.Models.ServerUtility;
using Microsoft.WindowsAzure.MobileServices;
using ModernHttpClient;
using Newtonsoft.Json;
using Splat;


namespace HandbookApp.Services
{
    public class AzureMobileService : IDisposable, IEnableLogger
    {
        private MobileServiceClient _azureClient;
        private HttpClient _httpClient;
#if DEBUG
        private static string TestMobileURL = "http://192.168.0.75:55506";
#endif
        private static string ProductionMobileURL = "https://handbookmobileappservice.azurewebsites.net/";
        private static string VerifyLicenceKeyAPI = "api/verifylicencekey";
        private static string ResetUpdatesAPI = "api/resetupdates";
        private static string GetUpdatesAPI = "api/updates";
        private static string PostUpdatesAPI = "api/postupdates";
        private static string RefreshTokenAPI = "api/refreshtoken";
        private static string LoadAppLogAPI = "api/loadapplog";
        private static string ZumoAuthName = "X-ZUMO-AUTH";
        private static string ZumoApiVersionName = "ZUMO-API-VERSION";
        private static string ZumoApiVersion = "2.0.0";
        private static string ContentJsonString = "application/json";

        public MobileServiceClient Client
        {
            get { return _azureClient; }
        }

        public AzureMobileService()
        {
            initializeAzureMobileServiceClient();
            initializeHttpClient();
        }


        public void SetAzureUserCredentials(string userId, string token)
        {
            _azureClient.CurrentUser = new MobileServiceUser(userId);
            _azureClient.CurrentUser.MobileServiceAuthenticationToken = token;
            _httpClient.DefaultRequestHeaders.Remove(ZumoAuthName);
            _httpClient.DefaultRequestHeaders.Add(ZumoAuthName, token);
        }


        public async Task<bool> VerifyLicenceKey(VerifyLicenceKeyMessage vlkm)
        {
            bool result = false;

            HttpResponseMessage response = null;
            HttpRequestMessage req = null;

            try
            {
                req = setupJSONRequest(VerifyLicenceKeyAPI, JsonConvert.SerializeObject(vlkm));
                response = await _httpClient.SendAsync(req);
                response.EnsureSuccessStatusCode();

                result = true;
                this.Log().Info("LicenceKey is Verified");
            }
            catch (Exception ex)
            {
                if (response == null)
                {
                    this.Log().InfoException("VerifyLicenceKey response is null", ex);
                    throw new ServerExceptions.NetworkFailure();
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            this.Log().InfoException("VerifyLicenceKey response code: Unauthorized", ex);
                            throw new ServerExceptions.Unauthorized();
                        case HttpStatusCode.BadRequest:
                            this.Log().InfoException(string.Format("VerifyLicenceKey response code: BadRequest: {0}", await response.Content.ReadAsStringAsync()), ex);
                            throw new ServerExceptions.ActionFailure();
                        default:
                            this.Log().InfoException(string.Format("VerifyLicenceKey response code: UnknownFailure: {0}", await response.Content.ReadAsStringAsync()), ex);
                            throw new ServerExceptions.UnknownFailure(ex);
                    }
                }
            }
            finally
            {
                if (req != null)
                {
                    req.Dispose();
                }

                if (response != null)
                {
                    response.Dispose();
                }
            }

            return result;
        }

        public async Task<bool> ResetUpdates()
        {
            bool result = false;

            HttpRequestMessage req = null;
            HttpResponseMessage response = null;

            try
            {
                req = setupJSONRequest(ResetUpdatesAPI, "");
                response = await _httpClient.SendAsync(req);
                response.EnsureSuccessStatusCode();

                result = true;
                this.Log().Info("ResetUpdates success");
            }
            catch (Exception ex)
            {
                if (response == null)
                {
                    this.Log().InfoException("ResetUdpates response is null", ex);
                    throw new ServerExceptions.NetworkFailure();
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            this.Log().InfoException("ResetUpdates response code: Unauthorized", ex);
                            throw new ServerExceptions.Unauthorized();
                        case HttpStatusCode.BadRequest:
                            this.Log().InfoException(string.Format("ResetUpdates response code: BadRequest: {0}", await response.Content.ReadAsStringAsync()), ex);
                            throw new ServerExceptions.ActionFailure();
                        default:
                            this.Log().InfoException(string.Format("ResetUpdates response code: UnknownFailure: {0}", await response.Content.ReadAsStringAsync()), ex);
                            throw new ServerExceptions.UnknownFailure(ex);
                    }
                }
            }
            finally
            {
                if (req != null)
                {
                    req.Dispose();
                }
                if (response != null)
                {
                    response.Dispose();
                }
            }

            return result;
        }

        public async Task<List<ServerUpdateMessage>> GetUpdates()
        {
            List<ServerUpdateMessage> results = null;

            HttpRequestMessage req = null;
            HttpResponseMessage response = null;

            try
            {
                req = setupJSONRequest(GetUpdatesAPI, "");
                response = await _httpClient.SendAsync(req);
                response.EnsureSuccessStatusCode();

                results = await doSuccessfulGetUpdates(response);
                this.Log().Info("GetUpdates results received: {0}", results.Count.ToString());
            }
            catch (Exception ex)
            {
                if (response == null)
                {
                    this.Log().InfoException("GetUpdates response is null", ex);
                    throw new ServerExceptions.NetworkFailure();
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            this.Log().InfoException("GetUpdates response code: Unauthorized", ex);
                            throw new ServerExceptions.Unauthorized();
                        case HttpStatusCode.BadRequest:
                            this.Log().InfoException(string.Format("GetUpdates response code: BadRequest: {0}", await response.Content.ReadAsStringAsync()), ex);
                            throw new ServerExceptions.ActionFailure();
                        default:
                            this.Log().InfoException(string.Format("GetUpdates response code: UnknownFailure: {0}", await response.Content.ReadAsStringAsync()), ex);
                            throw new ServerExceptions.UnknownFailure(ex);
                    }
                }
            }
            finally
            {
                if (req != null)
                {
                    req.Dispose();
                }

                if (response != null)
                {
                    response.Dispose();
                }
            }

            return results;
        }


        public async Task<bool> PostUpdates(UpdateJsonMessage ujm)
        {
            bool result = false;

            HttpRequestMessage req = null;
            HttpResponseMessage response = null;

            try
            {
                req = setupJSONRequest(PostUpdatesAPI, JsonConvert.SerializeObject(ujm));
                response = await _httpClient.SendAsync(req);
                response.EnsureSuccessStatusCode();

                result = true;
                this.Log().Info("PostUpdates success");
            }
            catch (Exception ex)
            {
                if (response == null)
                {
                    this.Log().InfoException("PostUpdates response is null", ex);
                    throw new ServerExceptions.NetworkFailure();
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            this.Log().InfoException("PostUpdates response code: Unauthorized", ex);
                            throw new ServerExceptions.Unauthorized();
                        case HttpStatusCode.BadRequest:
                        this.Log().InfoException(string.Format("PostUpdates response code: BadRequest: {0}", await response.Content.ReadAsStringAsync()), ex);
                            throw new ServerExceptions.ActionFailure();
                        default:
                        this.Log().InfoException(string.Format("PostUpdates response code: UnknownFailure: {0}", await response.Content.ReadAsStringAsync()), ex);
                            throw new ServerExceptions.UnknownFailure(ex);
                    }
                }
            }
            finally
            {
                if (req != null)
                {
                    req.Dispose();
                }

                if (response != null)
                {
                    response.Dispose();
                }
            }

            return result;
        }


        public async Task<string> RefreshToken()
        {
            string result = null;

            HttpRequestMessage req = null;
            HttpResponseMessage response = null;

            try
            {
                req = setupJSONRequest(RefreshTokenAPI, "");
                response = await _httpClient.SendAsync(req);
                response.EnsureSuccessStatusCode();

                result = await doSuccessfulRefreshToken(response);
                this.Log().Info("RefreshToken success");
            }
            catch (Exception ex)
            {
                if (response == null)
                {
                    this.Log().InfoException("RefreshToken response is null", ex);
                    throw new ServerExceptions.NetworkFailure();
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            this.Log().InfoException("PostUpdates response code: Unauthorized", ex);
                            throw new ServerExceptions.Unauthorized();
                        case HttpStatusCode.BadRequest:
                            this.Log().InfoException(string.Format("RefreshToken response code: BadRequest: {0}", await response.Content.ReadAsStringAsync()), ex);
                            throw new ServerExceptions.ActionFailure();
                        default:
                            this.Log().InfoException(string.Format("RefreshToken response code: UnknownFailure: {0}", await response.Content.ReadAsStringAsync()), ex);
                            throw new ServerExceptions.UnknownFailure(ex);
                    }
                }
            }
            finally
            {
                if (req != null)
                {
                    req.Dispose();
                }
                if (response != null)
                {
                    response.Dispose();
                }
            }
            return result;
        }


        public async Task<bool> LoadAppLog(List<AppLogItemMessage> items)
        {
            bool result = false;

            HttpRequestMessage req = null;
            HttpResponseMessage response = null;

            try
            {
                req = setupJSONRequest(LoadAppLogAPI, JsonConvert.SerializeObject(items));
                response = await _httpClient.SendAsync(req);
                response.EnsureSuccessStatusCode();

                result = true;
                this.Log().Info("LoadAppLog success");
            }
            catch (Exception ex)
            {
                if (response == null)
                {
                    this.Log().InfoException("LoadAppLog response is null", ex);
                    throw new ServerExceptions.NetworkFailure();
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            this.Log().InfoException("LoadAppLog response code: Unauthorized", ex);
                            throw new ServerExceptions.Unauthorized();
                        case HttpStatusCode.BadRequest:
                            this.Log().InfoException(string.Format("LoadAppLog response code: BadRequest: {0}", await response.Content.ReadAsStringAsync()), ex);
                            throw new ServerExceptions.ActionFailure();
                        default:
                            this.Log().InfoException(string.Format("LoadAppLog response code: UnknownFailure: {0}", await response.Content.ReadAsStringAsync()), ex);
                            throw new ServerExceptions.UnknownFailure(ex);
                    }
                }
            }
            finally
            {
                if (req != null)
                {
                    req.Dispose();
                }

                if (response != null)
                {
                    response.Dispose();
                }
            }
            return result;
        }


        public void Dispose()
        {
            _azureClient.Dispose();
            _httpClient.Dispose();
        }


        private void initializeAzureMobileServiceClient()
        {
#if DEBUG
            _azureClient = new MobileServiceClient(TestMobileURL);
            _azureClient.AlternateLoginHost = new Uri(ProductionMobileURL);
#else
            _azureClient = new MobileServiceClient(ProductionMobileURL);
#endif
        }

        private void initializeHttpClient()
        {
            _httpClient = new HttpClient(new NativeMessageHandler());
#if DEBUG
            _httpClient.BaseAddress = new Uri(TestMobileURL);
#else
            _httpClient.BaseAddress = new Uri(ProductionMobileURL);
#endif
            _httpClient.DefaultRequestHeaders.Add(ZumoApiVersionName, ZumoApiVersion);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentJsonString));
        }

        private HttpRequestMessage setupJSONRequest(string url, string content)
        {
            HttpRequestMessage result = new HttpRequestMessage(HttpMethod.Post, url);
            result.Content = new StringContent(content);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentJsonString);
            return result;
        }

        private async Task<List<ServerUpdateMessage>> doSuccessfulGetUpdates(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            List<ServerUpdateMessage> messages = JsonConvert.DeserializeObject<List<ServerUpdateMessage>>(content);
            return messages;
        }

        private async Task<string> doSuccessfulRefreshToken(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            TokenResponseMessage message = JsonConvert.DeserializeObject<TokenResponseMessage>(content);
            return message.Token;
        }
    }
}
