using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace GrayWolf.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region properties
        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private List<GrayWolfDevice> _devices;
        public List<GrayWolfDevice> Devices
        {
            get => _devices;
            private set => SetProperty(ref _devices, value);
        }
        #endregion

        #region commands
        public ICommand LoginCommand { get; }

        public ICommand CancelCommand { get; }

        #region services
        private IAuthService AuthService { get; }
        #endregion
        #endregion

        public LoginViewModel()
        {
            LoginCommand = new AsyncRelayCommand(OnLogin);
            CancelCommand = new AsyncRelayCommand(OnCancel);

            AuthService = Ioc.Default.GetService<IAuthService>();

#if DEBUG
            Username = Constants.DEMO_LOGIN;
            Password = Constants.DEMO_PASS;
#endif
        }

        private Task OnCancel()
        {
            return Navigation.PopPopupAsync();
        }

        private async Task OnLogin()
        {
            if (!SetBusy())
            {
                return;
            }
            try
            {
                await AuthService.LoginAsync(Username, Password);
                await Navigation.PopPopupAsync();
            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e);
                await Alert.DisplayError(e);
            }
            finally
            {
                IsBusy = false;
            }

        }

    }
}
