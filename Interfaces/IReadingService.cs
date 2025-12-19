using GrayWolf.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
    public interface IReadingService
    {
        Task<List<Reading>> GetDeviceDataAsync(string deviceId);
        Task<Reading> GetReadingByIdAsync(string readingId);
    }
}
