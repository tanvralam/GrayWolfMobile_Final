
using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GrayWolf.Converters;
using GrayWolf.Enums;
using GrayWolf.Extensions;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Messages;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;
using Plugin.BLE.Abstractions.EventArgs;
//using Plugin.BLE.iOS;
using Plugin.BLE.Abstractions.Extensions;



namespace GrayWolf.Services
{
    public class RealBleService : BleService
    {
        #region guid
        private Guid ProbeServiceGUID => new Guid("5adcfa03-1ae1-4b75-9667-d56bbad13008");

        private Guid SensorServiceGUID => new Guid("24cfd5b2-4663-435f-9634-52b08834c61b");

        protected List<Guid> DeviceServicesGuids => new List<Guid> { ProbeServiceGUID };
        #endregion

        #region scan handle properties
        private SemaphoreSlim Semaphore { get; }
        private CancellationTokenSource ScanCTS { get; set; }
        #endregion

        private IAdapter Adapter => CrossBluetoothLE.Current.Adapter;

        private readonly List<BleDevice> _visibleDevices = new List<BleDevice>();

        private readonly Dictionary<BleDevice, CancellationTokenSource> _reconnectionDevices = new Dictionary<BleDevice, CancellationTokenSource>();

        private readonly Dictionary<string, CancellationTokenSource> _reconnectionTokens = new Dictionary<string, CancellationTokenSource>();

        private List<GrayWolfDeviceDBO> _selectedDbDevices; 

        public RealBleService()
        {
            Semaphore = new SemaphoreSlim(1, 1);
        }

        //public override void Initialize()
        //{
        //    Adapter.WhenStatusChanged().Subscribe(WhenStatusChanged);
        //}

        public override void Initialize()
        {
            CrossBluetoothLE.Current.StateChanged += WhenStatusChanged;
            
            Adapter.DeviceDiscovered += OnScanResult;

        }

        private async void WhenStatusChanged(object sender, BluetoothStateChangedArgs e)
        {
            try
            {
                if (e.NewState == BluetoothState.Off)
                {
                    await StopScanAsync();
                    var reconnectionDevices = _visibleDevices.Where(x => x.IsFetchRunning && !_reconnectionDevices.Keys.Contains(x)).ToList();
                    reconnectionDevices.ForEach(x =>
                    {
                        _reconnectionDevices[x] = new CancellationTokenSource(TimeSpan.FromMilliseconds(Constants.BLE_RESTORE_CONNECTION_TIMEOUT_MS));
                        x.UnsubscribeFromDeviceUpdates(); //TODO check if this call is needed
                    });
                }
                else if (e.NewState == BluetoothState.On)
                {
                    Task.Run(TryRestoreConnectionAsync);
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG, new Dictionary<string, object>
                {
                    { "AdapterStatus", $"{e.NewState}" }
                });
            }
        }

        //private async void WhenStatusChanged(AdapterStatus status)
        //{
        //    try
        //    {
        //        if (status == AdapterStatus.PoweredOff)
        //        {
        //            await StopScanAsync();
        //            var reconnectionDevices = _visibleDevices.Where(x => x.IsFetchRunning && !_reconnectionDevices.Keys.Contains(x)).ToList();
        //            reconnectionDevices.ForEach(x =>
        //            {
        //                _reconnectionDevices[x] = new CancellationTokenSource(TimeSpan.FromMilliseconds(Constants.BLE_RESTORE_CONNECTION_TIMEOUT_MS));
        //                x.UnsubscribeFromDeviceUpdates(); //TODO check if this call is needed
        //            });
        //        }
        //        else if (status == AdapterStatus.PoweredOn)
        //        {
        //            Task.Run(TryRestoreConnectionAsync);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AnalyticsService.TrackError(ex, TAG, new Dictionary<string, object>
        //        {
        //            { "AdapterStatus", $"{status}" }
        //        });
        //    }
        //}

        #region scan

