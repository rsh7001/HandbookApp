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
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using HandbookApp.Utilities;
using HandbookApp.ViewModels;
using ReactiveUI;
using Xamarin.Forms;


namespace HandbookApp.Views
{
    public class BookpagePage : BasePage<BookpageViewModel>
    {
        private bool isNavigating;

        private WebView pageWebView;

        private IObservable<EventPattern<WebNavigatedEventArgs>> navigated;
        private IObservable<EventPattern<WebNavigatingEventArgs>> navigating;

        protected override void SetupViewElements()
        {
            base.SetupViewElements();

            isNavigating = false;

            pageWebView = new WebView();
            Content = pageWebView;
        }

        protected override void SetupObservables()
        {
            navigated = Observable.FromEventPattern<WebNavigatedEventArgs>(
                ev => pageWebView.Navigated += ev,
                ev => pageWebView.Navigated -= ev);

            navigating = Observable.FromEventPattern<WebNavigatingEventArgs>(
                ev => pageWebView.Navigating += ev,
                ev => pageWebView.Navigating -= ev);
        }

        protected override void SetupSubscriptions()
        {
            navigated
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => DisplayNavigated((WebNavigatedEventArgs) x.EventArgs))
                .DisposeWith(subscriptionDisposibles);

            navigating
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async x => await DisplayNavigating((WebNavigatingEventArgs) x.EventArgs))
                .DisposeWith(subscriptionDisposibles);

            this.WhenAnyValue(x => x.ViewModel.PageSource)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => pageWebView.Source = x)
                .DisposeWith(subscriptionDisposibles);
           
        }


        private async Task DisplayNavigating(WebNavigatingEventArgs eventArgs)
        {
            if (isNavigating)
            {
                return;
            }

            string url = null;
            if (eventArgs.Url.StartsWith("http"))
            {
                url = eventArgs.Url;
            }
            else if (eventArgs.Url.StartsWith("hybrid://"))
            {
                url = eventArgs.Url.Remove(0, 9);
            }
            else
            {
                isNavigating = true;
                return;
            }

            eventArgs.Cancel = true;
            await this.ViewModel.GoBookpage.ExecuteAsync(url);
        }

        private void DisplayNavigated(WebNavigatedEventArgs eventArgs)
        {
            isNavigating = false;
        }
    }
}
