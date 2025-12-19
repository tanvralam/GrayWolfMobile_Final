using GrayWolf.Extensions;
using GrayWolf.Interfaces;
using GrayWolf.Services;
using MvvmHelpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace GrayWolf.Models.Domain
{
    public abstract class BleDevice : ObservableObject
    {
        private string _id;
        public string Id
        {
            get => _id;
            protected set => SetProperty(ref _id, value);
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private string _deviceName;
        public string DeviceName
        {
            get => _deviceName;
            protected set => SetProperty(ref _deviceName, value);
        }

        private bool _isFetchRunning;
        public bool IsFetchRunning
        {
            get => _isFetchRunning;
            protected set => SetProperty(ref _isFetchRunning, value);
        }

        private GrayWolfDevice _grayWolfDevice;
        public GrayWolfDevice GrayWolfDevice
        {
            get => _grayWolfDevice;
            protected set
            {
                SetProperty(ref _grayWolfDevice, value);
                DeviceUpdated?.Invoke(this, new EventArg<GrayWolfDevice>(value));
            }
        }

        public event EventHandler<EventArg<GrayWolfDevice>> DeviceUpdated;

        protected IBleService BleService { get; }
        protected IDeviceService DeviceService { get; }

        public BleDevice(IBleService bleService, IDeviceService deviceService)
        {
            BleService = bleService;
            DeviceService = deviceService;
        }

        public virtual Task SubscribeToDeviceUpdatesAsync()
        {
            if (IsFetchRunning)
            {
                return Task.CompletedTask;
            }
            IsFetchRunning = true;
            Debug.WriteLine("Started reading data from device");
            Device.StartTimer(TimeSpan.FromMilliseconds(5000), () =>
            {
                Task.Run(FetchDeviceAsync);
                return IsFetchRunning;
            });
            return Task.CompletedTask;
        }

        public virtual void UnsubscribeFromDeviceUpdates()
        {
            IsFetchRunning = false;
        }

        public abstract Task FetchDeviceAsync();

        public async Task OnDeviceFetchCompletedAsync(GrayWolfDevice device, bool isConnected)
        {
            Debug.WriteLine("Fetch completed, updating device data");
            await Device.InvokeOnMainThreadAsync(async() =>
            {
                try
                {
                    device.IsOnline = isConnected;
                    device.LastPing = DateTime.UtcNow;
                    GrayWolfDevice.UpdateData(device.Data);
                    if(!device.Data.Any(x => x.Id.IsNullOrEmpty() || x.OriginalUnit == null))
                    {
                        Debug.WriteLine("Updating db");
                        await DeviceService.UpdateDeviceInDBAsync(device, Enums.DeviceSource.Ble);
                    }
                }
                catch(Exception ex) 
                {
                    Debug.WriteLine(ex);
                }
            });
            await Task.Delay(1000);
        }
        
        public virtual void Connect()
        {
            BleService.ConnectToDevice(this);
        }

        public virtual void Disconnect()
        {
            UnsubscribeFromDeviceUpdates();
            BleService.DisconnectFromDevice(this);
        }
    }
}
