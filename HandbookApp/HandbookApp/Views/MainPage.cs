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
using Xamarin.Forms;
using HandbookApp.Actions;
using HandbookApp.States;



namespace HandbookApp.Views
{
    public class CustomCell : ViewCell
    {
        public CustomCell()
        {
            StackLayout cellWrapper = new StackLayout();
            cellWrapper.Orientation = StackOrientation.Horizontal;
            Label articleIdLbl = new Label();
            Label titleLbl = new Label();
            Label contentLbl = new Label();

            articleIdLbl.SetBinding(Label.TextProperty, "Id");
            titleLbl.SetBinding(Label.TextProperty, "Title");
            contentLbl.SetBinding(Label.TextProperty, "Content");

            cellWrapper.Children.Add(articleIdLbl);
            cellWrapper.Children.Add(titleLbl);
            cellWrapper.Children.Add(contentLbl);
            View = cellWrapper;
        }
    }

    public class MainPage : ContentPage
    {
        private Entry articleIdEntry;
        private Entry articleTitleEntry;
        private Entry articleContentEntry;

        public MainPage()
        {
            articleIdEntry = new Entry();
            articleIdEntry.Text = "";

            articleTitleEntry = new Entry();
            articleTitleEntry.Text = "";

            articleContentEntry = new Entry();
            articleContentEntry.Text = "";


            var articleLst = new ListView();
            articleLst.ItemTemplate = new DataTemplate(typeof(CustomCell));

            var lbl = new Label();
            lbl.HorizontalTextAlignment = TextAlignment.Center;
            lbl.Text = "Welcome to Xamarin Forms!";

            var incrementButton = new Button();
            incrementButton.Text = "Increment";
            incrementButton.Clicked += IncrementButton_Clicked;

            var decrementButton = new Button();
            decrementButton.Text = "Decrement";
            decrementButton.Clicked += DecrementButton_Clicked;

            var numberLbl = new Label();
            numberLbl.HorizontalTextAlignment = TextAlignment.Center;
            numberLbl.Text = "-1";

            var sl = new StackLayout();
            sl.VerticalOptions = LayoutOptions.Start;
            sl.Children.Add(incrementButton);
            sl.Children.Add(decrementButton);
            sl.Children.Add(lbl);
            sl.Children.Add(numberLbl);
            sl.Children.Add(articleIdEntry);
            sl.Children.Add(articleTitleEntry);
            sl.Children.Add(articleContentEntry);
            sl.Children.Add(articleLst);

            Content = sl;
            
            App.Store.Subscribe( (AppState state) =>
            {
                numberLbl.Text = state.Articles.Count.ToString();
                articleLst.ItemsSource = state.Articles.Values;
            });
        }

        private void DecrementButton_Clicked(object sender, System.EventArgs e)
        {
            if(articleIdEntry.Text != "")
            {
                App.Store.Dispatch(new DeleteArticleAction { ArticleId = articleIdEntry.Text });
            }
        }

        private void IncrementButton_Clicked(object sender, System.EventArgs e)
        {
            if (articleIdEntry.Text != "")
            {
                App.Store.Dispatch(new AddArticleAction { ArticleId = articleIdEntry.Text, Title = articleTitleEntry.Text, Content = articleContentEntry.Text });
            }
        }
    }
}
