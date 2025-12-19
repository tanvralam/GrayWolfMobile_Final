using System;
using System.Globalization;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.DependencyInjection;
using GrayWolf.Enums;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using GrayWolf.Models.DTO;
using SensorUnit = GrayWolf.Enums.SensorUnit;

namespace GrayWolf.Converters
{
    public static class DatumConverters
    {
        private static ISensorsService SensorsService => Ioc.Default.GetService<ISensorsService>();

        public static Datum ToDatum(this LogDatumDTO dto)
        {

            // BVW FuckaDuck
            NegativeValuesHandleStrategy negvals = UnitConverters.GetNegativeValuesHandleStrategy((int)dto.Code, SensorsService);
            double v = dto.Value;
            if (v<0.0)
            {
                if (negvals == NegativeValuesHandleStrategy.ClampToZero) v = 0;                
            }
            if ((SensorsService.IsCO2((int)dto.Code))&&(v< GrayWolf.Helpers.Constants.CO2_CUTOFF))
            {
                if (negvals == NegativeValuesHandleStrategy.ClampCO2) v = GrayWolf.Helpers.Constants.CO2_CUTOFF;
            }

            return new Datum
            {
                Id = dto.Id,
                SourceUnit = new Models.Domain.SensorUnit
                {
                    Value = v.ToString(CultureInfo.InvariantCulture),
                    Name = dto.Unit,
                    Decimals = $"{SensorsService.GetDPsForSensor((int)dto.Code, (int)dto.UnitCode)}",

            
                },
                Type = dto.Code,
                Sensor = dto.Sensor,
            };
        }

        public static Datum ToDatum(this DatumDTO dto)
        {
            // BVW FuckaDuck
            NegativeValuesHandleStrategy negvals = UnitConverters.GetNegativeValuesHandleStrategy((int)dto.Code, SensorsService);
            double v = dto.Value;
            if (v < 0.0)
            {
                if (negvals == NegativeValuesHandleStrategy.ClampToZero) v = 0;
            }
            if ((SensorsService.IsCO2((int)dto.Code)) && (v < GrayWolf.Helpers.Constants.CO2_CUTOFF))
            {
                if (negvals == NegativeValuesHandleStrategy.ClampCO2) v = GrayWolf.Helpers.Constants.CO2_CUTOFF;
            }


            return new Datum
            {
                Id = dto.Id,
                Sensor = dto.Sensor,
                Type = dto.Code,
                Channel = dto.Col,
                SourceUnit = new Models.Domain.SensorUnit
                {
                    Value = v.ToString(CultureInfo.InvariantCulture),
                    Name = dto.Unit,
                    Decimals = $"{SensorsService.GetDPsForSensor((int)dto.Code, (int)dto.UnitCode) }"
                },
            };
        }

        public static LogDatumDTO ToLogDatum(this Reading reading)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";

            var unit = reading.ConvertedUnit;

            // BVW FuckaDuck
            NegativeValuesHandleStrategy negvals = UnitConverters.GetNegativeValuesHandleStrategy((int)reading.SensorCode, SensorsService);
            double v = Math.Round(Convert.ToDouble(unit.Value, provider), 4);

            if (v < 0.0)
            {
                if (negvals == NegativeValuesHandleStrategy.ClampToZero) v = 0;
            }
            if ((SensorsService.IsCO2((int)reading.SensorCode)) && (v < GrayWolf.Helpers.Constants.CO2_CUTOFF))
            {
                if (negvals == NegativeValuesHandleStrategy.ClampCO2) v = GrayWolf.Helpers.Constants.CO2_CUTOFF;
            }

            var logDatum = new LogDatumDTO
            {
                Sensor = reading.Name,
                Value = v,
                Unit = unit.Name,
                Id = reading.SensorId,
                UnitCode = unit.Code.GetSensorUnit(),
                Code = reading.SensorCode
            };
            return logDatum;
        }

        public static LJH_Column ToLjhColumn(this LogDatumDTO dto, string serialNumber, ParameterNameDisplayOption parameterNameDisplayOption)
        {
            return new LJH_Column
            {
                Id = dto.Id,
                Code = (int)dto.Code,
                Sensor = GetSensorName((int)dto.Code, SensorsService, parameterNameDisplayOption),
                SerialNumber = serialNumber,
                Unit = dto.Unit,
                UnitCode = (int)dto.UnitCode
            };
        }

        private static string GetSensorName(int code, ISensorsService sensorsService, ParameterNameDisplayOption parameterNameDisplayOption)
        {
            var isShort = parameterNameDisplayOption == Enums.ParameterNameDisplayOption.Short;
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
