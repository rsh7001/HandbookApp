using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using HandbookApp.ViewModels;
using ReactiveUI;
using Xamarin.Forms;

namespace HandbookApp.Views
{
    public partial class MainView : BasePage<MainViewModel>
    {
        public MainView()
        {
            InitializeComponent();
        }

        protected override void SetupObservables()
        {
            this.OneWayBind(ViewModel, vm => vm.BackgroundTaskRunning, c => c.updatingSpinner.IsRunning);
            this.OneWayBind(ViewModel, vm => vm.LastUpdateTime, c => c.updateTime.Text);
            this.OneWayBind(ViewModel, vm => vm.Handbooks, c => c.booksList.ItemsSource);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.OnAppearingSetup();
        }
    }

}
