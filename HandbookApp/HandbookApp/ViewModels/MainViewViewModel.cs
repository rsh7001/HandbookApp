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
using System.Threading.Tasks;
using HandbookApp.Actions;
using HandbookApp.Services;
using ReactiveUI;


namespace HandbookApp.ViewModels
{
    public class MainViewViewModel : ReactiveObject
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
            set {  this.RaiseAndSetIfChanged (ref _numArticles, value); }
        }

        private ObservableAsPropertyHelper<bool> _canIncrement;
        public bool CanIncrement { get { return _canIncrement.Value; } }


        public ReactiveCommand<Unit> ExecuteUpdate;
        public ReactiveCommand<Unit> ExecuteIncrement;
        public ReactiveCommand<Unit> ExecuteDecrement;


        public MainViewViewModel()
        {
            this.WhenAnyValue(i => i.ArticleId, i => !string.IsNullOrWhiteSpace(i))
                .ToProperty(this, v => v.CanIncrement, out _canIncrement);

            var canExecuteIncrement = 
                this.WhenAnyValue (x => x.CanIncrement);


            ExecuteUpdate = ReactiveCommand.CreateAsyncTask (
                async _ => {
                    await JsonServerService.JsonServerUpdate();
                    return Unit.Default;
                });


            ExecuteIncrement = ReactiveCommand.CreateAsyncTask<Unit> (
                canExecuteIncrement,
                async _ => {
                    App.Store.Dispatch(new AddArticleAction { ArticleId = _articleId, Title = _articleTitle, Content = "" });
                    await Task.Delay(1);
                    return Unit.Default;
                });

            ExecuteDecrement = ReactiveCommand.CreateAsyncTask<Unit> (
                canExecuteIncrement,
                async _ => {
                    App.Store.Dispatch(new DeleteArticleAction { ArticleId = _articleId });
                    await Task.Delay(1);
                    return Unit.Default;
                });

            App.Store
                .Subscribe(state => {
                    NumArticles = state.Articles.Count.ToString();
                });


        }
    }
}
