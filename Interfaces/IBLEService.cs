using GrayWolf.Models.Domain;
using GrayWolf.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
    public interface IBleService
    {
        event EventHandler<ScannedDevicesUpdatedEventArgs> OnVisibleDevicesChanged;

        void Initialize();

        void ConnectToDevice(BleDevice device);

        void ConnectToDevices(List<BleDevice> devices);

        void DisconnectFromDevice(BleDevice device);

        void DisconnectFromDevices(List<BleDevice> device);

        Task StartScanAsync();

        Task StopScanAsync();
    }
}
