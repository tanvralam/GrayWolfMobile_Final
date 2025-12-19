using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using GrayWolf.Extensions;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using GrayWolf.Services;
using GrayWolf.Views;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class SelectDevicesPageViewModel : BaseViewModel
    {
        #region Properties

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsLoading));
                }
            }
        }

        private string _loggedUser;

        public string LoggedUser
        {
            get { return _loggedUser; }
            set
            {
                _loggedUser = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsLoggedIn));
            }
        }

        public bool IsLoggedIn => !string.IsNullOrWhiteSpace(_loggedUser);


        private ObservableCollection<GrayWolfDevice> _cloudDeviceList;

        public ObservableCollection<GrayWolfDevice> CloudDeviceList
        {
            get { return _cloudDeviceList; }
            set
            {
                _cloudDeviceList = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<BleDevice> _bluetoothDeviceList;

        public ObservableCollection<BleDevice> BluetoothDeviceList
        {
            get { return _bluetoothDeviceList; }
            set
            {
                _bluetoothDeviceList = value;
                RaisePropertyChanged();
            }
        }

        private IBleService BleService => DeviceService.BLEInstance;
        #endregion

        #region Commands

        public IAsyncRelayCommand OpenLogInCommand { get; }

        public IAsyncRelayCommand CancelCommand { get; }

        public IAsyncRelayCommand OkCommand { get; }

        public ICommand ItemSelectedCommand { get; }

        public ICommand BleDeviceSelectedCommand { get; }

        public ICommand LogOutCommand { get; }
        #endregion

        #region services
        private IAuthService AuthService { get; }
        #endregion

        public SelectDevicesPageViewModel()
        {
            OpenLogInCommand = new AsyncRelayCommand(OnOpenLogin);
            CancelCommand = new AsyncRelayCommand(OnCancel);
            OkCommand = new AsyncRelayCommand(OnOk);
            ItemSelectedCommand = new Command<GrayWolfDevice>(OnItemSelected);
            BleDeviceSelectedCommand = new Command<BleDevice>(OnBleDeviceSelected);
            LogOutCommand = new Command(OnLogout);
            AuthService = Ioc.Default.GetService<IAuthService>();
            LoggedUser = AuthService.CurrentUserLogin;
        }

        private async void OnLogout()
        {
            await AuthService.LogoutAsync();
            LoggedUser = null;
            
            var selectedDevices = await DeviceService.GetSelectedDevicesAsync();
            await DeviceService.SelectDevicesAsync(selectedDevices.Where(x => x.IsSelected && x.Source != Enums.DeviceSource.Cloud).ToList());
            
            RaisePropertyChanged(nameof(IsLoggedIn));
        }

        private void OnItemSelected(GrayWolfDevice arg)
        {
            arg.IsSelected = !arg.IsSelected;
        }

        private  void OnBleDeviceSelected(BleDevice arg)
        {
            arg.IsSelected = !arg.IsSelected;
        }

        private void FetchBleDevices()
        {
            var devices = DeviceService.GetDiscoveredBleDevices();
            OnBleDevicesUpdated(devices);
        }

       
        public override async void OnAppearing()
        {
            base.OnAppearing();
          
            IsLoading = true;
           // BleService.Initialize();//i have added            
            await BleService.StartScanAsync();
            FetchBleDevices();
            BleService.OnVisibleDevicesChanged += BleService_OnVisibleDevicesChanged;
            //await Load();
            IsLoading = false;
        }

        public override async void OnDisappearing()
        {
            base.OnDisappearing();
            await BleService.StopScanAsync();
            BleService.OnVisibleDevicesChanged -= BleService_OnVisibleDevicesChanged;
        }

        private bool IsDeviceInBluetoothList(BleDevice device, IEnumerable<BleDevice> source) => source.Any(x => x.Id == device.Id);

        private async Task OnOk()
        {
            try
            {
                await GetStoragePermissionsAsync();
                    var cloudDevices = CloudDeviceList?
                    .Where(x => x.IsSelected)?
                    .ToList() ?? new List<GrayWolfDevice>();

                    var devicesToConnect = BluetoothDeviceList?.Where(x => x.IsSelected)?.ToList() ?? new List<BleDevice>();
                    BleService.ConnectToDevices(devicesToConnect);

                    var allSelected = new List<GrayWolfDevice>();
                    allSelected.AddRange(cloudDevices);
                    var bleDevices = devicesToConnect
                        .Select(x => x.GrayWolfDevice)
                        .ToList();
                    allSelected.AddRange(bleDevices);
                    await DeviceService.SelectDevicesAsync(allSelected);

                    var devicesToDisconnect = BluetoothDeviceList?.Where(x => !x.IsSelected)?.ToList() ?? new List<BleDevice>();
                    BleService.DisconnectFromDevices(devicesToDisconnect);
                    await NavigationService.Instance.Nav.PopPopupAsync();
                }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
        }

        private async Task OnCancel()
        {
            await NavigationService.Instance.Nav.PopPopupAsync();
        }

        private async Task OnOpenLogin()
        {
            var loginPg = new LoginPage();
            loginPg.Disappearing += LoginPg_Disappearing;
            await Navigation.PushPopupAsync(loginPg);
        }

        private async void LoginPg_Disappearing(object sender, EventArgs e)
        {
            LoggedUser = AuthService.CurrentUserLogin;
            await Load();
        }


        public async Task Load()
        {

            try
            {
                if (!IsLoggedIn) return;
                IsBusy = true;
                await DeviceService.FetchCloudDevicesAsync();
                var devices = await DeviceService.GetCloudDevicesFromDBAsync();
                CloudDeviceList = new ObservableCollection<GrayWolfDevice>(devices);
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

        private void BleService_OnVisibleDevicesChanged(object sender, Utility.ScannedDevicesUpdatedEventArgs e)
        {
            OnBleDevicesUpdated(e.Devices);
        }

        private void OnBleDevicesUpdated(IEnumerable<BleDevice> devices)
        {
            lock ("BleCollectionFetch")
            {
                if (BluetoothDeviceList?.Any() == true)
                {
                    var newDevices = devices.Where(x => !IsDeviceInBluetoothList(x, BluetoothDeviceList)).ToList();
                    if (newDevices.Any())
                    {
                        BluetoothDeviceList.AddRange(newDevices);
                    }

                    var removedDevices = BluetoothDeviceList.Where(x => !IsDeviceInBluetoothList(x, devices)).ToList();
                    if (removedDevices.Any() && BluetoothDeviceList.Any())
                    {
                        BluetoothDeviceList.RemoveRange(removedDevices);
                    }
                }
                else
                {
                    BluetoothDeviceList = devices.ToObservableCollection();
                }
            }
        }

        public async Task<bool> GetStoragePermissionsAsync()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    var statuses = await Ioc.Default.GetService<IStoragePermissionService>().RequestStoragePermissions();
                    return statuses;
                case Device.iOS:
                    var status = await Permissions.RequestAsync<Permissions.StorageRead>();
                    if (status != PermissionStatus.Granted)
                    {
                        return false;
                    }
                    status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                    return status == PermissionStatus.Granted;
                default:
                    return false;
            }
        }

    }
}
