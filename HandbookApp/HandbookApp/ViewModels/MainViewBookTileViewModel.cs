using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using HandbookApp.States;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace HandbookApp.ViewModels
{
    public class MainViewBookTileViewModel : ReactiveObject, IRoutableViewModel
    {
        [Reactive] public Book Model { get; set; }

        public ReactiveCommand<Unit> OpenThisBook { get; protected set; }

        public string UrlPathSegment
        {
            get; protected set;
        }

        public IScreen HostScreen
        {
            get; protected set;
        }

        public MainViewBookTileViewModel(Book model, IScreen hostscreen = null)
        {
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
