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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandbookApp.Actions;
using HandbookApp.Services;
using HandbookApp.States;
using Xamarin.Forms;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.PlatformServices;

namespace HandbookApp.Views
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        public MainPage()
        {
            InitializeComponent();

            updateButton.Clicked += UpdateButton_Clicked;
            incrementButton.Clicked += IncrementButton_Clicked;
            decrementButton.Clicked += DecrementButton_Clicked;

            App.Store
                .Subscribe(state => {
                    Device.BeginInvokeOnMainThread( () => {
                        numLbl.Text = state.Articles.Count.ToString();
                    });
                });
                   
        }

        private void DecrementButton_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(entryId.Text))
            {
                App.Store.Dispatch(new DeleteArticleAction { ArticleId = entryId.Text });
            }
        }

        private void IncrementButton_Clicked(object sender, EventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(entryId.Text))
            {
                App.Store.Dispatch(new AddArticleAction { ArticleId = entryId.Text, Title = entryTitle.Text, Content = entryContent.Text });
            }
        }

        private void UpdateButton_Clicked(object sender, EventArgs e)
        {
            Task.Run(() => JsonServerService.JsonServerUpdate());
        }
    }
}
