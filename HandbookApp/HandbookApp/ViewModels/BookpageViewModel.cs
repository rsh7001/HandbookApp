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
using HandbookApp.States;
using ReactiveUI;
using Splat;

namespace HandbookApp.ViewModels
{
    public class BookpageViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; protected set; }

        public string UrlPathSegment
        {
            get {  return "page name"; }
        }

        private string _bookpageName;
        public string BookpageName
        {
            get { return _bookpageName; }
            set { this.RaiseAndSetIfChanged (ref _bookpageName, value); }
        }


        private string _bookpageTitle;
        public string BookpageTitle
        {
            get { return _bookpageTitle; }
            set { this.RaiseAndSetIfChanged(ref _bookpageTitle, value); }
        }

        private string _bookpageArticleId;
        public string BookpageArticleId
        {
            get { return _bookpageArticleId; }
            set { this.RaiseAndSetIfChanged(ref _bookpageArticleId, value); }
        }

        private string _bookpageLinksTitle;
        public string BookpageLinksTitle
        {
            get { return _bookpageLinksTitle; }
            set { this.RaiseAndSetIfChanged(ref _bookpageLinksTitle, value); }
        }
        
        private List<Tuple<string, string>> _bookpageLinks;
        public List<Tuple<string, string>> BookpageLinks
        {
            get { return _bookpageLinks; }
            set { this.RaiseAndSetIfChanged(ref _bookpageLinks, value); }
        }

        private Bookpage _currentPage;
        private Article _currentArticle;

        public ReactiveCommand<Unit> GoBack;

        public ReactiveCommand<Object> GoToPage;

        public BookpageViewModel(string bookpageId, IScreen hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            GoBack = HostScreen.Router.NavigateBack;

            GoToPage = ReactiveCommand.CreateAsyncObservable(x => HostScreen.Router.Navigate.ExecuteAsync(new BookpageViewModel((string) x, HostScreen)));

            _currentPage = getBookpage(bookpageId);
                
            if(_currentPage != null)
            {
                BookpageLinks = setLinks(_currentPage.Links);
                BookpageArticleId = _currentPage.ArticleId;
                BookpageLinksTitle = _currentPage.LinksTitle;
                
                _currentArticle = getArticle(_currentPage.ArticleId);
                BookpageTitle = setTitle(_currentPage, _currentArticle);
                BookpageName = bookpageId;
            }
            else
            {
                _bookpageName = "No page";
            }


        }

        private List<Tuple<string, string>> setLinks(List<string> links)
        {
            List<Tuple<string, string>> linkslst = new List<Tuple<string, string>>();

            foreach (var bookpageId in links)
            {
                Bookpage bookpage = getBookpage(bookpageId);
                if (bookpage == null)
                {
                    continue;
                }
                var item = new Tuple<string, string>(bookpageId, setBookpageLinkTitle(bookpage));
                linkslst.Add(item);
            }
            
            return linkslst;
        }

        private Article getArticle(string articleId)
        {
            if(articleId == null)
            {
                return null;
            }

            if (App.Store.GetState().Articles.ContainsKey(articleId))
            {
                return App.Store.GetState().Articles[articleId];
            }

            return null;
        }

        private string setTitle(Bookpage bookpage, Article article)
        {
            if(bookpage == null)
            {
                return null;
            }

            if(article == null)
            {
                return bookpage.Title;
            }

            return article.Title;
        }

        private string setBookpageLinkTitle(Bookpage bookpage)
        {
            var bookpageArticle = getArticle(bookpage.ArticleId);
            return setTitle(bookpage, bookpageArticle) ?? bookpage.LinksTitle;
        }

        private Bookpage getBookpage(string bookpageId)
        {
            if (App.Store.GetState().Bookpages.ContainsKey(bookpageId))
            {
                return App.Store.GetState().Bookpages[bookpageId];
            }

            return null;
        }
    }
}
