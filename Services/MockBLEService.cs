using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Enums;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using GrayWolf.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GrayWolf.Services
{
    public class MockBleService : BleService
    {
        private const int DATA_REFRESH_RATE = 1000;

        public DSII_BLEGenerator Generator { get; }

        private BleDevice Device { get; set; }

        private DemoProbeType DemoMode { get; set; }

        private bool IsGeneratorRunning { get; set; }
        public MockBleService()
        {
            Generator = new DSII_BLEGenerator();
        }

        public override void Initialize()
        {
        }

        public void StartDemoMode(DemoProbeType mode)
        {
            DemoMode = mode;
            GenerateMockDevice(DemoMode);
        }

        public void StopDemo()
        {
            Device.Disconnect();
            Device = null;
            IsGeneratorRunning = false;
        }

        public override Task StartScanAsync()
        {
            return Task.CompletedTask;
        }

        public override Task StopScanAsync()
        {
            return Task.CompletedTask;
        }

        public override void ConnectToDevice(BleDevice device)
        {
            InvokeDeviceConnected(device);
            if (!IsGeneratorRunning)
            {
                IsGeneratorRunning = true;
                Microsoft.Maui.Controls.Device.StartTimer(TimeSpan.FromMilliseconds(DATA_REFRESH_RATE), () =>
                {
                    Generator.UpdateValues();
                    return IsGeneratorRunning;
                });
            }
        }

        public override void DisconnectFromDevice(BleDevice device)
        {
            InvokeDeviceDisconnected(device, false);
        }

        private void GenerateMockDevice(DemoProbeType mode)
        {
            try
            {
                var json = Generator.GenerateSimulatedJsonDevice(mode);
                var grayWolfDevice = JsonConvert.DeserializeObject<GrayWolfDevice>(json);
                grayWolfDevice.IsOnline = true;
                grayWolfDevice.IsExpanded = true;

                var deviceService = Ioc.Default.GetService<IDeviceService>();
                var device = new MockBleDevice(grayWolfDevice, Generator, mode, this, deviceService, SensorsService);
                device.Connect();
                Device = device;
                InvokeVisibleDevicesChanged(new List<BleDevice> { Device });
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG);
            }
        }

        protected override Task TryRestoreConnectionAsync()
        {
            return Task.CompletedTask;
        }
    }
}
