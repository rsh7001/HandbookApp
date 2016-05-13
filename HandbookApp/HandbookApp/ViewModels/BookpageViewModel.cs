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
using Xamarin.Forms;

namespace HandbookApp.ViewModels
{
    public class BookpageViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; protected set; }

        private string _urlPathSegment;
        public string UrlPathSegment
        {
            get {  return _urlPathSegment; }
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

        private string _bookpageArticleContent;
        public string BookpageArticleContent
        {
            get { return _bookpageArticleContent; }
            set { this.RaiseAndSetIfChanged(ref _bookpageArticleContent, value); }
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

        private WebViewSource _pageSource;
        public WebViewSource PageSource
        {
            get { return _pageSource; }
            set { this.RaiseAndSetIfChanged(ref _pageSource, value); }
        }

        private HtmlWebViewSource _pageHtml;
        public HtmlWebViewSource PageHtml
        {
            get { return _pageHtml; }
            set { this.RaiseAndSetIfChanged(ref _pageHtml, value); }
        }

        public ReactiveCommand<Unit> GoBack;
        public ReactiveCommand<Object> GoToPage;

        private Article _currentArticle;

        public BookpageViewModel(string url, IScreen hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();
            GoBack = HostScreen.Router.NavigateBack;
            GoToPage = ReactiveCommand.CreateAsyncObservable(x => HostScreen.Router.Navigate.ExecuteAsync(new BookpageViewModel((string) x, HostScreen)));

            if(url.StartsWith("http"))
            {
                var urlsource = new UrlWebViewSource();
                urlsource.Url = url;
                PageSource = urlsource;
                return;
            }
            Bookpage _currentPage = getBookpage(url);
            _currentArticle = null;
            if (_currentPage != null)
            {
                BookpageLinks = setLinks(_currentPage.Links);
                BookpageArticleId = _currentPage.ArticleId;
                BookpageLinksTitle = _currentPage.LinksTitle;

                _currentArticle = getArticle(_currentPage.ArticleId);
                BookpageTitle = setTitle(_currentPage, _currentArticle);
                BookpageArticleContent = setArticleContent(_currentArticle);
                _urlPathSegment = setBookpageLinkTitle(_currentPage);
                if(App.HtmlService.Formattedpages.ContainsKey(url))
                {
                    PageSource = App.HtmlService.Formattedpages[url];
                }
                else
                {
                    PageSource = createHtmlContent(_currentArticle, BookpageLinksTitle, BookpageLinks);
                }
            }
            else
            {
                // TODO: Log error and return
                _urlPathSegment = "No page";
            }
            
        }

        private string setArticleContent(Article article)
        {
            if(article == null)
            {
                return null;
            }

            return article.Content;
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


        private HtmlWebViewSource createHtmlContent(Article article, string linkstitle, List<Tuple<string, string>> pagelinks)
        {
            const string initial = "<!DOCTYPE html><html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta charset=\"UTF-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"></head><body><div id=\"content\">";
            string content = "";
            if (article != null)
            {
                content = article.Content;
            }
            else
            {
                content = "<h2 class=\"linksTitle\">" + linkstitle + "</h2>";
            }

            string links = "";
            foreach (var link in pagelinks)
            {
                links = links + "<div class=\"clickableLink\"><p class=\"clickableLink\"><a class=\"clickableLink\" href=\"hybrid://" + link.Item1 + "\">" + link.Item2 + "</a></p></div>";
            }

            var result = new HtmlWebViewSource();
            result.Html = initial + content + links + "</div></body></html>";
            return result;
        }
    }
}
