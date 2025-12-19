using GeolocatorPlugin.Abstractions;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
    public interface IGeolocationService
    {
        Position CurrentPosition { get; }
        void SetNoPosition(); 

        Task<Position> GetPositionOnceAsync();
        Task<bool> StartListeningAsync();

        Task<bool> StopListeningAsync();
    }
}