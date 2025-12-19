using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Converters;
using GrayWolf.Enums;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using GrayWolf.Models.DTO;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GrayWolf.Services
{
    public class SensorTipsService : ISensorTipsService
    {
        private IAnalyticsService AnalyticsService { get; }
        private IApiService ApiService { get; }

        public SensorTipsService()
        {
            AnalyticsService = Ioc.Default.GetService<IAnalyticsService>();
            ApiService = SimpleIoc.Default.GetInstance<IApiService>(Constants.SENSOR_TIPS_API_SERVICE_KEY);
            //ApiService = Ioc.Default.GetService<IApiService>();
        }

        public async Task<List<SensorTip>> GetSensorTipsAsync(SensorType sensorType, DeviceModelsEnum deviceType, string defaultSensorName)
        {
            var nvc = new NameValueCollection()
            {
                { "sensor", $"{(int)sensorType}" },
                { "family", $"{(int)deviceType}" }
            };
            var result = await ApiService.GetAsync<List<SensorTipDTO>>("/api/SensorTips/", nvc);
            return result.Select(x => x.ToDomain(defaultSensorName)).ToList();
        }

        public Task<string> GetHtmlTextForCategoryAsync(int categoryId, SensorType sensorType, DeviceModelsEnum deviceType, CancellationToken cToken = default)
        {
            var nvc = new NameValueCollection()
            {
                { "categoryID", $"{categoryId}" },
                { "sensorID", $"{(int)sensorType}" },
                { "familyID", $"{(int)deviceType}" }
            };
            return ApiService.GetAsync<string>("/api/SensorTips/", nvc, cToken, deserializeAction: x => x.Trim('\"'));
        }
    }
}
