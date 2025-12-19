
using GrayWolf.Enums;
using GrayWolf.Extensions;
using GrayWolf.Interfaces;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using GrayWolf.Models.DTO;
using GrayWolf.Services;
using System;
using SensorUnit = GrayWolf.Models.Domain.SensorUnit;

namespace GrayWolf.Converters
{
    public static class ReadingConverters
    {
        public static Reading ToReading(this DatumDTO datum, DateTime timeStamp, string deviceId, string deviceSerialNum, ISensorsService sensorsService, ISettingsService settings)
        {
            var unit = datum.ToSensorUnit(sensorsService);

            return new Reading
            {
                SensorCode = datum.Code,
                SensorId = datum.Id,
                DeviceId = deviceId,
                Channel = datum.Col,
                OriginalUnit = unit,
                ConvertedUnit = unit,
                TimeStamp = timeStamp,
                Name = GetSensorName((int)datum.Code, sensorsService, settings),
                DeviceSerialNumber = deviceSerialNum,
                Id = GetReadingId(deviceId, datum.Id, (int)datum.Code, (int)datum.UnitCode)
            };
        }

        public static Reading ToReading(this ReadingDBO dbo, SensorUnit originalUnit, SensorUnit convertedUnit, ISensorsService sensorsService, ISettingsService settings)
        {
            return new Reading
            {
                SensorId = dbo.SensorId,
                Status = dbo.Status,
                DeviceSerialNumber = dbo.DeviceSerialNumber,
                DeviceId = dbo.DeviceId,
                Channel = dbo.Channel,
                TimeStamp = dbo.TimeStamp,
                OriginalUnit = originalUnit,
                ConvertedUnit = convertedUnit,
                IsLogged = dbo.IsLogged,
                Name = GetSensorName(dbo.SensorCode, sensorsService, settings),
                SensorCode = (SensorType)dbo.SensorCode,
                Id = dbo.Id
            };
        }

        public static ReadingDBO ToReadingDBO(this Reading reading)
        {
            return new ReadingDBO
            {
                SensorId = reading.SensorId,
                Status = reading.Status,
                DeviceSerialNumber = reading.DeviceSerialNumber,
                SensorCode = (int)reading.SensorCode,
                TimeStamp = reading.TimeStamp,
                Channel = reading.Channel,
                DeviceId = reading.DeviceId,
                IsLogged = reading.IsLogged,
                Id = GetReadingId(reading.DeviceId, reading.SensorId, (int)reading.SensorCode, reading.OriginalUnit.Code),
                UnitCode = reading.OriginalUnit.Code,
                Value = reading.OriginalUnit.Value,
            };
        }

        public static string GetReadingId(string deviceId, int sensorId, int sensorCode, int unitCode)
        {
            return $"{deviceId}:{sensorId}:{sensorCode}:{unitCode}";
        }

        private static string GetSensorName(int code, ISensorsService sensorsService, ISettingsService settings)
        {
            var isShort = settings.ParameterNameDisplayMode == Enums.ParameterNameDisplayOption.Short;
            if (isShort)
            {
                return sensorsService.LookupShortSensorName(code);
            }
            else
            {
                return sensorsService.GetSensorName(code);
            }
        }

    }
}
