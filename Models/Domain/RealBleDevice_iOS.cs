//using Acr.Reactive;
//using GalaSoft.MvvmLight.Ioc;
//using GrayWolf.Converters;
//using GrayWolf.Enums;
//using GrayWolf.Helpers;
//using GrayWolf.Interfaces;
//using GrayWolf.Services;
//using GrayWolf.ViewModels;
//using MvvmHelpers;
//using Plugin.BluetoothLE;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Globalization;
//using System.Linq;
//using System.Reactive.Linq;
//using System.Runtime.CompilerServices;
//using System.Threading;
//using System.Threading.Tasks;


//namespace GrayWolf.Models.Domain
//{
//    public class RealBleDevice_iOS : RealBleDevice, IDisposable
//    {
//        private CancellationTokenSource DeviceStatusChangedTCS { get; set; }

//        private CancellationTokenSource ReadingCTS { get; set; }

//        public RealBleDevice_iOS(
//            IDevice nativeDevice,
//            IBleService bleService,
//            IDeviceService deviceService) : base(
//                bleService,
//                deviceService)
//        {
//            SetDevice(nativeDevice, false);
//        }

//        private void SetDevice(IDevice device, bool isSelected)
//        {
//            try
//            {
//                DeviceStatusChangedTCS?.Cancel();
//            }
//            catch (Exception ex) { }
//            if (device == null)
//                return;
//            Id = device.Uuid.ToString();
//            DeviceName = device.Name;
//            GrayWolfDevice = new GrayWolfDevice
//            {
//                DeviceName = device.Name,
//                DeviceID = Id,
//                Data = new List<Reading>(),
//                DeviceType = DeviceModelsEnum.Unknown,
//                Source = DeviceSource.Ble,
//                StatusEnum = ProbeStatus.UNKNOWN,
//                BatteryStatusEnum = BatteryStatus.UNKNOWN
//            };
//            NativeDevice = device;

//            DeviceStatusChangedTCS = new CancellationTokenSource();
//            NativeDevice
//                .WhenStatusChanged()
//                .Subscribe(OnDeviceStatusChanged, DeviceStatusChangedTCS.Token);
//            IsSelected = isSelected;
//        }

//        protected override async void OnPropertyChanged([CallerMemberName] string propertyName = "")
//        {
//            base.OnPropertyChanged(propertyName);
//            if(propertyName == nameof(IsConnected) || propertyName == nameof(IsSelected))
//            {
//                //await Device.InvokeOnMainThreadAsync(async() => await SubscribeIfReadyAsync());
//                await Microsoft.Maui.Controls.Device.InvokeOnMainThreadAsync(async () => await SubscribeIfReadyAsync());
//            }
//        }

//        private bool _isFetchStarted = false;
//        public override Task FetchDeviceAsync()
//        {
//            if (IsFetchFailed)
//            {
//                _isFetchStarted = false;
//            }
//            IsFetchFailed = false;
//            if (!_isFetchStarted)
//            {
//                _isFetchStarted = true;
//                Debug.WriteLine("Fetch started");
//                ReadProbe();
//            }
//            return Task.CompletedTask;
//        }

//        private void OnDatumUpdated(int channel, Reading datum)
//        {
//            datum.TimeStamp = DateTime.UtcNow;
//            Sensors[channel] = datum;
//            var data = Sensors.OrderBy(x => x.Key).Select(x => x.Value);
//            GrayWolfDevice.UpdateData(data);
//        }

//        private void UpdateReadingCTS()
//        {
//            try
//            {
//                ReadingCTS.Cancel();
//            }
//            catch (Exception) { }
//            ReadingCTS = new CancellationTokenSource();
//        }

//        #region read/write
//        private void ReadProbe()
//        {
//            try
//            {
//                if (!IsConnected || !IsFetchRunning)
//                {
//                    return;
//                }
//                Debug.WriteLine("Reading probe");
//                UpdateReadingCTS();
//                NativeDevice.ReadCharacteristic(ProbeGuid, ProbeCharacteristicGuid).Subscribe(OnProbeResult, ReadingCTS.Token);
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"Failed to read to probe: {ex.Message}");
//                IsFetchFailed = true;
//            }
//        }

