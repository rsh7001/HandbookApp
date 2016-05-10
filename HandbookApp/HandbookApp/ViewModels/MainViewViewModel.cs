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
using System.Reactive;
using System.Reactive.Linq;
using HandbookApp.Actions;
using HandbookApp.Services;
using HandbookApp.States;
using ReactiveUI;
using Splat;


namespace HandbookApp.ViewModels
{

    public class MainViewViewModel : ReactiveObject, IRoutableViewModel
    {

        private string _articleId;
        public string ArticleId
        {
            get { return _articleId; }
            set { this.RaiseAndSetIfChanged (ref _articleId, value); }
        }

        private string _articleTitle;
        public string ArticleTitle
        {
            get { return _articleTitle; }
            set { this.RaiseAndSetIfChanged (ref _articleTitle, value); }
        }

        private string _numArticles;
        public string NumArticles
        {
            get { return _numArticles; }
            set { this.RaiseAndSetIfChanged (ref _numArticles, value); }
        }

        private string _numBookpages;
        public string NumBookpages
        {
            get { return _numBookpages; }
            set { this.RaiseAndSetIfChanged (ref _numBookpages, value); }
        }

        private string _numBooks;
        public string NumBooks
        {
            get { return _numBooks; }
            set { this.RaiseAndSetIfChanged(ref _numBooks, value); }
        }

        private ObservableAsPropertyHelper<bool> _canIncrement;
        public bool CanIncrement { get { return _canIncrement.Value; } }

        public string UrlPathSegment
        {
            get
            {
                return "ReactiveMainPage";
            }
        }

        public IScreen HostScreen { get; protected set; }
        

        public ReactiveCommand<Unit> Update;
        public ReactiveCommand<Unit> Increment;
        public ReactiveCommand<Unit> Decrement;
        

        public MainViewViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            this.WhenAnyValue(i => i.ArticleId, i => !string.IsNullOrWhiteSpace(i))
                .ToProperty(this, v => v.CanIncrement, out _canIncrement);

            var canExecuteIncrement = 
                this.WhenAnyValue (x => x.CanIncrement);

            Update = ReactiveCommand.CreateAsyncObservable(x => updateImpl());
  
            Increment = ReactiveCommand.CreateAsyncObservable<Unit> (canExecuteIncrement, _ => incrementImpl());

            Decrement = ReactiveCommand.CreateAsyncObservable<Unit> (canExecuteIncrement, _ => decrementImpl());

            App.Store
                .DistinctUntilChanged(state => new { state.Articles })
                .Subscribe(state => setNumArticles(state));

            App.Store
                .DistinctUntilChanged(state => new { state.Bookpages })
                .Subscribe(state => setNumBookpages(state));

            App.Store
                .DistinctUntilChanged(state => new { state.Books })
                .Subscribe(state => setNumBooks(state));
                
                
        }

        private void setNumBooks(AppState state)
        {
            NumBooks = state.Books.Count.ToString();
        }

        private void setNumBookpages(AppState state)
        {
            NumBookpages = state.Bookpages.Count.ToString();
        }

        private void setNumArticles(AppState state)
        {
            NumArticles = state.Articles.Count.ToString();
        }

        private IObservable<Unit> updateImpl()
        {
            JsonServerService.JsonServerUpdate();
            return Observable.Start(() => { return Unit.Default; });
        }

        private IObservable<Unit> decrementImpl()
        {
            App.Store.Dispatch(new DeleteArticleAction { ArticleId = _articleId });
            return Observable.Start(() => { return Unit.Default; });
        }

        private IObservable<Unit> incrementImpl()
        {
            App.Store.Dispatch(new AddArticleAction { ArticleId = _articleId, Title = _articleTitle, Content = "" });
            return Observable.Start(() => { return Unit.Default; });
        }

    }
}
