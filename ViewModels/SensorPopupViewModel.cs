using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using GrayWolf.Views;
using GrayWolf.Views.Popups;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class SensorPopupViewModel : BasePopupViewModel
    {
        #region properties
        private List<SensorUnit> _units;
        public List<SensorUnit> Units
        {
            get => _units;
            private set => SetProperty(ref _units, value);
        }

        private SensorUnit _selectedUnit;
        public SensorUnit SelectedUnit
        {
            get => _selectedUnit;
            set => SetProperty(ref _selectedUnit, value);
        }

        private bool IsFailed { get; set; }

        private bool _isVOCButtonVisible;
        public bool IsVOCButtonVisible
        {
            get => _isVOCButtonVisible;
            private set => SetProperty(ref _isVOCButtonVisible, value);
        }

        private Exception InitException { get; set; }

        private Reading _reading;
        public Reading Reading
        {
            get => _reading;
            private set => SetProperty(ref _reading, value);
        }

        private bool _isNetworkAvailable;
        public bool IsNetworkAvailable
        {
            get => _isNetworkAvailable;
            private set => SetProperty(ref _isNetworkAvailable, value);
        }

        private bool _isUnitActive = true;
        public bool IsUnitActive
        {
            get => _isUnitActive;
            private set => SetProperty(ref _isUnitActive, value);
        }
        #endregion

        #region services
        private ISensorsService SensorsService { get; }
        private IReadingService ReadingService { get; }
        #endregion

        #region commands
        public ICommand IsLoggedCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand TipsCommand { get; }
        public ICommand VOCCommand { get; }
        public ICommand SelectUnitCommand { get; }
        #endregion

        public SensorPopupViewModel(string readingId)
        {
            SensorsService = Ioc.Default.GetService<ISensorsService>();
            ReadingService = Ioc.Default.GetService<IReadingService>();
            IsLoggedCommand = new Command(ChangeIsLogged);
            ConfirmCommand = new Command(ConfirmConversion);
            TipsCommand = new Command(GoToTips);
            VOCCommand = new Command(GoToVOC);
            SelectUnitCommand = new Command<SensorUnit>(SelectUnit);

            Initialize(readingId);
        }

        private async void Initialize(string readingId)
        {
            try
            {
                IsUnitActive = !IsLogActive;
                Reading = await ReadingService.GetReadingByIdAsync(readingId);
                var units = SensorsService.GetSensorUnits((int)Reading.SensorCode);
                if(!units.Any(x => x.Code == Reading.OriginalUnit.Code))
                {
                    units.Insert(0, Reading.OriginalUnit);
                }
                IsVOCButtonVisible = SensorsService.IsPID((int)Reading.SensorCode);
                Units = units;
                SelectedUnit = Units.FirstOrDefault(x => x.Code == Reading.ConvertedUnit.Code);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                IsFailed = true;
                InitException = ex;
                CheckInitializeException();
            }
        }

        private async void CheckInitializeException()
        {
            if (IsActive && IsFailed)
            {
                await Alert.DisplayError(InitException);
                await OnBacksAsync();
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            CheckInitializeException();
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            IsNetworkAvailable = ContainsInternetConnectionProfile(Connectivity.ConnectionProfiles);
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            IsNetworkAvailable = ContainsInternetConnectionProfile(e.ConnectionProfiles);
        }

        private bool ContainsInternetConnectionProfile(IEnumerable<ConnectionProfile> profiles)
        {
            return profiles.Contains(ConnectionProfile.Cellular) || profiles.Contains(ConnectionProfile.Ethernet) || profiles.Contains(ConnectionProfile.WiFi);
        }

        private async void GoToTips()
        {
            if (await ShowAlertIfNoNetworkAsync() || !SetBusy())
            {
                return;
            }

            var device = await DeviceService.GetDeviceByIdAsync(Reading.DeviceId);
            await Navigation.PopAllPopupAsync();
            await Navigation.PushAsync(new SensorTipsPage(Reading, device.DeviceType));

            IsBusy = false;
        }

        private async void GoToVOC()
        {
            if (await ShowAlertIfNoNetworkAsync() || !SetBusy())
            {
                return;
            }

            try
            {
                var vocService = Ioc.Default.GetService<IVOCService>();

                var items = await vocService.GetVOCItemsAsync();

                var tcs = new TaskCompletionSource<VOCItem>();
                await Navigation.PushPopupAsync(new SelectVOCItemPopupPage(items, tcs));
                var item = await tcs.Task;

                await Navigation.PopAllPopupAsync();
                await Navigation.PushAsync(new VOCListPage(items, item));
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                await Alert.DisplayError(ex);
            }

            IsBusy = false;
        }

        private async Task<bool> ShowAlertIfNoNetworkAsync()
        {
            if (IsNetworkAvailable)
            {
                return false;
            }
            await Alert.ShowMessage(Localization.Localization.Error_NetworkRequired);
            return true;
        }

        private async void ConfirmConversion()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                if(SelectedUnit != null)
                {
                    
                    DeviceService.SetUnitConversion(Reading, SelectedUnit);
                }
                await DeviceService.UpdateReadingVisibilityAsync(Reading.Id, Reading.IsLogged);
                DeviceService.NotifyDeviceUpdated(Reading.DeviceId);
                await OnBacksAsync();
            }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ChangeIsLogged()
        {
            if(Reading == null || LogService.IsLogging)
            {
                return;
            }
            Reading.IsLogged = !Reading.IsLogged;
        }

        private void SelectUnit(SensorUnit unit)
        {
            if (LogService.IsLogging)
                return;
            SelectedUnit = unit;
        }
    }
}
