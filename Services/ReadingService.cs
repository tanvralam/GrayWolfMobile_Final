
using CommunityToolkit.Mvvm.DependencyInjection;
using GrayWolf.Converters;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using System.Globalization;


namespace GrayWolf.Services
{
    public class ReadingService : IReadingService
    {
        private IDatabase Database { get; }

        private IAnalyticsService AnalyticsService { get; }

        private ISensorsService SensorsService { get; }

        private ISettingsService SettingsService { get; }

        public ReadingService()
        {
            var container = Ioc.Default;
            Database = container.GetService<IDatabase>();
            AnalyticsService = container.GetService<IAnalyticsService>();
            SensorsService = container.GetService<ISensorsService>();
            SettingsService = container.GetService<ISettingsService>();
        }

        public async Task<Reading> GetReadingByIdAsync(string readingId)
        {
            try
            {
                var dbo = await Database.GetItemAsync<ReadingDBO>(readingId);
                var conversionId = GetConversionIdFromReading(dbo);
                var conversion = await Database.GetItemAsync<UnitConversionDBO>(conversionId);
                var originalUnit = dbo.ToSensorUnit(SensorsService);
                var convertedUnit = conversion == null ? originalUnit : GetConvertedUnit(conversion, dbo);
                return dbo.ToReading(originalUnit, convertedUnit, SensorsService, SettingsService);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Reading>> GetDeviceDataAsync(string deviceId)
        {
            var result = new List<Reading>();
            var readingDbos = (await Database.GetItemsAsync<ReadingDBO>(x => x.DeviceId == deviceId)).OrderBy(x => x.Channel);
            var conversions = await Database.GetItemsAsync<UnitConversionDBO>();
            foreach (var dbo in readingDbos)
            {
                var originalUnit = dbo.ToSensorUnit(SensorsService);
                Models.Domain.SensorUnit convertedUnit;
                var conversionId = GetConversionIdFromReading(dbo);
                if (conversions.FirstOrDefault(x => x.Id == conversionId) is UnitConversionDBO conversionDBO)
                {
                    convertedUnit = GetConvertedUnit(conversionDBO, dbo);
                }
                else
                {
                    convertedUnit = originalUnit;
                }
                result.Add(dbo.ToReading(originalUnit, convertedUnit, SensorsService, SettingsService));
            }
            return result.DistinctBy(x => x.Id).ToList();//change
            
        }

        private Models.Domain.SensorUnit GetConvertedUnit(UnitConversionDBO conversion, ReadingDBO dbo)
        {
            var value = Double.TryParse(dbo.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue) ? parsedValue : 0.0;
            var unitCode = conversion.TargetUnitCode;
            return new Models.Domain.SensorUnit
            {
                Code = conversion.TargetUnitCode,
                Name = SensorsService.GetSensorUnitName(unitCode),
                NegativeValuesHandleStrategy = GrayWolf.Converters. UnitConverters.GetNegativeValuesHandleStrategy(dbo.SensorCode, SensorsService),
                Decimals = $"{SensorsService.GetDPsForSensor(dbo.SensorCode, unitCode)}",
                Value = SensorsService.ConvertValue(value, dbo.UnitCode, conversion.TargetUnitCode, dbo.SensorCode)
            };
        }

        private string GetConversionIdFromReading(ReadingDBO dbo)
        {
            var con = $"{dbo.DeviceId}:{dbo.SensorId}:{dbo.SensorCode}:{dbo.UnitCode}";
            return con;
        }
    }
}
