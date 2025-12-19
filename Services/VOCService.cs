using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Converters;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using GrayWolf.Models.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrayWolf.Services
{
    public class VOCService : IVOCService
    {
        private IApiService ApiService { get; }

        public VOCService()
        {
            ApiService = SimpleIoc.Default.GetInstance<IApiService>(Constants.SENSOR_TIPS_API_SERVICE_KEY);
         
        }

        public async Task<List<VOCItem>> GetVOCItemsAsync()
        {
            var result = await ApiService.GetAsync<List<VOCItemDTO>>("/api/VOCList");
            return result.Select(x => x.ToDomain()).ToList();
        }
    }
}
