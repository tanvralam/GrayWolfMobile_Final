using GrayWolf.Interfaces;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using GrayWolf.Models.DTO;

namespace GrayWolf.Converters
{
    public static class UnitConverters
    {
        public static SensorUnit ToSensorUnit(this DatumDTO dto, ISensorsService sensorService)
        {
            var sensorCode = (int)dto.Code;
            var unitCode = (int)dto.UnitCode;
            return new SensorUnit
            {
                Value = $"{dto.Value}",
                Code = (int)dto.UnitCode,
                Name = dto.Unit,
                NegativeValuesHandleStrategy = GetNegativeValuesHandleStrategy(sensorCode, sensorService),
                Decimals = $"{sensorService.GetDPsForSensor(sensorCode,unitCode)}"
            };
        }

        public static SensorUnit ToSensorUnit(this ReadingDBO dbo, ISensorsService sensorService)
        {
            var sensorCode = dbo.SensorCode;
            int unitCode = (int)dbo.UnitCode;
            return new SensorUnit
            {
                Code = dbo.UnitCode,
                Name = sensorService.GetSensorUnitName(dbo.UnitCode),
                Decimals = $"{sensorService.GetDPsForSensor(dbo.SensorCode, unitCode)}",
                NegativeValuesHandleStrategy = GetNegativeValuesHandleStrategy(sensorCode, sensorService),
                Value = dbo.Value
            };
        }

        public static NegativeValuesHandleStrategy GetNegativeValuesHandleStrategy(int code, ISensorsService sensorsService)
        {
            if(sensorsService.IsCO2(code))
            {
                return NegativeValuesHandleStrategy.ClampCO2;
            }

            var isClampable = ((sensorsService.IsEC(code)) || (sensorsService.IsPID(code)) || (sensorsService.IsHCHO(code)));
    
            return isClampable ? NegativeValuesHandleStrategy.ClampToZero : NegativeValuesHandleStrategy.ShowNegativeValues;
        }
    }
}
