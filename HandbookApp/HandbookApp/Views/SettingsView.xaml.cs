using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandbookApp.ViewModels;
using Xamarin.Forms;
using ReactiveUI;
using HandbookApp.Utilities;

namespace HandbookApp.Views
{
    public partial class SettingsView : BasePage<SettingsViewModel>
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        protected override void SetupObservables()
        {
            this.OneWayBind(ViewModel, vm => vm.Logout, c => c.logoutButton.Command);
            this.OneWayBind(ViewModel, vm => vm.ResetLicenceKey, c => c.resetLicenceKeyButton.Command);
            this.OneWayBind(ViewModel, vm => vm.RefreshContent, c => c.resetContentsButton.Command);
            this.OneWayBind(ViewModel, vm => vm.ReturnToMainPage, c => c.returnMainPageButton.Command);
        }
    }
}