        private async Task<bool> RequestBluetoothPermissions()
        {
            // if (Device.RuntimePlatform == Device.iOS)
            // {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status != PermissionStatus.Granted)
            {
                Debug.WriteLine("Location permission is required for Bluetooth scanning.");
            }
            // }

            if (status != PermissionStatus.Granted)
            {
                return false;
            }

            return true;
        }
        public override async Task StartScanAsync()
        {
            //var hasBlePermissions=await RequestBluetoothPermissions();
            //if (!hasBlePermissions)
            //{
            //    return;
            //}

            var hasBlePermissions = await Ioc.Default.GetService<IPermissionsService>().RequestBlePermissions();
            if (!hasBlePermissions)
            {
                return;
            }

            await StopScanAsync();
            await Semaphore.WaitAsync();
            try
            {
                _selectedDbDevices = await GetSelectedDevicesFromDBAsync();
                foreach(var device in ConnectedDevices.Values)
                {
                    device.IsSelected = IsDeviceSelected(device);
                }

                CancelAllReconnections();
                _visibleDevices.Clear();
                _visibleDevices.AddRange(ConnectedDevices.Values.Where(x => x.IsConnected));
                InvokeVisibleDevicesChanged(_visibleDevices);

                if (ScanCTS == null)
                {
                    ScanCTS = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                }

                //var config = new ScanConfig
                //{
                //    ServiceUuids = DeviceServicesGuids,
                //    ScanType = BleScanType.LowLatency,
                //};



                //Adapter
                //    .ScanExtra(config, restart: true)
                //    .Subscribe(OnScanResult, ScanCTS.Token);

                Adapter.ScanMode = ScanMode.LowLatency;
                

                await Adapter.StartScanningForDevicesAsync(DeviceServicesGuids.ToArray(), cancellationToken: ScanCTS.Token);

                //await Adapter.StartScanningForDevicesAsync( cancellationToken: ScanCTS.Token);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public override async Task StopScanAsync()
        {
            try
            {
                _selectedDbDevices = null;
                await Semaphore.WaitAsync();
                await Adapter.StopScanningForDevicesAsync();
                try
                {
                    ScanCTS?.Cancel();
                }
                catch (Exception) { }
                ScanCTS = null;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
            }
            finally
            {
                Semaphore.Release();
            }
        }

        //private void OnScanResult(IScanResult result)
        //{
        //    OnDeviceDiscovered(result.Device);
        //}

        private void OnScanResult(Object? obj, DeviceEventArgs args)
        {
            OnDeviceDiscovered(args.Device);
        }


        private void OnDeviceDiscovered(IDevice device)
        {
            AddVisibleDevice(device);
            Debug.WriteLine($"Device discovered. Name: {device.Name}. Id: {device.Id}");
        }

        private void AddVisibleDevice(IDevice device)
        {
            try
            {
                if (device.Name.IsNullOrEmpty() || _visibleDevices.Any(x => $"{x.Id}" == $"{device.Id}"))
                {
                    return;
                }
                var bleDevice = GetBleDevice(device);
                bleDevice.IsSelected = IsDeviceSelected(bleDevice);
                _visibleDevices.Add(bleDevice);
                InvokeVisibleDevicesChanged(_visibleDevices);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG, new Dictionary<string, object>
                {
                    { "DeviceName", $"{device?.Name}" },
                    { "DeviceId", $"{device?.Id}" },
                    { "Status", $"{device?.State}" },
                    { "PairingStatus", $"{device?.BondState}" },
                });
            }
        }
        #endregion

        #region connection
        public override async void ConnectToDevice(BleDevice device)
        {
            try
            {
                device.GrayWolfDevice.IsExpanded = true;
                device.IsSelected = true;
                var realDevice = ParseRealBLEDevice(device);

                AnalyticsService.TrackEvent("Connecting to device", TAG, GetDeviceAnalyticsParameters(device));
                await realDevice.ConnectAsync();
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG, GetDeviceAnalyticsParameters(device));
            }
        }

