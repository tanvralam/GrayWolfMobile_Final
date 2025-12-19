using GrayWolf.Enums;
using GrayWolf.Models.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
    public interface ISensorTipsService
    {
        Task<string> GetHtmlTextForCategoryAsync(int categoryId, SensorType sensorType, DeviceModelsEnum deviceType, CancellationToken cToken = default);
        Task<List<SensorTip>> GetSensorTipsAsync(SensorType sensorType, DeviceModelsEnum deviceType, string defaultSensorName);
    }
}
