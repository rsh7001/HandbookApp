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

using System.Reactive;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using HandbookApp.States;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;


namespace HandbookApp.ViewModels
{
    [DataContract]
    public class MainViewBookTileViewModel : CustomBaseViewModel
    {
        [Reactive][DataMember] public Book Model { get; set; }

        [IgnoreDataMember]
        public ReactiveCommand<Unit> OpenThisBook { get; protected set; }


        public MainViewBookTileViewModel(Book model, IScreen hostscreen = null)
        {
            _urlPathSegment = model.Id;
            _viewModelName = this.GetType().ToString();
             
            HostScreen = hostscreen ?? Locator.Current.GetService<IScreen>();

            Model = model;
            
            OpenThisBook = ReactiveCommand.CreateAsyncTask(x => openThisBookImpl(model.StartingBookpage));
        }

        private async Task openThisBookImpl(string startingBookpage)
        {
            var vm = new BookpageViewModel(startingBookpage, HostScreen);
            await HostScreen.Router.Navigate.ExecuteAsyncTask(vm);
        }
    }
}
