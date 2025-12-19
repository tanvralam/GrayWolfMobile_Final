//using Acr.Reactive;
using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Converters;
using GrayWolf.Enums;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Services;
using GrayWolf.ViewModels;
using MvvmHelpers;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;

//using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;


namespace GrayWolf.Models.Domain
{
    public class RealBleDevice_Droid : RealBleDevice
    {
        private ISensorsService SensorsService { get; }


        private IBleService BleService { get; }

        private CancellationTokenSource DeviceStatusChangedTCS { get; set; }

        public RealBleDevice_Droid(
            IDevice nativeDevice,
            IBleService bleService,
            IDeviceService deviceService) : base(
                bleService,
                deviceService)
        {
            SensorsService = Ioc.Default.GetService<ISensorsService>();
            BleService = bleService;
            SetDevice(nativeDevice, false);
        }

        private void SetDevice(IDevice device, bool isSelected)
        {
            try
            {
                DeviceStatusChangedTCS?.Cancel();
            }
            catch (Exception ex) { }
if (device == null)
                return;
            Id = device.Id.ToString();
            DeviceName = device.Name;
            GrayWolfDevice = new GrayWolfDevice
            {
                DeviceName = device.Name,
                DeviceID = Id,
                Data = new List<Reading>(),
                DeviceType = DeviceModelsEnum.Unknown,
                Source = DeviceSource.Ble,
                StatusEnum = ProbeStatus.UNKNOWN,
                BatteryStatusEnum = BatteryStatus.UNKNOWN,
            };
            NativeDevice = device;

            DeviceStatusChangedTCS = new CancellationTokenSource();

            CrossBluetoothLE.Current.Adapter.DeviceConnected += Adapter_DeviceConnected;
            CrossBluetoothLE.Current.Adapter.DeviceDisconnected+= Adapter_DeviceConnected;
            //NativeDevice
            //    .WhenStatusChanged()
            //    .Subscribe(OnDeviceStatusChanged, DeviceStatusChangedTCS.Token);



            IsSelected = isSelected;
        }

        private void Adapter_DeviceConnected(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            OnDeviceStatusChanged(e.Device.State);
        }

