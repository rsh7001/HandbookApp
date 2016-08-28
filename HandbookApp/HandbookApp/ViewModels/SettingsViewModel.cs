using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HandbookApp.Actions;
using ReactiveUI;
using HandbookApp.Utilities;


namespace HandbookApp.ViewModels
{
    [DataContract]
    public class SettingsViewModel : CustomBaseViewModel
    {
        [IgnoreDataMember]
        public ReactiveCommand<Unit> RefreshContent;

        [IgnoreDataMember]
        public ReactiveCommand<Unit> Logout;

        [IgnoreDataMember]
        public ReactiveCommand<Unit> ResetLicenceKey;

        [IgnoreDataMember]
        public ReactiveCommand<Unit> ReturnToMainPage;


        public SettingsViewModel(IScreen hostscreen) : base(hostscreen)
        {
            _viewModelName = this.GetType().ToString();
            _urlPathSegment = "Settings";

            RefreshContent = ReactiveCommand.CreateAsyncTask(x => refreshContentImpl());

            Logout =  ReactiveCommand.CreateAsyncTask(x => logoutImpl());
            ResetLicenceKey = ReactiveCommand.CreateAsyncTask(x => resetLicenceKeyImpl());

            ReturnToMainPage = ReactiveCommand.CreateAsyncTask(x => returnToMainPageImpl());
        }

        private async Task returnToMainPageImpl()
        {
            await NavigateToMainAsync();
        }

        private async Task refreshContentImpl()
        {
            await App.Store.Dispatch(ServerActionCreators.ServerResetUpdatesAction());

            await NavigateToMainAsync();
        }

        private async Task logoutImpl()
        {
            await Task.Run(() => {
                App.Store.Dispatch(new LogoutAction());
            });

            await NavigateToMainAsync();
        }

        private async Task resetLicenceKeyImpl()
        {
            await Task.Run(() => {
                App.Store.Dispatch(new ClearLicenceKeyAction());
            });
        }
    }
}
