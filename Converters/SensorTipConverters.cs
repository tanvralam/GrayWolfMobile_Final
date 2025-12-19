using GrayWolf.Models.Domain;
using GrayWolf.Models.DTO;

namespace GrayWolf.Converters
{
    public static class SensorTipConverters
    {
        public static SensorTip ToDomain(this SensorTipDTO dto, string sensorName)
        {
            return new SensorTip
            {
                CategoryId = dto.CategoryID,
                CategoryName = $"{dto.CategoryName} {sensorName}"
            };
        }
    }
}
