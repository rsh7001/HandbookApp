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
using System.Reactive.Linq;
using HandbookApp.States;
using HandbookApp.Utilities;
using Microsoft.WindowsAzure.MobileServices;
using HandbookApp.Models;
using HandbookApp.Models.ServerRequests;
using HandbookApp.Models.ServerUtility;
using Redux;
using System.Threading.Tasks;
using HandbookApp.Services;

namespace HandbookApp.Actions
{

    public static class ServerActionCreators
    {
        private static bool httpClientAvailable = true;

        private static bool loggingIn = false;
        private static bool updatingData = false;
        private static bool verifyingLicenceKey = false;
        private static bool refreshingToken = false;
        private static bool postingUpdateData = false;
        private static bool loadingAppLog = false;
        private static bool fullupdating = false;


        public static AsyncActionsCreator<AppState> LoginAction(MobileServiceAuthenticationProvider provider)
        {
            return async (dispatch, getState) => {

                await doLogin(dispatch, getState, provider);

            };
        }
        
        public static AsyncActionsCreator<AppState> ServerVerifyLicenceKeyAction()
        {
            return async (dispatch, getState) => {

                await doVerifyLicenceKey(dispatch, getState);

            };
        }


        public static AsyncActionsCreator<AppState> ServerFullUpdateAction()
        {
            return async (dispatch, getState) => {

                if (!getState().CurrentState.IsLicensed || fullupdating)
                {
                    return;
                }

                fullupdating = true;

                await doPostUpdates(dispatch, getState);
                
                await doGetUpdates(dispatch, getState);

                await doPostUpdates(dispatch, getState);

                await doLoadAppLog(dispatch, getState);

                fullupdating = false;
            };
        }

        public static AsyncActionsCreator<AppState> ServerGetUpdateAction()
        {
            return async (dispatch, getState) => {

                await doGetUpdates(dispatch, getState);

            };
        }

        public static AsyncActionsCreator<AppState> ServerRefreshTokenAction()
        {
            return async (dispatch, getState) => {
                
                await doRefreshToken(dispatch, getState);
                
            };
        }

        public static AsyncActionsCreator<AppState> ServerPostUpdateAction()
        {
            return async (dispatch, getState) => {

                await doPostUpdates(dispatch, getState);

            };
        }

        public static AsyncActionsCreator<AppState> ServerLoadAppLogAction()
        {
            return async (dispatch, getState) => {

                await doLoadAppLog(dispatch, getState);

            };
        }


        private async static Task doLogin(Dispatcher dispatch, Func<AppState> getState, MobileServiceAuthenticationProvider provider)
        {
            if (!httpClientAvailable || loggingIn)
            {
                return;
            }

            httpClientAvailable = false;
            loggingIn = true;

            dispatch(new SetCheckingLoginAction());

            var success = await App.Authenticator.Authenticate(provider);

            if (success)
            {
                var user = App.ServerService.Client.CurrentUser;
                dispatch(new LoginAction {
                    UserId = user.UserId.ToString(),
                    AuthToken = user.MobileServiceAuthenticationToken.ToString()
                });

                App.ServerService.SetAzureUserCredentials(user.UserId.ToString(), user.MobileServiceAuthenticationToken.ToString());
            }
            else
            {
                dispatch(new LogoutAction());
            }

            loggingIn = false;
            httpClientAvailable = true;
        }

        private async static Task doVerifyLicenceKey(Dispatcher dispatch, Func<AppState> getState)
        {
            if (!getState().CurrentState.IsLoggedIn || verifyingLicenceKey || !httpClientAvailable)
            {
                return;
            }

            httpClientAvailable = false;
            verifyingLicenceKey = true;
            dispatch(new VerifyingLicenceKeyAction());

            bool success = false;

            try
            {
                VerifyLicenceKeyMessage vlkm = new VerifyLicenceKeyMessage { LicenceKey = getState().CurrentState.LicenceKey };
                success = await App.ServerService.VerifyLicenceKey(vlkm);

                if (success)
                {
                    dispatch(new SetLicensedAction());
                }
                else
                {
                    dispatch(new SetHasLicensedErrorAction());
                }

            }
            catch (Exception ex)
            {
                if (ex is ServerExceptions.NetworkFailure)
                {
                    // Do Nothing extra
                }
                else if (ex is ServerExceptions.ActionFailure)
                {
                    dispatch(new SetHasLicensedErrorAction());
                }
                else if (ex is ServerExceptions.Unauthorized)
                {
                    dispatch(new SetHasUnauthorizedErrorAction());
                }
                else if (ex is ServerExceptions.UnknownFailure)
                {
                    dispatch(new SetHasLicensedErrorAction());
                    if (ex.InnerException != null)
                    {
                    }

                }
                else
                {
                    dispatch(new SetHasLicensedErrorAction());
                }
            }

            dispatch(new ClearVerifyingLicenceKeyAction());
            verifyingLicenceKey = false;
            httpClientAvailable = true;
        }


