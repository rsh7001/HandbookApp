using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandbookApp.States;
using Xamarin.Forms;
using System.Reactive.Linq;

namespace HandbookApp.Views
{
    public partial class MainSection : ContentView
    {
        public MainSection()
        {
            InitializeComponent();

            App.Store
                .Subscribe(state => {
                    Device.BeginInvokeOnMainThread(() => {
                        //ArticleItemsControl.ItemsSource = state.Articles.Values.OrderBy(x => x.Id);
                        setStackLayoutChildren(state.Articles.Values.OrderBy(x => x.Id));
                    });
                });
        }

        private void setStackLayoutChildren(IEnumerable<Article> values)
        {
            var elements = values
                .Select(x => new StackLayout {
                    Orientation = StackOrientation.Vertical,
                    Children = {
                        new Label {
                            Text = x.Id
                        },
                        new Label {
                            Text = x.Title
                        }
                    }
                });
            articlesSL.Children.Clear();
            foreach (var e in elements)
            {
                articlesSL.Children.Add(e);
            }
        }
    }
}