        public override void DisconnectFromDevice(BleDevice device)
        {
            try
            {
                var key = device.Id;
                InvokeDeviceDisconnected(device, false);
                var realDevice = ParseRealBLEDevice(device);
                realDevice.IsDisconnectedByUser = true;
                var nativeDevice = realDevice.NativeDevice;
                //nativeDevice.CancelConnection();
                Adapter.DisconnectDeviceAsync(nativeDevice);
                AnalyticsService.TrackEvent("Manual disconnect from device", TAG, GetDeviceAnalyticsParameters(device));
                InvokeConnectedDevicesChanged();
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG, GetDeviceAnalyticsParameters(device));
            }
        }

        protected override void InvokeDeviceConnected(BleDevice device)
        {
            base.InvokeDeviceConnected(device);
            try
            {
                if (_reconnectionDevices.TryGetValue(device, out CancellationTokenSource cts))
                {
                    cts.Cancel();
                }
                else if (_reconnectionTokens.TryGetValue(device.Id, out cts))
                {
                    cts.Cancel();
                }
            }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
            }
            finally
            {
                _reconnectionDevices.Remove(device);
                _reconnectionTokens.Remove(device.Id);
            }
        }
        #endregion

        #region connection restoration
        private void CancelAllReconnections()
        {
            foreach(var key in _reconnectionDevices.Keys.ToList())
            {
                try
                {
                    if(_reconnectionDevices.TryGetValue(key, out var token))
                    {
                        token.Cancel();
                    }
                }
                catch (Exception) { }
            }
            foreach(var key in _reconnectionTokens.Keys.ToList())
            {
                try
                {
                    if(_reconnectionTokens.TryGetValue(key, out var token))
                    {
                        token.Cancel();
                    }
                }
                catch (Exception) { }
            }
            _reconnectionTokens.Clear();
            _reconnectionDevices.Clear();
        }

        protected override async Task TryRestoreConnectionAsync()
        {
            var source = $"{DeviceSource.Ble}";
            var allBleDevices = await Database.GetItemsAsync<GrayWolfDeviceDBO>(x => x.Source == source);
            var devices = await Database.GetItemsAsync<GrayWolfDeviceDBO>(x => x.Source == source && x.IsSelected);
            var status = CrossBluetoothLE.Current.State;
            if (status == BluetoothState.On)
            {
                var tasks = new List<Task>();
                foreach (var device in devices)
                {
                    tasks.Add(TryRestoreConnectionAsync(device));
                }
                await Task.WhenAll(tasks);
            }
        }

        private async Task TryRestoreConnectionAsync(GrayWolfDeviceDBO device)
        {
            try
            {
                //var timeSpan = TimeSpan.FromMilliseconds(Constants.BLE_RESTORE_CONNECTION_TIMEOUT_MS);
                //var lastTime = device.LastPing;
                //var failTime = lastTime.AddMilliseconds(Constants.BLE_RESTORE_CONNECTION_TIMEOUT_MS);
                //var compare = failTime.CompareTo(DateTime.UtcNow);
                //if(compare < 0)
                //{
                //    await DeselectDeviceAsync(device);
                //}
                //else
                //{
                //    var ticksLeft = failTime.Subtract(DateTime.UtcNow);
                //}
                var ticks = 5000;
                var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(ticks));
                var now = DateTime.Now;
                _reconnectionTokens[device.Id] = cts;
                try
                { 
                    await Adapter.StartScanningForDevicesAsync(new[] { new Guid(device.Id) }, null, false, cts.Token);

                    Adapter.DeviceDiscovered += (sender, args) =>
                    {
                        if (args.Device.Id == new Guid(device.Id))
                        {
                            OnDeviceRestored(args.Device);
                            Adapter.StopScanningForDevicesAsync(); // Stop scanning once the device is found
                        }
                    };


                    //Adapter.ScanUntilDeviceFound(new Guid(device.Id)).Subscribe(OnDeviceRestored, cts.Token);
                    await Task.Delay(ticks, cts.Token);
                }
                catch (TaskCanceledException) 
                {
                }
                try
                {
                    var timeSpent = DateTime.Now.Subtract(now);
                    if (timeSpent.TotalMilliseconds >= ticks)
                        await DeselectDeviceAsync(device);
                }
                catch (Exception) { }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG, new Dictionary<string, object>
                {
                    { "DeviceName", $"{device.DeviceName}" },
                    { "DeviceId", $"{device.Id}" }
                });
                await DeselectDeviceAsync(device);
            }
        }

        private async Task DeselectDeviceAsync(GrayWolfDeviceDBO device)
        {
            device.IsSelected = false;
            //var transaction = await Database.BeginTransactionAsync();
            await Database.UpdateAsync(device);
            //Database.Commit(transaction);
            await Microsoft.Maui.Controls.Device.InvokeOnMainThreadAsync(() =>
            {
                Messenger.Default.Send(new DeviceConfigurationChangedMessage
                {
                    Device = device.ToGrayWolfDevice()
                });
            });
        }

        private void OnDeviceRestored(IDevice nativeDevice)
        {
            try
            {
                var key = $"{nativeDevice.Id}";
                if (!ConnectedDevices.ContainsKey(key))
                {
                    ConnectedDevices[key] = GetBleDevice(nativeDevice);
                }
                var bleDevice = ConnectedDevices[nativeDevice.Id.ToString()];
                var realDevice = ParseRealBLEDevice(bleDevice);
                ConnectToDevice(bleDevice);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG, GetDeviceAnalyticsParameters(GetBleDevice(nativeDevice)));
            }
        }
        #endregion

        private RealBleDevice ParseRealBLEDevice(BleDevice device)
        {
            if (!(device is RealBleDevice realDevice))
            {
                throw new ArgumentException();
            }
            return realDevice;
        }

        public RealBleDevice GetBleDevice(IDevice device)
        {
            var deviceService =Ioc.Default.GetService<IDeviceService>();


            //if (DeviceInfo.Platform == DevicePlatform.iOS)
            //{
            //    return new RealBleDevice_iOS(device, this, deviceService);
            //}
            //else 
            //{
                return new RealBleDevice_Droid(device, this, deviceService);
            //}

            
        }

        private async Task<bool> CheckBlePermissionsAsync()
        {
            if (DeviceInfo.Platform != DevicePlatform.Android)
            {
                return true;
            }

            var permission = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            return permission == PermissionStatus.Granted;
        }

        public void SendDeviceStatusChanged(BleDevice device, DeviceState status)
        {
            switch (status)
            {
                case DeviceState.Connected:
                    ConnectedDevices[device.Id] = device;
                    if(_reconnectionTokens.TryGetValue(device.Id, out var reconnectionTCS))
                    {
                        try
                        {
                            reconnectionTCS.Cancel();
                        }
                        catch (Exception) { }
                        _reconnectionTokens.Remove(device.Id);
                    }
                    break;
                case DeviceState.Disconnected:
                    InvokeDeviceDisconnected(device, false);
                    break;
                default:
                    break;
            }
        }

        protected override void InvokeDeviceDisconnected(BleDevice device, bool isWaitingForReconnection)
        {
            ConnectedDevices.Remove(device.Id);
            if (device is RealBleDevice realDevice)
            {
                realDevice.Dispose();
            }
            base.InvokeDeviceDisconnected(device, isWaitingForReconnection);

        }

        private Task<List<GrayWolfDeviceDBO>> GetSelectedDevicesFromDBAsync()
        {
            var source = $"{DeviceSource.Ble}";
            return Database.GetItemsAsync<GrayWolfDeviceDBO>(x => x.IsSelected && x.Source == source);
        }

        private bool IsDeviceSelected(BleDevice device) => _selectedDbDevices?.Any(x => x.Id == $"{device.Id}" && x.IsSelected) ?? false;
    }
}