//        private void SwitchChannel(int channel)
//        {
//            try
//            {
//                byte[] bytes = new byte[1] { 0x00 };
//                if (channel < 10) bytes = new byte[1] { (byte)(channel + 0x30) };
//                else if(channel < 100)
//                {
//                    var tens = channel / 10;
//                    bytes = new byte[2] { (byte)(tens + 0x30), (byte)((channel - tens * 10) + 0x30) };
//                }
//                Debug.WriteLine($"Switching to channel {channel}");
//                Channel = channel;
//                if (IsFetchRunning)
//                {
//                    UpdateReadingCTS();
//                    NativeDevice.WriteCharacteristic(SensorGuid, SensorChannelGuid, bytes).Subscribe(OnChannelSwitched, ReadingCTS.Token);
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"Failed to switch channel: {ex.Message}");
//                IsFetchFailed = true;
//            }
//        }
//        #endregion

//        #region callbacks
//        private void OnProbeResult(CharacteristicGattResult result)
//        {
//            try
//            {
//                if (UpdateProbe(result.Data))
//                {
//                    Channels = Convert.ToInt32(result.Data[5]);
//                }
//                SwitchChannel(0);
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"Failed to parse probe result: {ex.Message}");
//            }
//        }

//        private void OnDataResult(CharacteristicGattResult result)
//        {
//            try
//            {
//                var bytes = result.Data;

//                if (!bytes.Any())
//                {
//                    return;
//                }

//                var channel = Convert.ToInt32(bytes[0]);
//                if(channel != Channel && IsFetchRunning)
//                {
//                    UpdateReadingCTS();
//                    NativeDevice.ReadCharacteristic(SensorGuid, SensorDataGuid).Subscribe(OnDataResult, ReadingCTS.Token);
//                    return;
//                }

//                Debug.WriteLine($"OnDataResult: channel {channel}");
//                var reading = GetOrCreateReading(channel);

//                UpdateReading(reading, bytes);

//                OnDatumUpdated(channel, reading);
//              /// doesnt seem to work here BVW   SendLogButtonMessageIfClicked(bytes[0]);

//                if (IsFetchRunning)
//                {
//                    UpdateReadingCTS();
//                    NativeDevice.ReadCharacteristic(SensorGuid, SensorValuesGuid).Subscribe(OnValuesResult, ReadingCTS.Token);
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"Failed to parse sensor data: {ex.Message}");
//                IsFetchFailed = true;
//            }
//        }

//        private async void OnValuesResult(CharacteristicGattResult result)
//        {
//            try
//            {
//                var bytes = result.Data;
//                if (!bytes.Any())
//                {
//                    return;
//                }
//                int channel = Convert.ToInt32(bytes[0] & 0x3F);

//                // Bill VW - March 2022. Moved here because it was not working 
//                SendLogButtonMessageIfClicked(bytes[0]);

//                Debug.WriteLine($"OnValueResult: channel {channel}");
//                var reading = GetOrCreateReading(channel);
//                UpdateReadingValue(reading, bytes);
//                OnDatumUpdated(channel, reading);
//                var newChannel = channel + 1;
//                if (newChannel == Channels)
//                {
//                    await OnDeviceFetchCompletedAsync(GrayWolfDevice, NativeDevice.Status == ConnectionStatus.Connected);
//                    if (IsFetchRunning)
//                    {
//                        UpdateReadingCTS();
//                        NativeDevice.ReadCharacteristic(ProbeGuid, ProbeCharacteristicGuid).Subscribe(OnProbeResult, ReadingCTS.Token);
//                    }
//                }
//                else
//                {
//                    SwitchChannel(newChannel);
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"Failed to parse sensor value: {ex.Message}");
//                IsFetchFailed = true;
//            }
//        }

//        private void OnChannelSwitched(CharacteristicGattResult result)
//        {
//            try
//            {
//                if (IsFetchRunning)
//                {
//                    UpdateReadingCTS();
//                    NativeDevice.ReadCharacteristic(SensorGuid, SensorDataGuid).Subscribe(OnDataResult, ReadingCTS.Token);
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"Failed to read characteristic: {ex.Message}");
//                IsFetchFailed = true;
//            }
//        }

//        private void OnStelsBytesReceived(Datum datum, byte[] bytes)
//        {
//            if (!bytes.Any())
//            {
//                return;
//            }

//            //Not implemented yet
//        }
//        #endregion

//        #region connection
//        public override async Task ConnectAsync()
//        {
//            await ConnectWait(NativeDevice).Timeout(TimeSpan.FromSeconds(15));
//        }

//        private IObservable<IDevice> ConnectWait(IDevice device)
//        {
//            return Observable.Create<IDevice>(ob =>
//            {
//                var sub = device
//                    .WhenStatusChanged()
//                    .Subscribe(status =>
//                    {
//                        if (status == ConnectionStatus.Connected)
//                        {
//                            ob.Respond(device);
//                        }
//                    });

