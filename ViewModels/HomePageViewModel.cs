using Acr.UserDialogs;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using GrayWolf.CustomControls;
using GrayWolf.Enums;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Messages;
using GrayWolf.Models.Domain;
using GrayWolf.Services;
using GrayWolf.Views;
using GrayWolf.Views.Popups;
using MvvmHelpers;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
//using XamarinUniversity.Infrastructure;

namespace GrayWolf.ViewModels
{
    public class HomePageViewModel : BaseMenuViewModel
    {
        private const string TAG = "HomePageViewModel";
        private const string LogIcon = "ic_trend_log";
        private const string StopLogIcon = "ic_stop_log";
        private static HomePageViewModel viewModel;
        #region ICommand  
        public ICommand MenuCommand { get; }
        public ICommand CheckBatteryStatusCommand { get; }
        public ICommand CameraCommand { get; }
        public ICommand RecordSoundCommand { get; }
        public ICommand LogCommand { get; }
        public ICommand SelectDevicesCommand { get; }
        public ICommand ProbeSelectedCommand { get; }
        public ICommand AddNoteCommand { get; }
        public ICommand OpenDrawingNoteCommand { get; }
        public ICommand VideoCommand { get; }
        public ICommand SensorClickCommand { get; }
        public ICommand ProbeTappedCommand { get; }
        #endregion

        #region PROPERTIES
        private bool _isSubscribed = false;

        private HomePageStatus _status;
        public HomePageStatus Status
        {
            get => _status;
            private set => SetProperty(ref _status, value);
        }

        private string _logButtonSource;
        public string LogButtonSource
        {
            get => _logButtonSource;
            private set
            {
                var formatted = Device.RuntimePlatform == Device.WinUI ? $"{value}.png" : value;
                SetProperty(ref _logButtonSource, formatted);
            }
        }

        private ObservableRangeCollection<GrayWolfDevice> _probeList;
        public ObservableRangeCollection<GrayWolfDevice> ProbeList
        {
            get { return _probeList; }
            set => SetProperty(ref _probeList, value);
        }

        private bool _logButtonEnabled;
        public bool LogButtonEnabled
        {
            get => _logButtonEnabled;
            private set => SetProperty(ref _logButtonEnabled, value);
        }

        private string _currentLogName;
        public string CurrentLogName
        {
            get => _currentLogName;
            private set => SetProperty(ref _currentLogName, value);
        }

        private Color _logNameColor;
        public Color LogNameColor
        {
            get => _logNameColor;
            private set => SetProperty(ref _logNameColor, value);
        }

        public bool CanSwitchToDemoMode => !AuthService.IsLoggedIn && ProbeList?.Any() != true;
        #endregion

        #region services
        private readonly IAuthService AuthService;
        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="HomePageViewModel"/> class.
        /// </summary>
        /// <param name="nav"></param>
        public HomePageViewModel()
        {
            LogButtonSource = LogIcon;
            LogButtonEnabled = false;
            viewModel = this;
            MenuCommand = new Command(OnMenu);
            CheckBatteryStatusCommand = new AsyncRelayCommand(OnCheckBatteryStatusAsync);
            CameraCommand = new Command(OnCameraAsync);
            RecordSoundCommand = new Command(OnRecordAsync);
            LogCommand = new AsyncRelayCommand(OnLogAsync);
            ProbeList = new ObservableRangeCollection<GrayWolfDevice>();
            SelectDevicesCommand = new AsyncRelayCommand(OnSelectDevices);
            OpenDrawingNoteCommand = new AsyncRelayCommand(OnOpenDrawingNote);
            AddNoteCommand = new AsyncRelayCommand(OnAddNote);
            VideoCommand = new Command(CaptureVideo);
            SensorClickCommand = new Command<Reading>(OpenSensorMenu);
            ProbeTappedCommand = new Command<GrayWolfDevice>(OnProbeTapped);
            AuthService = Ioc.Default.GetService<IAuthService>();
            Status = HomePageStatus.Loading;
            TrySubscribeToMessenger();
            DeviceService.Init();
            
        }

        

        
        #endregion

        #region override
        public override void OnAppearing()
        {
            base.OnAppearing();

            TrySubscribeToMessenger();
            RaisePropertyChanged(nameof(CanSwitchToDemoMode));
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            _isSubscribed = false;
        }

        public void UpdateCurrentLogName()
        {
            var log = LogService.CurrentFile;
            if (log != null)
            {
                LogNameColor = Colors.Black;
                CurrentLogName = LogService.CurrentFile?.Name;
            }
            else
            {
                LogNameColor = Colors.DimGray;
                CurrentLogName = Localization.Localization.HomePage_SelectLocation;
            }
        }