        private async static Task doGetUpdates(Dispatcher dispatch, Func<AppState> getState)
        {

            if (!getState().CurrentState.IsLicensed || updatingData)
            {
                return;
            }

            updatingData = true;
            dispatch(new SetUpdatingDataAction());

            List<ServerUpdateMessage> messages = null;
            try
            {
                messages = await App.ServerService.GetUpdates();
                processServerUpdateMessages(messages);
                dispatch(new SetLastUpdateTimeAction { UpdateTime = DateTimeOffset.UtcNow });
            }
            catch (Exception ex)
            {
                if (ex is ServerExceptions.NetworkFailure)
                {
                    // Do Nothing extra
                }
                else if (ex is ServerExceptions.ActionFailure)
                {
                    // Do Nothing extra
                }
                else if (ex is ServerExceptions.Unauthorized)
                {
                    dispatch(new SetHasUnauthorizedErrorAction());
                }
                else if (ex is ServerExceptions.UnknownFailure)
                {
                    if (ex.InnerException != null)
                    {
                        //Log Inner Exception
                    }
                }
                else
                {
                    // Log Unknown exception
                }
            }
            finally
            {
                messages.Clear();
            }

            dispatch(new ClearUpdatingDataAction());
            updatingData = false;
        }

        private async static Task doRefreshToken(Dispatcher dispatch, Func<AppState> getState)
        {

            if (!getState().CurrentState.IsLoggedIn || refreshingToken)
            {
                return;
            }
            
            refreshingToken = true;
            dispatch(new SetRefreshingTokenAction());

            string token = null;
            try
            {
                token = await App.ServerService.RefreshToken();

                if (token != null)
                {
                    dispatch(new SetRefreshTokenAction { Token = token });
                    var user = App.ServerService.Client.CurrentUser;
                    App.ServerService.SetAzureUserCredentials(user.UserId.ToString(), token);
                }
                else
                {
                    dispatch(new SetHasUnauthorizedErrorAction());
                }

            }
            catch (Exception ex)
            {
                token = null;
                if (ex is ServerExceptions.NetworkFailure)
                {
                    // Do Nothing extra
                }
                else if (ex is ServerExceptions.ActionFailure)
                {
                    dispatch(new SetHasUnauthorizedErrorAction());
                }
                else if (ex is ServerExceptions.Unauthorized)
                {
                    dispatch(new SetHasUnauthorizedErrorAction());
                }
                else if (ex is ServerExceptions.UnknownFailure)
                {
                    dispatch(new SetHasUnauthorizedErrorAction());
                    if (ex.InnerException != null)
                    {
                        //Log inner Exception
                    }
                }
                else
                {
                    dispatch(new SetHasUnauthorizedErrorAction());
                    //Log Unknown Exception
                }
            }

            dispatch(new ClearRefreshingTokenAction());
            refreshingToken = false;
        }

        private async static Task doPostUpdates(Dispatcher dispatch, Func<AppState> getState)
        {
            if (!getState().CurrentState.IsLicensed || postingUpdateData)
            {
                return;
            }

            postingUpdateData = true;
            dispatch(new SetPostingUpdateDataAction());

            bool success = false;

            try
            {
                UpdateJsonMessage ujm = createPostUpdateJsonMessage();
                success = await App.ServerService.PostUpdates(ujm);

                if(success)
                {
                    dispatch(new RemoveLocalPostUpdatesDataAction { Data = ujm });
                }
            }
            catch (Exception ex)
            {
                if (ex is ServerExceptions.NetworkFailure)
                {
                    // Do Nothing Extra
                }
                else if (ex is ServerExceptions.ActionFailure)
                {
                    // Do Nothing Extra
                }
                else if (ex is ServerExceptions.Unauthorized)
                {
                    dispatch(new SetHasUnauthorizedErrorAction());
                }
                else if (ex is ServerExceptions.UnknownFailure)
                {
                    if (ex.InnerException != null)
                    {
                        //Log inner Exception
                    }
                }
                else
                {
                    //Log Unknown Exception
                }
            }
            
            dispatch(new ClearPostingUpdateDataAction());
            postingUpdateData = false;
        }