        protected override async void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);
            if(propertyName == nameof(IsConnected) || propertyName == nameof(IsSelected))
            {
                await Microsoft.Maui.Controls.Device.InvokeOnMainThreadAsync(async() => await SubscribeIfReadyAsync());
            }
        }

        private bool _isFetchStarted = false;
        public override Task FetchDeviceAsync()
        {
            if (IsFetchFailed)
            {
                _isFetchStarted = false;
            }
            IsFetchFailed = false;
            if (!_isFetchStarted)
            {
                _isFetchStarted = true;
                Debug.WriteLine("Fetch started");
                return ReadProbeAsync();
            }
            return Task.CompletedTask;
        }

        private void OnDatumUpdated(int channel, Reading datum)
        {
            datum.TimeStamp = DateTime.UtcNow;
            Sensors[channel] = datum;
            var data = Sensors.OrderBy(x => x.Key).Select(x => x.Value);
            GrayWolfDevice.UpdateData(data);
        }

        #region read/write
        private async Task ReadProbeAsync()
        {
            try
            {
                if (!IsConnected || !IsFetchRunning)
                {
                    //BleService.DisconnectFromDevice(BleService.);
                    return;
                }
                Debug.WriteLine("Reading probe");
               // var result = await NativeDevice.ReadCharacteristic(ProbeGuid, ProbeCharacteristicGuid).SingleAsync();

                var service = await NativeDevice.GetServiceAsync(ProbeGuid);
                var characteristic = await (await service.GetCharacteristicAsync(ProbeCharacteristicGuid)).ReadAsync();


                OnProbeResult(characteristic);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to read to probe: {ex.Message}");
                IsFetchFailed = true;
            }
        }

        private async void SwitchChannel(int channel)
        {
            try
            {
                byte[] bytes = new byte[1] { 0x00 };
                if (channel < 10) bytes = new byte[1] { (byte)(channel + 0x30) };
                else if(channel < 100)
                {
                    var tens = channel / 10;
                    bytes = new byte[2] { (byte)(tens + 0x30), (byte)((channel - tens * 10) + 0x30) };
                }

                Debug.WriteLine($"<<<<<<<<<<<<<< Switching to channel {channel}");

                Channel = channel;
                
                //OnChannelSwitched(await NativeDevice.WriteCharacteristic(SensorGuid, SensorChannelGuid, bytes).SingleAsync());
               
                var service = await NativeDevice.GetServiceAsync(SensorGuid);
                var characteristicObj=await service.GetCharacteristicAsync(SensorChannelGuid);
                 await characteristicObj.WriteAsync(bytes);
               // var characteristic = await characteristicObj.ReadAsync();
                OnChannelSwitched();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to switch channel: {ex.Message}");
                IsFetchFailed = true;
            }
        }
        #endregion]

        #region callbacks
        //private void OnProbeResult(CharacteristicGattResult result)
        //{
        //    try
        //    {
        //        if (UpdateProbe(result.Data))
        //        {
        //            Channels = Convert.ToInt32(result.Data[5]);
        //        }

        //        SwitchChannel(0);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Failed to parse probe result: {ex.Message}");
        //    }
        //}


        private void OnProbeResult((byte[] data, int resultCode) characteristic)
        {
            try
            {
                if (UpdateProbe(characteristic.data))
                {
                    Channels = Convert.ToInt32(characteristic.data[5]);
                }

                SwitchChannel(0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to parse probe result: {ex.Message}");
            }
        }


        //private async void OnDataResult(CharacteristicGattResult result)
        //{
        //    try
        //    {
        //        var bytes = result.Data;

        //        if (!bytes.Any())
        //        {
        //            return;
        //        }

        //        var channel = Convert.ToInt32(bytes[0] & 0x3F);
        //        if(channel != Channel)
        //        {
        //            OnDataResult(await NativeDevice.ReadCharacteristic(SensorGuid, SensorDataGuid).SingleAsync());
        //            return;
        //        }

        //        Debug.WriteLine($"OnDataResult: channel {channel}");
        //        var reading = GetOrCreateReading(channel);

        //        UpdateReading(reading, bytes);

        //        OnDatumUpdated(channel, reading);


        //        // does not appear to work here // SendLogButtonMessageIfClicked(bytes[0]);


        //        if (IsFetchRunning)
        //        {
        //            OnValuesResult(await NativeDevice.ReadCharacteristic(SensorGuid, SensorValuesGuid).SingleAsync());
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Failed to parse sensor data: {ex.Message}");
        //        IsFetchFailed = true;
        //    }
        //}


        private async void OnDataResult((byte[] data, int resultCode) characteristic)
        {
            try
            {
                var bytes = characteristic.data;

                if (!bytes.Any())
                {
                    return;
                }

                var channel = Convert.ToInt32(bytes[0] & 0x3F);
                if (channel != Channel)
                {
                    var service = await NativeDevice.GetServiceAsync(SensorGuid);
                    var Newcharacteristic = await (await service.GetCharacteristicAsync(SensorValuesGuid)).ReadAsync();
                    OnDataResult(Newcharacteristic);
                    return;
                }

                Debug.WriteLine($"OnDataResult: channel {channel}");
                var reading = GetOrCreateReading(channel);

                UpdateReading(reading, bytes);

                OnDatumUpdated(channel, reading);


                // does not appear to work here // SendLogButtonMessageIfClicked(bytes[0]);


                if (IsFetchRunning)
                {
                    var service = await NativeDevice.GetServiceAsync(SensorGuid);
                    var newcharacteristic = await (await service.GetCharacteristicAsync(SensorValuesGuid)).ReadAsync();
                    OnValuesResult(newcharacteristic);
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to parse sensor data: {ex.Message}");
                IsFetchFailed = true;
            }
        }


        //private async void OnValuesResult(CharacteristicGattResult result)
        //{
        //    try
        //    {
        //        var bytes = result.Data;
        //        if (!bytes.Any())
        //        {
        //            return;
        //        }
        //        var channel = Convert.ToInt32(bytes[0] & 0x3F);

        //        // Bill VW - March 2022. Moved here because it was not working 
        //        SendLogButtonMessageIfClicked(bytes[0]);



        //        Debug.WriteLine($"OnValueResult: channel {channel}");
        //        var reading = GetOrCreateReading(channel);
        //        UpdateReadingValue(reading, bytes);
        //        OnDatumUpdated(channel, reading);
        //        if (!IsFetchRunning)
        //        {
        //            return;
        //        }

        //        bool isComplete;
        //        var newChannel = NextChannel(out isComplete);
        //        if (isComplete)
        //        {
        //            await OnDeviceFetchCompletedAsync(GrayWolfDevice, NativeDevice.Status == ConnectionStatus.Connected);
        //            OnProbeResult(await NativeDevice.ReadCharacteristic(ProbeGuid, ProbeCharacteristicGuid).SingleAsync());
        //        }
        //        else
        //        {
        //            SwitchChannel(newChannel);
        //        }



        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Failed to parse sensor value: {ex.Message}");
        //        IsFetchFailed = true;
        //    }
        //}


        private async void OnValuesResult((byte[] data, int resultCode) characteristic)
        {
            try
            {
                var bytes = characteristic.data;
                if (!bytes.Any())
                {
                    return;
                }
                var channel = Convert.ToInt32(bytes[0] & 0x3F);

                // Bill VW - March 2022. Moved here because it was not working 
                SendLogButtonMessageIfClicked(bytes[0]);



                Debug.WriteLine($"OnValueResult: channel {channel}");
                var reading = GetOrCreateReading(channel);
                UpdateReadingValue(reading, bytes);
                OnDatumUpdated(channel, reading);
                if (!IsFetchRunning)
                {
                    return;
                }

                bool isComplete;
                var newChannel = NextChannel(out isComplete);
                if (isComplete)
                {
                    await OnDeviceFetchCompletedAsync(GrayWolfDevice, NativeDevice.State == DeviceState.Connected);
                    var service = await NativeDevice.GetServiceAsync(ProbeGuid);
                    var newcharacteristic = await (await service.GetCharacteristicAsync(ProbeCharacteristicGuid)).ReadAsync();
                    OnProbeResult(newcharacteristic);
                }
                else
                {
                    SwitchChannel(newChannel);
                }



            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to parse sensor value: {ex.Message}");
                IsFetchFailed = true;
            }
        }


        //private async void OnChannelSwitched(CharacteristicGattResult result)
        //{
        //    try
        //    {
        //        if (IsFetchRunning)
        //        {
        //            OnDataResult(await NativeDevice.ReadCharacteristic(SensorGuid, SensorDataGuid).SingleAsync());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Failed to read characteristic: {ex.Message}");
        //        IsFetchFailed = true;
        //    }
        //}

        private async void OnChannelSwitched()
        {
            try
            {
                if (IsFetchRunning)
                {
                    var service = await NativeDevice.GetServiceAsync(SensorGuid);
                    var newcharacteristic = await (await service.GetCharacteristicAsync(SensorDataGuid)).ReadAsync();
                    OnDataResult(newcharacteristic);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to read characteristic: {ex.Message}");
                IsFetchFailed = true;
            }
        }


        private void OnStelsBytesReceived(Datum datum, byte[] bytes)
        {
            if (!bytes.Any())
            {
                return;
            }

            //Not implemented yet
        }
        #endregion

        #region get values
        private Reading GetOrCreateReading(int channel)
        {
            if (!Sensors.TryGetValue(channel, out Reading datum))
            {
                datum = new Reading
                {
                    Channel = channel,
                    OriginalUnit = new SensorUnit(),
                    DeviceId = GrayWolfDevice.DeviceID
                };
                Sensors[channel] = datum;
            }
            datum.TimeStamp = DateTime.UtcNow;
            return datum;
        }

        private SensorStatus GetSensorStatus(byte statusByte)
        {
            var status = Convert.ToInt32(statusByte);
            return Enum.IsDefined(typeof(SensorStatus), status) ? (SensorStatus)status : SensorStatus.ERROR;
        }

        private ProbeStatus GetProbeStatus(byte statusByte)
        {
            var status = Convert.ToInt32(statusByte);
            return Enum.IsDefined(typeof(ProbeStatus), status) ? (ProbeStatus)status : ProbeStatus.UNKNOWN;
        }

        private BatteryStatus GetBatteryStatus(byte statusByte)
        {
            var status = Convert.ToInt32(statusByte);
            return Enum.IsDefined(typeof(BatteryStatus), status) ? (BatteryStatus)status : BatteryStatus.UNKNOWN;
        }

        private string GetDeviceSerialNum(byte preByte, byte[] postBytes)
        {
            var preString = Convert.ToInt32(preByte);
            var post = (postBytes[0] << 8) + postBytes[1];
            var serialNum = $"{preString:00}-{post}";

            return serialNum;
        }

        private long GetUptime(byte[] bytes)
        {
            var seconds = (uint)BitConverter.ToInt32(bytes, 0);
            return seconds;
        }
        #endregion

        #region connection
        public override async Task ConnectAsync()
        {
            // await ConnectWait(NativeDevice).Timeout(TimeSpan.FromSeconds(15));
            await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(NativeDevice);
        }

        

        //private IObservable<IDevice> ConnectWait(IDevice device)
        //{
        //    return Observable.Create<IDevice>(ob =>
        //    {
        //        var sub = device
        //            .WhenStatusChanged()
        //            .Subscribe(status =>
        //            {
        //                if (status == ConnectionStatus.Connected)
        //                {
        //                    ob.Respond(device);
        //                }
        //            });

        //        device.Connect();

        //        return () => { sub.Dispose(); };
        //    });
        //}




        #endregion

        #region observable
        private CancellationTokenSource reconnectTCS;
        private async void OnDeviceStatusChanged(DeviceState status)
        {
            if (status == DeviceState.Connected)
            {
                Debug.WriteLine("Device connected");
                WasConnected = true;
                IsReconnecting = false;
                IsConnected = true;
                OnPropertyChanged(nameof(IsConnected));
                IsDisconnectedByUser = false;
                GrayWolfDevice.IsOnline = true;
                try
                {
                    reconnectTCS?.Cancel();
                }
                catch(Exception) { }
                if (BleService is RealBleService realBleService)
                {
                    realBleService.SendDeviceStatusChanged(this, status);
                }
                reconnectTCS = null;
            }

            if (status == DeviceState.Disconnected)
            {
                _isFetchStarted = false;
                IsFetchRunning = false;
                IsConnected = false;
                if (!IsDisconnectedByUser && !IsReconnecting && WasConnected)
                {
                    try
                    {
                        await OnDeviceFetchCompletedAsync(GrayWolfDevice, NativeDevice.State == DeviceState.Connected);
                        Debug.WriteLine($"Trying to reconnect to device({GrayWolfDevice?.DeviceDisplayName}) in {Constants.BLE_RESTORE_CONNECTION_TIMEOUT_MS}ms");
                        IsReconnecting = true;
                        // NativeDevice.CancelConnection();
                        await CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(NativeDevice);
                        reconnectTCS = reconnectTCS ?? new CancellationTokenSource();
                       

                        var adapter = CrossBluetoothLE.Current.Adapter;
                        await adapter.StartScanningForDevicesAsync(new[] { NativeDevice.Id }, null,false,reconnectTCS.Token);

                        adapter.DeviceDiscovered += (sender, args) =>
                        {
                            if (args.Device.Id == NativeDevice.Id)
                            {
                                OnRestored(args.Device);
                                adapter.StopScanningForDevicesAsync(); // Stop scanning once the device is found
                            }
                        };

                        await Task.Delay(TimeSpan.FromMilliseconds(Constants.BLE_RESTORE_CONNECTION_TIMEOUT_MS), reconnectTCS.Token);
                        if(NativeDevice.State != DeviceState.Connected)
                        {
                            Debug.WriteLine($"Failed to reconnect to device({GrayWolfDevice?.DeviceDisplayName}), removing from the home page");
                            GrayWolfDevice.IsSelected = false;
                            IsSelected = false;
                        }
                        await OnDeviceFetchCompletedAsync(GrayWolfDevice, NativeDevice.State == DeviceState.Connected);
                        if(NativeDevice.State != DeviceState.Connected)
                            TrySendDisconnected();
                        reconnectTCS?.Cancel();
                    }
                    catch (TaskCanceledException) 
                    {

                    }
                    catch(Exception ex)
                    {
                    }
                }
                else if(IsDisconnectedByUser)
                {
                    try
                    {
                        reconnectTCS?.Cancel();
                    }
                    catch (TaskCanceledException) { }
                    TrySendDisconnected();
                }
            }
        }

        private async Task SubscribeIfReadyAsync()
        {
            if (IsConnected && IsSelected)
            {
                GrayWolfDevice.IsSelected = true;
                await SubscribeToDeviceUpdatesAsync();
            }
        }

        //private void TrySendDisconnected()
        //{
        //    if (BleService is RealBleService realBleService)
        //    {
        //        realBleService.SendDeviceStatusChanged(this, ConnectionStatus.Disconnected);
        //    }
        //    WasConnected = false;
        //}

        private void TrySendDisconnected()
        {
            if (BleService is RealBleService realBleService)
            {
                realBleService.SendDeviceStatusChanged(this, DeviceState.Disconnected);
            }
            WasConnected = false;
        }

        //private async void OnRestored(IDevice device)
        //{
        //    try
        //    {
        //        SetDevice(device, true);
        //        await NativeDevice.ConnectWait().Timeout(TimeSpan.FromSeconds(15));
        //    }
        //    catch (Exception) { }
        //}

        private async void OnRestored(IDevice device)
        {
            try
            {
                SetDevice(device, true);
                await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(NativeDevice);
            }
            catch (Exception) { }
        }


        public override void Dispose()
        {
            try
            {
                DeviceStatusChangedTCS?.Cancel();
            }
            catch (TaskCanceledException) { }
            DeviceStatusChangedTCS = null;
            try
            {
                reconnectTCS?.Cancel();
            }
            catch (TaskCanceledException) { }
            reconnectTCS = null;
        }
        #endregion
    }
}
