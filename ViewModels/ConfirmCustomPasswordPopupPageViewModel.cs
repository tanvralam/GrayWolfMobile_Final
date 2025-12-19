using RGPopup.Maui.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class ConfirmCustomPasswordPopupPageViewModel : BasePopupViewModel
    {
        private readonly string _savedPassword;
        private readonly IOnCustomPasswordConfirmedListener _listener;

        public ICommand ConfirmCommand { get; }

        public ConfirmCustomPasswordPopupPageViewModel(string password, IOnCustomPasswordConfirmedListener listener)
        {
            _savedPassword = password;
            _listener = listener;
            ConfirmCommand = new Command<string>(Confirm);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void Confirm(string customPasword)
        {
            if (!SetBusy())
            {
                return;
            }

            if(customPasword != _savedPassword)
            {
                await Alert.ShowMessage(Localization.Localization.DataSecurity_PasswordMissmatch);
                IsBusy = false;
                return;
            }

            await _listener.OnPasswordConfirmedAsync(customPasword);
            await Navigation.PopAllPopupAsync();

            IsBusy = false;
        }
    }
}