        private async static Task doLoadAppLog(Dispatcher dispatch, Func<AppState> getState)
        {

            if (!getState().CurrentState.IsLicensed || loadingAppLog)
            {
                return;
            }

            loadingAppLog = true;

            dispatch(new SetLoadingAppLogAction());

            bool success = false;
            
            List<AppLogItemMessage> items = LogStoreService.LogStore.ToList();

            try
            {
                success = await App.ServerService.LoadAppLog(items);

                if (success)
                {
                    LogStoreService.Clear();
                }
            }
            catch (Exception ex)
            {
                if (ex is ServerExceptions.NetworkFailure)
                {
                    // pass
                }
                else if (ex is ServerExceptions.ActionFailure)
                {

                }
                else if (ex is ServerExceptions.Unauthorized)
                {
                    dispatch(new SetHasUnauthorizedErrorAction());
                }
                else if (ex is ServerExceptions.UnknownFailure)
                {
                    if (ex.InnerException != null)
                    {
                        //Log inner Exception
                    }
                }
                else
                {
                    //Log Unknown Exception
                }
            }
            finally
            {
                items.Clear();
            }

            dispatch(new ClearLoadingAppLogAction());
            loadingAppLog = false;
        }


        private static void processServerUpdateMessages(List<ServerUpdateMessage> messages)
        {
            var addFullpages = messages
                .Where(x => x.Action == ServerUpdateMessage.AddFullpageActionId)
                .Select(x => new Fullpage() { Id = x.FullPageID, Title = x.FullPageTitle, Content = new Xamarin.Forms.HtmlWebViewSource() { Html = x.FullPageContent } });
            var updateFullpageAction = new AddFullpageRangeAction { Fullpages = addFullpages.ToList() };
            App.Store.Dispatch(updateFullpageAction);

            var updatePostFullpageAction = new AddPostUpdateAddFullpageIdsRangeAction { FullpageIds = addFullpages.Select(x => x.Id).ToList() };
            App.Store.Dispatch(updatePostFullpageAction);

                
            var addBooks = messages
                .Where(x => x.Action == ServerUpdateMessage.AddBookActionId)
                .Select(x => new Book() { Id = x.BookID, Title = x.BookTitle, StartingBookpage = x.BookStartingID, OrderIndex = x.BookOrder });
            var updateBookAction = new AddBookRangeAction { Books = addBooks.ToList() };
            App.Store.Dispatch(updateBookAction);

            var updatePostBookAction = new AddPostUpdateAddBookIdsRangeAction { BookIds = addBooks.Select(x => x.Id).ToList() };
            App.Store.Dispatch(updatePostBookAction);


            var deleteFullpages = messages
                .Where(x => x.Action == ServerUpdateMessage.DeleteFullpageActionId)
                .Select(x => x.FullPageID);
            var deleteFullpageAction = new DeleteFullpageRangeAction { FullpageIds = deleteFullpages.ToList() };
            App.Store.Dispatch(deleteFullpageAction);

            var deletePostFullpageAction = new AddPostUpdateDeleteFullpageIdsRangeAction { FullpageIds = deleteFullpages.ToList() };
            App.Store.Dispatch(deletePostFullpageAction);


            var deleteBooks = messages
                .Where(x => x.Action == ServerUpdateMessage.DeleteBookActionId)
                .Select(x => x.BookID);
            var deleteBookAction = new DeleteBookRangeAction { BookIds = deleteBooks.ToList() };
            App.Store.Dispatch(deleteBookAction);

            var deletePostBookAction = new AddPostUpdateDeleteBookIdsRangeAction { BookIds = deleteBooks.ToList() };
            App.Store.Dispatch(deletePostBookAction);

            if (messages.Count != 0)
            {
                App.Store.Dispatch(new SetReloadedAction());
            }
        }

        private static UpdateJsonMessage createPostUpdateJsonMessage()
        {
            return new UpdateJsonMessage {
                AddBookItemIds = App.Store.GetState().CurrentPostUpdateState.AddedBookIds.ToList(),
                DeleteBookItemIds = App.Store.GetState().CurrentPostUpdateState.DeletedBooksIds.ToList(),
                AddFullpageItemIds = App.Store.GetState().CurrentPostUpdateState.AddedFullpagesIds.ToList(),
                DeleteFullpageItemIds = App.Store.GetState().CurrentPostUpdateState.DeletedFullpagesIds.ToList()
            };
        }
    }
}