        protected override void RaiseLogStatusChanged()
        {
            base.RaiseLogStatusChanged();
            RaisePropertyChanged(nameof(IsLocationSelected));
            RaisePropertyChanged(nameof(AreGalleryAttachmentsEnabled));
            var log = LogService.CurrentFile;
            if (log != null)
            {
                LogNameColor = Colors.Black;
                CurrentLogName = LogService.CurrentFile?.Name;
            }
            else
            {
                LogNameColor = Colors.DimGray;
                CurrentLogName = Localization.Localization.HomePage_SelectLocation;
            }
            LogButtonEnabled = LogStatus != LogStatus.NoDevicesSelected;
            LogButtonSource = LogStatus == LogStatus.Logging ? StopLogIcon : LogIcon;
        }
        #endregion

        #region Methods
        private void TrySubscribeToMessenger()
        {
            if (_isSubscribed)
                return;
            _isSubscribed = true;
            MessengerInstance.Register<LogStatusChangedMessage>(this, OnLogStatusChanged);
            MessengerInstance.Register<SelectedDevicesMessage>(this, OnSelectedDevicesChanged);
            MessengerInstance.Register<DeviceConfigurationChangedMessage>(this, OnDeviceUpdated);
        }

        private void OnSelectedDevicesChanged(SelectedDevicesMessage message)
        {
            OnRefresh(message.Devices);
        }

        private async Task OnAddNote()
        {
            if (!SetBusy())
            {
                return;
            }
            await LogService.AddNote();
            IsBusy = false;
        }

        private async Task OnOpenDrawingNote()
        {
            if (!SetBusy())
            {
                return;
            }
            if (IsLocationSelected)
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        var statuses = await Ioc.Default.GetService<IStoragePermissionService>().RequestStoragePermissions();
                        if(!statuses)
                        {
                            IsBusy = false;
                            return;
                        }
                        break;
                    case Device.iOS:
                        var status = await Permissions.RequestAsync<Permissions.StorageRead>();
                        if (status != PermissionStatus.Granted)
                        {
                            IsBusy = false;
                            return;
                        }
                        status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                        if (status != PermissionStatus.Granted)
                        {
                            IsBusy = false;
                            return;
                        }
                        break;
                }

                await NavigationService.Instance.Nav.PushPopupAsync(new DrawingNotePopup());
            }
            else
            {
                await Alert.ShowAlert(Localization.Localization.DrawingNote_SelectLoggerMessage);
            }

