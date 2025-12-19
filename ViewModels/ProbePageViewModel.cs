using CommunityToolkit.Mvvm.Input;
using GrayWolf.Messages;
using GrayWolf.Models.Domain;
using GrayWolf.Views.Popups;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;



namespace GrayWolf.ViewModels
{
    public class ProbePageViewModel : BaseViewModel
    {
        #region variables
        private GrayWolfDevice _probe;
        public GrayWolfDevice Probe
        {
            get => _probe;
            private set => SetProperty(ref _probe, value);
        }

        public bool AnySensors => Probe?.Data?.Any() == true;
        #endregion

        #region commands
        public ICommand SensorCommand { get; }
        public ICommand MenuCommand { get; }
        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="ProbePageViewModel"/> class.
        /// </summary>
        /// <param name="nav"></param>
        public ProbePageViewModel(GrayWolfDevice device)
        {
            MenuCommand = new Command(OnMenuAsync);
            SensorCommand = new Command<Reading>(GoToSensorMenu);
            Probe = device;
        }

        #endregion

        private ObservableCollection<GrayWolfDevice> _probeList;
        public ObservableCollection<GrayWolfDevice> ProbeList
        {
            get { return _probeList; }
            set
            {
                if (_probeList == value) return;
                _probeList = value;
                OnPropertyChanged();
            }
        }

        #region PROPERTIES 

        #endregion

        #region override
        public override void OnAppearing()
        {   
            base.OnAppearing();
            MessengerInstance.Register<DeviceConfigurationChangedMessage>(this, OnDeviceUpdated);
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            MessengerInstance.Unregister(this);
        }

        public override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.RaisePropertyChanged(propertyName);
        }
        #endregion

        #region Methods

        /// <summary>
        /// TODO : To Open Menu Page...
        /// </summary>
        /// <param name="obj"></param>
        private void OnMenuAsync(object obj)
        {
            if (App.Current.MainPage is FlyoutPage mdp)
            {
                mdp.IsPresented = true;
            }
        }

        private async void OnDeviceUpdated(DeviceConfigurationChangedMessage message)
        {
            try
            {
                if (message.Device.DeviceID == Probe?.DeviceID)
                {
                    if (!message.Device.IsSelected)
                    {
                        if (IsBusy)
                        {
                            return;
                        }
                        IsBusy = true;
                        await Alert.ShowAlert(Localization.Localization.Probe_DisconnectMsg);
                        await OnBacksAsync();
                        return;
                    }
                    Probe.Uptime = message.Device.Uptime;
                    Probe.Status = message.Device.Status;
                    Probe.SensorStatus = message.Device.SensorStatus;
                    Probe.StatusEnum = message.Device.StatusEnum;

                    Probe.Status = Enums.ProbeStatusToTextConverter.ToFriendlyText(message.Device.StatusEnum);

                    Probe.SensorStatusEnum = message.Device.SensorStatusEnum;
                    Probe.BatteryStatus = message.Device.BatteryStatus;
                    Probe.BatteryStatusEnum = message.Device.BatteryStatusEnum;
                    Probe.IsOnline = message.Device.IsOnline;
                    Probe.UpdateData(message.Device.Data);
                }
                RaisePropertyChanged(nameof(AnySensors));
            }
            catch(Exception ex)
            {
                await Alert.DisplayError(ex);
                await OnBacksAsync();
            }
        }

        private async void GoToSensorMenu(Reading reading)
        {
            if (!SetBusy())
            {
                return;
            }

            await Navigation.PushPopupAsync(new SensorPopupPage(reading.Id));

            IsBusy = false;
        }
        #endregion
    }
}
