using GrayWolf.Converters;
using GrayWolf.Enums;
using GrayWolf.Interfaces;
using GrayWolf.Utility;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GrayWolf.Models.Domain
{
    public class MockBleDevice : BleDevice
    {
        private DSII_BLEGenerator Generator { get; }

        private DemoProbeType DemoMode { get; }

        private ISensorsService SensorsService { get; }

        public MockBleDevice(
            GrayWolfDevice device,
            DSII_BLEGenerator generator,
            DemoProbeType mode,
            IBleService bleService,
            IDeviceService deviceService,
            ISensorsService sensorsService) : base(
                bleService,
                deviceService)
        {
            GrayWolfDevice = device;
            UpdateDevice(device);
            Generator = generator;
            DemoMode = mode;
            SensorsService = sensorsService;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(GrayWolfDevice):
                    DeviceName = GrayWolfDevice?.DeviceName;
                    break;
            }
        }

        public override async Task FetchDeviceAsync()
        {
            try
            {
                var json = Generator.UpdateSimulatedDevice(DemoMode);
                var device = JsonConvert.DeserializeObject<GrayWolfDevice>(json);
                
                foreach (var reading in device.Data)
                {
                    reading.DeviceId = device.DeviceID;


                    int unitCode = reading.ConvertedUnit.Code; // fuckaduck

                    var decimals = $"{SensorsService.GetDPsForSensor((int)reading.SensorCode, unitCode)}";

                    reading.OriginalUnit.Decimals = decimals;
                    reading.ConvertedUnit.Decimals = decimals;
                    reading.Id = ReadingConverters.GetReadingId(reading.DeviceId, reading.SensorId, (int)reading.SensorCode, reading.OriginalUnit.Code);
                }
                UpdateDevice(device);
                if (IsFetchRunning)
                    await OnDeviceFetchCompletedAsync(GrayWolfDevice, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Mock BLE device fetch failed: {ex.Message}");
            }
        }

        private void UpdateDevice(GrayWolfDevice newDevice)
        {
            GrayWolfDevice.BatteryStatusEnum = newDevice.BatteryStatusEnum;
            GrayWolfDevice.BatteryStatus = $"{newDevice.BatteryStatusEnum}";
            GrayWolfDevice.Status = $"{newDevice.StatusEnum}";
            GrayWolfDevice.SensorStatus = $"{newDevice.SensorStatusEnum}";
            GrayWolfDevice.SensorStatusEnum = newDevice.SensorStatusEnum;
            GrayWolfDevice.StatusEnum = newDevice.StatusEnum;
            GrayWolfDevice.IsSelected = true;
            GrayWolfDevice.IsOnline = true;
            GrayWolfDevice.Uptime = newDevice.Uptime;
            GrayWolfDevice.UpdateData(newDevice.Data);
        }
    }
}
