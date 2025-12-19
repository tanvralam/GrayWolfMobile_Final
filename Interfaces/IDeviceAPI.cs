using GrayWolf.Models.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
    public interface IDeviceAPI
    {
        Task<List<GrayWolfDevice>> GetDevicesForUser(string username, string password, CancellationToken cToken = default);

        Task<List<GrayWolfDevice>> GetDeviceData(string deviceId, string token, CancellationToken cToken = default);
    }
}