//                device.Connect();

//                return () => { sub.Dispose(); };
//            });
//        }
//        #endregion

//        #region observable
//        private CancellationTokenSource reconnectTCS;
//        private async void OnDeviceStatusChanged(ConnectionStatus status)
//        {
//            if (status == ConnectionStatus.Connected)
//            {
//                Debug.WriteLine("Device connected");
//                WasConnected = true;
//                IsReconnecting = false;
//                IsConnected = true;
//                OnPropertyChanged(nameof(IsConnected));
//                IsDisconnectedByUser = false;
//                GrayWolfDevice.IsOnline = true;
//                try
//                {
//                    reconnectTCS?.Cancel();
//                }
//                catch(Exception) { }
//                if (BleService is RealBleService realBleService)
//                {
//                    realBleService.SendDeviceStatusChanged(this, status);
//                }
//                reconnectTCS = null;
//            }

//            if (status == ConnectionStatus.Disconnected)
//            {
//                try
//                {
//                    //ReadingCTS.Cancel();
//                }
//                catch (Exception) { }
//                _isFetchStarted = false;
//                IsFetchRunning = false;
//                IsConnected = false;
//                if (!IsDisconnectedByUser && !IsReconnecting && WasConnected)
//                {
//                    try
//                    {
//                        await OnDeviceFetchCompletedAsync(GrayWolfDevice, NativeDevice.Status == ConnectionStatus.Connected);
//                        Debug.WriteLine($"Trying to reconnect to device({GrayWolfDevice?.DeviceDisplayName}) in {Constants.BLE_RESTORE_CONNECTION_TIMEOUT_MS}ms");
//                        IsReconnecting = true;
//                        NativeDevice.CancelConnection();
//                        reconnectTCS = reconnectTCS ?? new CancellationTokenSource();
//                        CrossBleAdapter.Current.ScanUntilDeviceFound(NativeDevice.Uuid).Subscribe(OnRestored, reconnectTCS.Token);
//                        await Task.Delay(TimeSpan.FromMilliseconds(Constants.BLE_RESTORE_CONNECTION_TIMEOUT_MS), reconnectTCS.Token);
//                        if(NativeDevice.Status != ConnectionStatus.Connected)
//                        {
//                            Debug.WriteLine($"Failed to reconnect to device({GrayWolfDevice?.DeviceDisplayName}), removing from the home page");
//                            GrayWolfDevice.IsSelected = false;
//                            IsSelected = false;
//                        }
//                        await OnDeviceFetchCompletedAsync(GrayWolfDevice, NativeDevice.Status == ConnectionStatus.Connected);
//                        if(NativeDevice.Status != ConnectionStatus.Connected)
//                            TrySendDisconnected();
//                        reconnectTCS?.Cancel();
//                    }
//                    catch (TaskCanceledException) 
//                    {

//                    }
//                    catch(Exception ex)
//                    {
//                    }
//                }
//                else if(IsDisconnectedByUser)
//                {
//                    try
//                    {
//                        reconnectTCS?.Cancel();
//                    }
//                    catch (TaskCanceledException) { }
//                    TrySendDisconnected();
//                }
//            }
//        }

//        private async Task SubscribeIfReadyAsync()
//        {
//            if (IsConnected && IsSelected)
//            {
//                GrayWolfDevice.IsSelected = true;
//                await SubscribeToDeviceUpdatesAsync();
//            }
//        }

//        private void TrySendDisconnected()
//        {
//            if (BleService is RealBleService realBleService)
//            {
//                realBleService.SendDeviceStatusChanged(this, ConnectionStatus.Disconnected);
//            }
//            WasConnected = false;
//        }

//        private async void OnRestored(IDevice device)
//        {
//            try
//            {
//                SetDevice(device, true);
//                await NativeDevice.ConnectWait().Timeout(TimeSpan.FromSeconds(15));
//            }
//            catch (Exception) { }
//        }

//        public override void Dispose()
//        {
//            try
//            {
//                DeviceStatusChangedTCS?.Cancel();
//            }
//            catch (TaskCanceledException) { }
//            DeviceStatusChangedTCS = null;
//            try
//            {
//                reconnectTCS?.Cancel();
//            }
//            catch (TaskCanceledException) { }
//            reconnectTCS = null;
//        }
//        #endregion
//    }
//}
