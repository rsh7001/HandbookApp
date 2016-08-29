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

using HandbookApp.ViewModels;
using HandbookApp.Utilities;
using ReactiveUI;
using Xamarin.Forms;


namespace HandbookApp.Views
{
    public partial class MainView : BasePage<MainViewModel>
    {
        public MainView()
        {
            InitializeComponent();
        }

        protected override void SetupObservables()
        {
            this.OneWayBind(ViewModel, vm => vm.BackgroundTaskRunning, c => c.updatingSpinner.IsRunning);
            this.OneWayBind(ViewModel, vm => vm.LastUpdateTime, c => c.updateTime.Text);
            this.OneWayBind(ViewModel, vm => vm.Handbooks, c => c.booksList.ItemsSource);
            this.OneWayBind(ViewModel, vm => vm.SettingsPage, c => c.settingsButton.Command);
        }

    }

}
