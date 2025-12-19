using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GrayWolf.Converters;
using GrayWolf.Enums;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Messages;
using GrayWolf.Models.Domain;
using GrayWolf.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace GrayWolf.Services
{
    public abstract class BleService : IBleService
    {
        protected string TAG => GetType().Name;

        public event EventHandler<ScannedDevicesUpdatedEventArgs> OnVisibleDevicesChanged;
        protected Dictionary<string, BleDevice> ConnectedDevices { get; } = new Dictionary<string, BleDevice>();

        protected IDatabase Database { get; }

        protected IAnalyticsService AnalyticsService { get; }

        protected ISensorsService SensorsService { get; }

        public BleService()
        {
            Database = Ioc.Default.GetService<IDatabase>();
            SensorsService = Ioc.Default.GetService<ISensorsService>();
            AnalyticsService = Ioc.Default.GetService<IAnalyticsService>();
        }

        public abstract void Initialize();

        public abstract void ConnectToDevice(BleDevice device);

        public void ConnectToDevices(List<BleDevice> devices)
        {
            foreach (var device in devices)
            {
                ConnectToDevice(device);
            }
        }

        public abstract void DisconnectFromDevice(BleDevice device);

        public void DisconnectFromDevices(List<BleDevice> devices)
        {
            foreach (var device in devices)
            {
                DisconnectFromDevice(device);
            }
        }

        public abstract Task StartScanAsync();

        public abstract Task StopScanAsync();

        protected virtual void InvokeDeviceConnected(BleDevice device)
        {
            Device.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    device.GrayWolfDevice.IsSelected = true;
                    Debug.WriteLine($"Connected to device. Name: {device.DeviceName}. Id: {device.Id}");
                    ConnectedDevices[$"{device.Id}"] = device;
                    await device.SubscribeToDeviceUpdatesAsync();
                    InvokeConnectedDevicesChanged();
                    await Database.UpdateAsync(device.GrayWolfDevice.ToGrayWolfDeviceDBO());
                }
                catch (Exception ex)
                {
                    AnalyticsService.TrackError(ex, TAG, GetDeviceAnalyticsParameters(device));
                }
            });
        }

        protected virtual void InvokeDeviceDisconnected(BleDevice device, bool isWaitingForReconnection)
        {
            var key = device?.Id;
            try
            {
                Debug.WriteLine($"Disconnected from device. Name: {device.DeviceName}. Id: {key}");

                device.UnsubscribeFromDeviceUpdates();
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG, GetDeviceAnalyticsParameters(device));
            }
        }

        protected abstract Task TryRestoreConnectionAsync();

        protected virtual void InvokeVisibleDevicesChanged(IEnumerable<BleDevice> devices)
        {
            OnVisibleDevicesChanged?.Invoke(this, new ScannedDevicesUpdatedEventArgs(devices));
        }

        protected virtual void InvokeConnectedDevicesChanged()
        {
            Messenger.Default.Send(new DeviceUpdateMsg
            {
                Source = DeviceSource.Ble,
                Devices = ConnectedDevices.Values.Select(x => x.GrayWolfDevice).ToList()
            },
            Constants.BLE_CONNECTED_DEVICES_UPDATED);
        }

        protected Dictionary<string, object> GetDeviceAnalyticsParameters(BleDevice device)
        {
            return new Dictionary<string, object>
            {
                { "DeviceName", $"{device?.DeviceName}" },
                { "DeviceUuid", $"{device?.Id}" },
                { "IsConnected", $"{device?.IsConnected ?? false}" },
                { "IsSelected", $"{device?.IsSelected ?? false}" },
                { "IsFetchRunning", $"{device?.IsFetchRunning ?? false}" }
            };
        }
    }
}
