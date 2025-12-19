using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GrayWolf.Converters;
using GrayWolf.Enums;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Messages;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Devices;
//using Syncfusion.Maui.Data;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
namespace GrayWolf.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly List<BleDevice> DiscoveredBleGrayWolfDevices = new List<BleDevice>();
        private const int INTERVAL_MS_CLOUD = 30000;  //CLOUD is only polled every 30-60 seconds
        private const string TAG = "DeviceService";

        public static IDeviceService Instance => (IDeviceService)SimpleIoc.Default.GetService(typeof(IDeviceService));

        public IBleService BLEInstance { get; }

        public MockBleService FakeBleInstance { get; }

        private IDatabase Database { get; }

        private IDeviceAPI DeviceAPI { get; }

        private ISensorsService SensorsService { get; }

        private IReadingService ReadingService { get; }

        private IAnalyticsService AnalyticsService { get; }

        private ISettingsService SettingsService { get; }

        public bool IsDemoMode
        {
            get => SettingsService.IsDemoMode;
            private set
            {
                SettingsService.IsDemoMode = value;
            }
        }

        public DeviceService()
        {
            BLEInstance = SimpleIoc.Default.GetInstance<IBleService>(Constants.BLE_FACTORY_REAL);            
            BLEInstance.OnVisibleDevicesChanged += BLEService_OnVisibleDevicesChanged;           
            FakeBleInstance = SimpleIoc.Default.GetInstance<IBleService>(Constants.BLE_FACTORY_MOCK) as MockBleService;
            FakeBleInstance.OnVisibleDevicesChanged += FakeBleInstance_OnFakeDevicesAppeared;
            Database = Ioc.Default.GetService<IDatabase>();
            DeviceAPI = Ioc.Default.GetService<IDeviceAPI>();
            SensorsService = Ioc.Default.GetService<ISensorsService>();
            AnalyticsService = Ioc.Default.GetService<IAnalyticsService>();
            ReadingService = Ioc.Default.GetService<IReadingService>();
            SettingsService = Ioc.Default.GetService<ISettingsService>();

          
        }

        private async void FakeBleInstance_OnFakeDevicesAppeared(object sender, Utility.ScannedDevicesUpdatedEventArgs e)
        {
            var devices = e.Devices.Select(x => x.GrayWolfDevice).ToList();
            await SelectDevicesAsync(devices);
        }

        public async void Init()
        {
            if (IsDemoMode)
            {
                await ResetDemoDevicesAsync();
            }
            await Database.DeleteAllItemsAsync<GrayWolfDeviceDBO>();
            var selectedDevices = await GetSelectedDevicesAsync();
            var tasks = new List<Task>();
            foreach (var device in selectedDevices)
            {
                if (device.Source == DeviceSource.Ble)
                    device.IsOnline = false;
                tasks.Add(UpdateDeviceInDBAsync(device, DeviceSource.Ble));
            }
            await Task.WhenAll(tasks);
            selectedDevices = await GetSelectedDevicesAsync();
            Messenger.Default.Send(new SelectedDevicesMessage
            {
                Devices = selectedDevices.ToList()
            });

            BLEInstance.Initialize();
            FakeBleInstance.Initialize();
            StartPolling();
        }

        private async Task ResetDemoDevicesAsync()
        {
            try
            {
                IsDemoMode = false;
                await SelectDevicesAsync(new List<GrayWolfDevice>());
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
            }
        }

        #region polling
        private bool _continue;
        public async Task<bool> RefreshCloudDevices()
        {
            if (!_continue) return false;

            try
            {
                var devices = await GetDevicesBySourceAsync(DeviceSource.Cloud);
                await FetchDevicesDataAsync(devices);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                return false;
            }

            return true;

        }

        public async Task FetchCloudDevicesAsync()
        {
            try
            {
                var devices = await DeviceAPI.GetDevicesForUser(SettingsService.Email, SettingsService.Password);
                await FetchDevicesDataAsync(devices);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                throw ex;
            }
        }

        private async Task FetchDevicesDataAsync(List<GrayWolfDevice> devices)
        {
            var devicesWithData = await GetCloudDevicesWithDataAsync(devices);
            await UpdateDevicesInDBAsync(devicesWithData, DeviceSource.Cloud);
        }

        private async Task<List<GrayWolfDevice>> GetCloudDevicesWithDataAsync(List<GrayWolfDevice> devices)
        {
            try
            {
                var tasks = new List<Task<List<GrayWolfDevice>>>();
                if (devices == null)
                    return null;
                foreach (var grayWolfDevice in devices)
                {
                    tasks.Add(DeviceAPI.GetDeviceData($"{grayWolfDevice.DeviceID}", grayWolfDevice.SecurityToken));
                }
                await Task.WhenAll(tasks);

                for (int i = 0; i < tasks.Count; i++)
                {
                    var result = tasks[i].Result;
                    var grayWolfDevice = devices[i];
                    grayWolfDevice.Data = result
                        .SelectMany(x => x.Data)
                        .ToList();
                    if (result.FirstOrDefault() is GrayWolfDevice resultDevice)
                    {
                        grayWolfDevice.Title = resultDevice.Title;
                        grayWolfDevice.BatteryStatus = resultDevice.BatteryStatus;
                        grayWolfDevice.BatteryStatusEnum = resultDevice.BatteryStatusEnum;
                        grayWolfDevice.DeviceSerialNum = resultDevice.DeviceSerialNum;
                        grayWolfDevice.IsDeleted = resultDevice.IsDeleted;
                        grayWolfDevice.Status = resultDevice.Status;
                        grayWolfDevice.SensorStatus = resultDevice.SensorStatus;
                        grayWolfDevice.StatusEnum = resultDevice.StatusEnum;
                        grayWolfDevice.SensorStatusEnum = resultDevice.SensorStatusEnum;
                        grayWolfDevice.IsOnline = resultDevice.IsOnline;
                        grayWolfDevice.Uptime = resultDevice.Uptime < 0 ? 0 : resultDevice.Uptime;
                    }
                }

                return devices;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                throw ex;
            }
        }

        public async void StartPolling()
        {
            _continue = true;
            await RefreshCloudDevices();

            Device.StartTimer(TimeSpan.FromMilliseconds(INTERVAL_MS_CLOUD), () =>
            {
                Task.Run(async () =>
                {
                    await RefreshCloudDevices();
                });
                return true;
            });
        }
        #endregion

        private async Task<List<GrayWolfDevice>> GetDevicesBySourceAsync(DeviceSource source)
        {
            List<GrayWolfDeviceDBO> allDevices = null;
            try
            {
                allDevices = await Database.GetItemsAsync<GrayWolfDeviceDBO>();
                var sourceFormatted = $"{source}";
                var devices = await Database.GetItemsAsync<GrayWolfDeviceDBO>(p => p.Source == sourceFormatted);
                return devices
                    .Select(x => x.ToGrayWolfDevice(new List<Reading>()))
                    .ToList();
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                throw ex;
            }
        }

        public async Task UpdateDevicesInDBAsync(IEnumerable<GrayWolfDevice> devices, DeviceSource source)
        {
            try
            {
                if (!(devices?.Any() ?? false))
                    return;

                //devices.ForEach(p =>
                //{
                //    p.Source = source;
                //    p.UpdatePosition();
                //});

                devices.ToList().ForEach(p =>
                {
                    p.Source = source;
                    p.UpdatePosition();
                });
                var sourceFormatted = $"{source}";
                var existingDevices = await Database.GetItemsAsync<GrayWolfDeviceDBO>(x => x.Source == sourceFormatted);
                var existingDeviceIds = existingDevices.Select(x => x.Id).ToList();

                var deviceIds = devices.Select(p => p.DeviceID)
                    .ToList();

                var deletedDeviceIds = existingDeviceIds
                    .Except(deviceIds)
                    .ToList();
                var deleteTasks = new List<Task>();
                foreach (var id in deletedDeviceIds)
                {
                    deleteTasks.Add(Database.DeleteItemAsync<GrayWolfDeviceDBO>(id));
                }

                var existingDbDevices = await Database.GetItemsAsync<GrayWolfDeviceDBO>();

                var tasks = new List<Task>();
                foreach (var grayWolfDevice in devices)
                {
                    grayWolfDevice.IsSelected = existingDbDevices.Any(x => x.Id == grayWolfDevice.DeviceID && x.IsSelected);
                    tasks.Add(UpdateDeviceInDBAsync(grayWolfDevice, source));
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                throw ex;
            }
        }

        public async Task SelectDevicesAsync(List<GrayWolfDevice> domainDevices)
        {
            try
            {
                var previouslyConnectedDevices = await Database.GetItemsAsync<GrayWolfDeviceDBO>(x => x.IsSelected);
                var selectedDevices = domainDevices.Select(x => x.ToGrayWolfDeviceDBO()).ToList();
                foreach (var selectedDevice in selectedDevices)
                {
                    selectedDevice.IsSelected = true;
                }
                var deselectedDevices = previouslyConnectedDevices.Where(x => !selectedDevices.Any(y => y.Id == x.Id));
                foreach (var deselectedDevice in deselectedDevices)
                {
                    deselectedDevice.IsSelected = false;
                }
                var devicesToUpdate = new List<GrayWolfDeviceDBO>(selectedDevices);
                devicesToUpdate.AddRange(deselectedDevices);
                await Database.UpsertAllAsync(devicesToUpdate);
                NotifyDevicesUpdated();
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                throw ex;
            }
        }

        public async Task UpdateDeviceInDBAsync(GrayWolfDevice device, DeviceSource source)
        {
            try
            {
                device.Source = source;
                device.IsExpanded = true;
                var dbo = device.ToGrayWolfDeviceDBO();
                await Database.UpsertAsync(dbo);
                await RefreshDeviceDataAsync(device);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
            }
            await Device.InvokeOnMainThreadAsync(() => NotifyDeviceUpdated(device.DeviceID));
        }

        public async Task RefreshDeviceDataAsync(GrayWolfDevice device)
        {
            var readings = device.Data;
            if (readings == null)
            {
                return;
            }
            var deviceId = device.DeviceID;
            var newDbos = readings
                .Select(x => x.ToReadingDBO())
                .DistinctBy(x => x.Id)
                .ToList();

            var existingItems = await Database
                .GetItemsAsync<ReadingDBO>(x => x.DeviceId == device.DeviceID);
            var existingItemsIds = existingItems
                .Select(x => x.Id)
                .ToList();
            var ids = newDbos
                .Select(x => x.Id)
                .ToList();
            var idsToDelete = existingItemsIds
                .Except(ids)
                .ToList();
            foreach (var newDbo in newDbos)
            {
                if (existingItems.FirstOrDefault(x => x.Id == newDbo.Id) is ReadingDBO existingDbo)
                {
                    newDbo.IsLogged = existingDbo.IsLogged;
                }
                else
                {
                    newDbo.IsLogged = !SensorsService.IsDerived(newDbo.SensorCode);
                }
            }
            await Database.UpsertAllAsync(newDbos);
            await Database.DeleteItemsAsync<ReadingDBO>(idsToDelete);
        }

        #region event handlers
        private void BLEService_OnVisibleDevicesChanged(object sender, Utility.ScannedDevicesUpdatedEventArgs e)
        {
            DiscoveredBleGrayWolfDevices.Clear();
            DiscoveredBleGrayWolfDevices.AddRange(e.Devices);
        }
        #endregion

        public async Task<IEnumerable<GrayWolfDevice>> GetSelectedDevicesAsync()
        {
            var devices = await Database
                .GetItemsAsync<GrayWolfDeviceDBO>(x => x.IsSelected);
            devices = devices
                .DistinctBy(x => x.Id)
                .OrderBy(x => x.DeviceName)
                .ToList();
            var result = new List<GrayWolfDevice>();
            var dataTasks = new List<Task<List<Reading>>>();
            foreach (var dbo in devices)
            {
                dataTasks.Add(ReadingService.GetDeviceDataAsync(dbo.Id));
            }
            await Task.WhenAll(dataTasks);
            for (int i = 0; i < devices.Count; i++)
            {
                var dbo = devices[i];
                var data = dataTasks[i].Result;
                var device = dbo.ToGrayWolfDevice(data);
                result.Add(device);
            }
            return result;
        }

        public IEnumerable<BleDevice> GetDiscoveredBleDevices()
        {
            return DiscoveredBleGrayWolfDevices.OrderBy(x => x.DeviceName).ToList();
        }

        public async Task<IEnumerable<GrayWolfDevice>> GetCloudDevicesFromDBAsync()
        {
            var source = $"{DeviceSource.Cloud}";
            var devices = await Database.GetItemsAsync<GrayWolfDeviceDBO>(x => x.Source == source);
            return devices.Select(x => x.ToGrayWolfDevice()).DistinctBy(x => x.DeviceID);
        }

        public async Task SetUnitConversion(Reading reading, Models.Domain.SensorUnit sensorUnit)
        {
            await SensorsService.SetUnitConversion(reading, sensorUnit, $"{reading.DeviceId}:{reading.SensorId}:{(int)reading.SensorCode}:{reading.OriginalUnit.Code}", sensorUnit.Code);
        }

        public void RemoveUnitConversion(int conversionId, string deviceId)
        {
            SensorsService.RemoveUnitConversion(conversionId);
        }

        public async Task UpdateReadingVisibilityAsync(string id, bool isLogged)
        {
            var reading = await Database.GetItemAsync<ReadingDBO>(id);
            reading.IsLogged = isLogged;
            await Database.UpdateAsync(reading);
        }

        public async Task<GrayWolfDevice> GetDeviceByIdAsync(string deviceId)
        {
            var dbo = await Database.GetItemAsync<GrayWolfDeviceDBO>(deviceId);
            if (dbo == null)
            {
                return null;
            }
            var data = await ReadingService.GetDeviceDataAsync(deviceId);
            return dbo.ToGrayWolfDevice(data);
        }

        public async void NotifyDeviceUpdated(string deviceId)
        {
            var device = await GetDeviceByIdAsync(deviceId);
            if (device == null)
            {
                return;
            }
            Messenger.Default.Send(new DeviceConfigurationChangedMessage
            {
                Device = device
            });
        }

        public async Task SelectDefaultDeviceForDemoAsync()
        {
            var devices = await GetCloudDevicesFromDBAsync();
            var device = devices.FirstOrDefault();
            if(device == null)
            {
                return;
            }
            await SelectDevicesAsync(new List<GrayWolfDevice>
            {
                device
            });
        }

        public async Task StartDemoModeAsync(DemoProbeType mode)
        {
            IsDemoMode = true;
            await SelectDevicesAsync(new List<GrayWolfDevice>());

            (FakeBleInstance as MockBleService).StartDemoMode(mode);
            await FakeBleInstance.StartScanAsync();
        }

        public async Task StopDemoModeAsync()
        {
            IsDemoMode = false;
            FakeBleInstance.StopDemo();
            await SelectDevicesAsync(new List<GrayWolfDevice>());
        }

        public async void NotifyDevicesUpdated()
        {
            var newSelectedDevices = (await GetSelectedDevicesAsync()).ToList();
            Messenger.Default.Send(new SelectedDevicesMessage
            {
                Devices = newSelectedDevices
            });
        }
    }
}
