using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Converters;
using GrayWolf.Extensions;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using GrayWolf.Models.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GrayWolf.Services
{
    public class DeviceAPI : IDeviceAPI
    {
        private IApiService ApiService { get; }

        private ISensorsService SensorsService { get; }

        private IAnalyticsService AnalyticsService { get; }

        private ISettingsService SettingsService { get; }

        public DeviceAPI()
        {
            var container = Ioc.Default;
            ApiService = SimpleIoc.Default.GetInstance<IApiService>(Constants.GRAYWOLF_API_SERVICE_KEY);           
            SensorsService = container.GetService<ISensorsService>();
            AnalyticsService = container.GetService<IAnalyticsService>();
            SettingsService = container.GetService<ISettingsService>();
        }

        public async Task<List<GrayWolfDevice>> GetDeviceData(string deviceId, string token, CancellationToken cToken = default)
        {
            string url = "services/v1/mobile.aspx/GetDeviceData";
            try
            {
                var input = new GetDevicesInput { DataParameters = $"d={deviceId}&t={token}" };
                var response = await ApiService.PostAsync<RawGetDevicesReturn>(url, input);

                var objectString = response.D;
                if (objectString.IsNullOrWhiteSpace())
                {
                    return new List<GrayWolfDevice>();
                }
                else
                {
                    var responseObject = JsonConvert.DeserializeObject<GetDeviceDataReturn>(objectString);
                    return responseObject.Devices.Select(x => x.ToGrayWolfDevice(deviceId, SensorsService, SettingsService)).ToList();
                }
            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e);
                throw;
            }
        }

        public async Task<List<GrayWolfDevice>> GetDevicesForUser(string username, string password, CancellationToken cToken = default)
        {
            string url = "services/v1/mobile.aspx/GetDevicesForUser";
            try
            {
                var input = new GetDevicesInput { DataParameters = $"u={username}&p={password}" };

                var response = await ApiService.PostAsync<RawGetDevicesReturn>(url, input);
                var responseObject = JsonConvert.DeserializeObject<GetDevicesReturn>(response.D);

                return responseObject.Data.Devices;
            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e);
                throw e;
            }
        }
    }
}
