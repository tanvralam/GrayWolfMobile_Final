using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GrayWolf.Interfaces;
using GrayWolf.Services;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using IFileSystem = GrayWolf.Interfaces.IFileSystem;
using Acr.UserDialogs;

namespace GrayWolf.ViewModels
{
    public class BaseViewModel : ViewModelBase
    {
        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            private set => SetProperty(ref _isActive, value);
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);
            return true;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            RaisePropertyChanged(propertyName);
        }

        public Command BacksCommand { get; set; }

        public IAnalyticsService AnalyticsService => Ioc.Default.GetService<IAnalyticsService>();

        public INavigation Navigation => SimpleIoc.Default.GetInstance<INavigation>();
        public IApiService Api => Ioc.Default.GetService<IApiService>();

        public ISettingsService Settings => Ioc.Default.GetService<ISettingsService>();

        public IDeviceService DeviceService => Ioc.Default.GetService<IDeviceService>();
        public IAlertService Alert => AlertService.Instance;

        public IDatabase Database => Ioc.Default.GetService<IDatabase>();

        public ILogService LogService => Ioc.Default.GetService<ILogService>();

        public IFileSystem FileSystem => Ioc.Default.GetService<IFileSystem>();

        public BaseViewModel()
        {
            MessengerInstance = Messenger.Default;
            try
            {
                BacksCommand = new Command(async () => await OnBacksAsync());
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, GetType().Name);
                Alert.DisplayError(ex);
            }
        }

        /// <summary>
        /// TODO : To Navigate To Back Page...
        /// </summary>
        public virtual Task OnBacksAsync()
        {
            return Navigation.PopAsync();
        }
        public IUserDialogs UserDialog
        {
            get
            {
                return Ioc.Default.GetService<IUserDialogs>();
            }
        }
        public async Task PushModalAsync(Page page)
        {
            if (Navigation != null)
                await Navigation.PushModalAsync(page);
        }

        public async Task PopModalAsync()
        {
            if (Navigation != null)
                await Navigation.PopModalAsync();
        }

        public async Task PushAsync(Page page)
        {
            if (Navigation != null)
                await Navigation.PushAsync(page);
        }

        public async Task PopAsync()
        {
            if (Navigation != null)
                await Navigation.PopModalAsync();
        }

        public virtual void OnAppearing()
        {
            IsActive = true;
        }

        public virtual void OnDisappearing()
        {
            IsActive = false;
        }

        protected bool SetBusy()
        {
            if (IsBusy)
            {
                return false;
            }
            return IsBusy = true;
        }
    }
}
