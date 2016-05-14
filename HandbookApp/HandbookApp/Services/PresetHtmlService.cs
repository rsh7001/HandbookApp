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
using System.Collections.Immutable;
using HandbookApp.States;
using Xamarin.Forms;

namespace HandbookApp.Services
{
    public class PresetHtmlService
    {
        private static string stylesheet = EmbeddedFileService.readFile("HandbookApp.EmbeddedContent.Css.asciidoctor.css");

        public ImmutableDictionary<string, HtmlWebViewSource> Formattedpages { get; set; }

        public void CreateHtmlPages()
        {
            var pages = ImmutableDictionary<string, HtmlWebViewSource>.Empty;
            foreach (var p in App.Store.GetState().Bookpages)
            {
                pages.SetItem(p.Key, createPage(p.Value));
            }
            Formattedpages = pages;
        }

        private HtmlWebViewSource createPage(Bookpage bp)
        {
            List<Tuple<string, string>> links = new List<Tuple<string, string>>();

            Article article = null;
            if(bp.ArticleId != null)
            {
                if(App.Store.GetState().Articles.ContainsKey(bp.ArticleId))
                {
                    article = App.Store.GetState().Articles[bp.ArticleId];
                }
            }

            foreach (var id in bp.Links)
            {
                Bookpage bookpage = getBookpage(id);
                if(bookpage != null)
                {
                    var item = new Tuple<string, string>(id, setBookpageLinkTitle(bookpage));
                    links.Add(item);
                }
            }

            return createHtmlContent(article, bp.LinksTitle, links);
            
        }

        private string setBookpageLinkTitle(Bookpage bookpage)
        {
            var bookpageArticle = getArticle(bookpage.ArticleId);
            return setTitle(bookpage, bookpageArticle) ?? bookpage.LinksTitle;
        }

        private string setTitle(Bookpage bookpage, Article article)
        {
            if (bookpage == null)
            {
                return null;
            }

            if (article == null)
            {
                return bookpage.Title;
            }

            return article.Title;
        }

        private Article getArticle(string articleId)
        {
            if (articleId == null)
            {
                return null;
            }

            if (App.Store.GetState().Articles.ContainsKey(articleId))
            {
                return App.Store.GetState().Articles[articleId];
            }

            return null;
        }

        private Bookpage getBookpage(string id)
        {
            if (App.Store.GetState().Bookpages.ContainsKey(id))
            {
                return App.Store.GetState().Bookpages[id];
            }

            return null;
        }

        private HtmlWebViewSource createHtmlContent(Article article, string linkstitle, List<Tuple<string, string>> pagelinks)
        {
            const string initial = "<!DOCTYPE html><html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta charset=\"UTF-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><style>";
            string initial2 = initial + stylesheet + "</style></head><body><div id=\"content\">";
            
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
                links = links + "<div class=\"clickableLink\"><p color=\"red\"><a class=\"clickableLink\" href=\"hybrid://" + link.Item1 + "\">" + link.Item2 + "</a></p></div>";
            }
             
            var result = new HtmlWebViewSource();
            var resultstring = initial2 + content + links + "</div></body></html>";
            result.Html = resultstring;
            return result;
        }
    }
}