            IsBusy = false;
        }

        private async Task OnSelectDevices()
        {
            if (!SetBusy())
            {
                return;
            }
            if (CanSelectDevice)
            {
                await NavigationService.Instance.Nav.PushPopupAsync(new SelectDevicesPopup());
            }
            else
            {
                await Alert.ShowAlert(Localization.Localization.SelectDevices_LogIsRunningMessage);
            }
            IsBusy = false;
        }

        /// <summary>
        ///  To Open Menu Page...
        /// </summary>
        private void OnMenu()
        {
            if (Application.Current.MainPage is FlyoutPage mdp)
            {
                mdp.IsPresented = true;
            }

        }

        /// <summary>
        /// To Show Log Activity...
        /// </summary>
        private async Task OnLogAsync()
        {
            if (!SetBusy())
            {
                return;
            }
            if (LogStatus == LogStatus.NoDevicesSelected)
            {
                await Alert.ShowAlert(Localization.Localization.StartLog_SelectDevicesMessage);
            }
            else
            {
                if (!LogService.IsLogging)
                {
                    await Navigation.PushPopupAsync(new StartLogPopupPage());
                }
                else
                {
                    int interval = LogService.CurrentFile.LoggingInterval / 1000;

                    string logMess = String.Format(Localization.Localization.TrendLog_Status,
                        Path.GetFileName(LogService.CurrentFile.Name), interval);

                    var stopLog = await Ioc.Default.GetService<IUserDialogs>().ConfirmAsync(logMess, Localization.Localization.Title_TrendLog, Localization.Localization.Button_StopLog);
                    if (stopLog)
                    {
                        await LogService.StopLog();
                    }
                }
            }
            IsBusy = false;
        }

        /// <summary>
        /// To Open Record Sound Popup Page...
        /// </summary>
        private async void OnRecordAsync()
        {
            if (!SetBusy())
            {
                return;
            }
            await LogService.RecordSound();
            IsBusy = false;
        }

        /// <summary>
        /// To Open Battery Status Popup Page...
        /// </summary>
        private async Task OnCheckBatteryStatusAsync()
        {
            if (!SetBusy())
            {
                return;
            }
            //Show Popup;
            var batteryPopup = new BatteryStatusPopupPage(this);
            await Navigation.PushPopupAsync(batteryPopup);
            IsBusy = false;
        }

        /// <summary>
        /// To Open Camera, Take Picture And Show It In Camera Popup Page...
        /// </summary>
        private async void OnCameraAsync()
        {
            if (!SetBusy())
            {
                return;
            }
            await LogService.TakePhoto();
            IsBusy = false;
        }

        private async void CaptureVideo()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                await LogService.AddVideo();
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }

            IsBusy = false;
        }

        private async void OpenSensorMenu(Reading reading)
        {
            if (!SetBusy())
            {
                return;
            }
            await Navigation.PushPopupAsync(new SensorPopupPage(reading.Id));

            IsBusy = false;
        }

        private async void OnProbeTapped(GrayWolfDevice device)
        {
            if (!device.CanOpenProbePage || !SetBusy())
            {
                return;
            }
           await Navigation.PushAsync(new ProbePage(device));

            IsBusy = false;
        }



        private void OnRefresh(IEnumerable<GrayWolfDevice> devices)
        {
            try
            {
                if (!(ProbeList?.Any() ?? true))
                {
                    ProbeList = new ObservableRangeCollection<GrayWolfDevice>(devices);
                    Status = ProbeList.Any()
                        ? HomePageStatus.DevicesListLoaded
                        : HomePageStatus.Empty;
                    return;
                }

                foreach (var device in devices)
                {
                    if (!TryUpdateDeviceInProbeList(device))
                    {
                        AddDeviceToProbeList(device);
                    }
                }

                var removedDevices = ProbeList
                    .Where(x => !devices.Any(y => y.DeviceID == x.DeviceID))
                    .ToList();

                ProbeList.RemoveRange(removedDevices);

                var orignalUnit = ProbeList[0].Data[0].OriginalUnit;
                var uiReading = ProbeList[0].UIReadings;

                Status = ProbeList.Any()
                    ? HomePageStatus.DevicesListLoaded
                    : HomePageStatus.Empty;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
        }



        private int GetIndexForInsert(IEnumerable<GrayWolfDevice> devices, GrayWolfDevice device)
        {
            var names = new List<string>()
            {
                device.DeviceName
            };
            var namesToAdd = devices.Select(x => x.DeviceName);
            names.AddRange(namesToAdd);
            var index = names
                .OrderBy(x => x)
                .ToList()
                .IndexOf(device.DeviceName);
            return index == -1 ? 0 : index;
        }

        private void OnDeviceUpdated(DeviceConfigurationChangedMessage message)
        {
            try
            {
                if (message?.Device == null)
                {
                    return;
                }
                var isListUpdated = true;
                if (!message.Device.IsSelected)
                {
                    isListUpdated = ProbeList?.Remove(message.Device) ?? false;
                }
                else if (!TryUpdateDeviceInProbeList(message.Device))
                {
                    AddDeviceToProbeList(message.Device);
                }

                if (isListUpdated)
                {
                    Status = ProbeList.Any() ? HomePageStatus.DevicesListLoaded : HomePageStatus.Empty;
                }

                var orignalUnit = ProbeList[0].Data[0].OriginalUnit;
                //var uiReading = ProbeList[0].UIReadings[0].ConvertedUnit;
                var s = "";
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
        }

        public static void UpdateUI()
        {
            viewModel.ProbeList.Clear();
        }

        private bool TryUpdateDeviceInProbeList(GrayWolfDevice device)
        {
            if (!(ProbeList.FirstOrDefault(p => p.DeviceID == device.DeviceID) is GrayWolfDevice existing))
            {
                return false;
            }
            existing.BatteryStatus = device.BatteryStatus;
            existing.BatteryStatusEnum = device.BatteryStatusEnum;
            existing.CalStatus = device.CalStatus;
            existing.DeviceName = device.DeviceName;
            existing.DeviceSerialNum = device.DeviceSerialNum;
            existing.DeviceTextForList = device.DeviceTextForList;
            existing.DeviceType = device.DeviceType;
            existing.DeviceTypeLabel = device.DeviceTypeLabel;
            existing.IsActive = device.IsActive;
            existing.IsDeleted = device.IsDeleted;
            existing.LastFolderUpdate = device.LastFolderUpdate;
            existing.LastPing = device.LastPing;
            existing.LatestReadings = device.LatestReadings;
            existing.LocationXML = device.LocationXML;
            existing.Notes = device.Notes;
            existing.Position = device.Position;
            existing.SecurityToken = device.SecurityToken;
            existing.SecurityTokenExpiration = device.SecurityTokenExpiration;
            existing.Source = device.Source;
            existing.Status = device.Status;
            existing.StatusEnum = device.StatusEnum;
            existing.SensorStatus = device.SensorStatus;
            existing.SensorStatusEnum = device.SensorStatusEnum;
            existing.Title = device.Title;
            existing.Uptime = device.Uptime;
            existing.IsOnline = device.IsOnline;

            existing.UpdateData(device.Data);
            return true;
        }

        private void AddDeviceToProbeList(GrayWolfDevice device)
        {
            var index = GetIndexForInsert(ProbeList, device);
            device.IsExpanded = true;
            ProbeList.Insert(index, device);
            Status = HomePageStatus.DevicesListLoaded;
        }
        #endregion
    }
}
