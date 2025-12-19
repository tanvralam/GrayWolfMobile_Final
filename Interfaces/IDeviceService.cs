using GrayWolf.Enums;
using GrayWolf.Models.Domain;
using GrayWolf.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using SensorUnit = GrayWolf.Models.Domain.SensorUnit;

namespace GrayWolf.Interfaces
{
    public interface IDeviceService
    {
        bool IsDemoMode { get; }

        IEnumerable<BleDevice> GetDiscoveredBleDevices();

        Task<IEnumerable<GrayWolfDevice>> GetCloudDevicesFromDBAsync();

        IBleService BLEInstance { get; }

        Task SelectDevicesAsync(List<GrayWolfDevice> devices);

        Task<IEnumerable<GrayWolfDevice>> GetSelectedDevicesAsync();

        Task FetchCloudDevicesAsync();

        Task UpdateDeviceInDBAsync(GrayWolfDevice device, DeviceSource source);

        Task SetUnitConversion(Reading reading, SensorUnit sensorUnit);

        void RemoveUnitConversion(int conversionId, string deviceId);

        Task UpdateReadingVisibilityAsync(string id, bool isLogged);

        void NotifyDeviceUpdated(string deviceId);

        Task StartDemoModeAsync(DemoProbeType mode);

        Task StopDemoModeAsync();

        void NotifyDevicesUpdated();
        void Init();
        Task<GrayWolfDevice> GetDeviceByIdAsync(string deviceId);
    }
}
