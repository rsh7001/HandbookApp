using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandbookApp.Actions;
using HandbookApp.Services;
using HandbookApp.States;
using Xamarin.Forms;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.PlatformServices;

namespace HandbookApp.Views
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        public MainPage()
        {
            InitializeComponent();

            updateButton.Clicked += UpdateButton_Clicked;
            incrementButton.Clicked += IncrementButton_Clicked;
            decrementButton.Clicked += DecrementButton_Clicked;

            App.Store
                .Subscribe(state => {
                    Device.BeginInvokeOnMainThread( () => {
                        numLbl.Text = state.Articles.Count.ToString();
                    });
                });
                   
        }

        private void DecrementButton_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(entryId.Text))
            {
                App.Store.Dispatch(new DeleteArticleAction { ArticleId = entryId.Text });
            }
        }

        private void IncrementButton_Clicked(object sender, EventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(entryId.Text))
            {
                App.Store.Dispatch(new AddArticleAction { ArticleId = entryId.Text, Title = entryTitle.Text, Content = entryContent.Text });
            }
        }

        private void UpdateButton_Clicked(object sender, EventArgs e)
        {
            Task.Run(() => JsonServerService.JsonServerUpdate());
        }
    }
}
