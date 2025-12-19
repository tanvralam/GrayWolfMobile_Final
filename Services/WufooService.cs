using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrayWolf.Services
{
    public class WufooService : IWufooService
    {
        private IAnalyticsService AnalyticsService { get; }
        private IApiService ApiService { get; }
        
        public WufooService()
        {
            AnalyticsService = Ioc.Default.GetService<IAnalyticsService>();
            ApiService = SimpleIoc.Default.GetInstance<IApiService>(Constants.WUFOO_API_SERVICE_KEY);
           
        }

        public async Task SubmitDemoFormAsync(DemoModeForm form)
        {
            try
            {
                var args = new Dictionary<string, string>
                {
                    { "Field1", form.Name },
                    { "Field9", form.Email }
                };
                var result = await ApiService.PostFormDataAsync<Dictionary<string, object>>("api/v3/forms/m1yufxjw0wvju1b/entries.json", args);
                if (!result.TryGetValue("Success", out var success))
                {
                    throw new Exception("Failed to submit form");
                }

                if (long.TryParse($"{success}", out var lSuccess) && lSuccess == 1)
                {
                    return;
                }
                else
                {
                    throw new Exception("Failed to submit form");
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                throw ex;
            }
        }
    }
}
