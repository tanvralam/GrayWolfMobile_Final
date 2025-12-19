using GrayWolf.Interfaces;
using GeolocatorPlugin;
using GeolocatorPlugin.Abstractions;
using System;
using System.Threading.Tasks;


namespace GrayWolf.Services
{
    public class GeolocationService : IGeolocationService
    {
        private const double DebugLat = 34.05338210245921;
        private const double DebugLon = -118.2446612916555;

        private TimeSpan GeolocationTimeout => TimeSpan.FromSeconds(5);

        private IGeolocator Geolocator => CrossGeolocator.Current;

        public Position CurrentPosition { get; private set; }

        public GeolocationService()
        {
            CurrentPosition = new Position();
            Geolocator.PositionChanged += Geolocator_PositionChanged;
            Geolocator.PositionError += Geolocator_PositionError;
        }

        public void SetNoPosition()
        {
            CurrentPosition = new Position(0, 0);
        }

        private void Geolocator_PositionError(object sender, PositionErrorEventArgs e)
        {
            CurrentPosition = new Position(0, 0);
        }

        private void Geolocator_PositionChanged(object sender, PositionEventArgs e)
        {
#if GDEBUG
            CurrentPosition = new Position(DebugLat, DebugLon);
#else
            CurrentPosition = e.Position;
#endif
        }

        public async Task<bool> StartListeningAsync()
        {
            var canStart = await CanGetPositionAsync();
            if (!canStart)
            {
                return false;
            }
#if GDEBUG
            CurrentPosition = new Position(DebugLat, DebugLon);
#else
            CurrentPosition = await Geolocator.GetPositionAsync(GeolocationTimeout) ?? new Position();
#endif

            return await Geolocator.StartListeningAsync(TimeSpan.FromSeconds(5), 5, listenerSettings: new ListenerSettings
            {
                PauseLocationUpdatesAutomatically = true,
                ActivityType = ActivityType.Fitness
            });
        }

        public async Task<bool> StopListeningAsync()
        {
            var result = await Geolocator.StopListeningAsync();
            return result;
        }

        public async Task<Position> GetPositionOnceAsync()
        {
            var canGetPosition = await CanGetPositionAsync();
            if (!canGetPosition)
            {
                return new Position();
            }
#if GDEBUG
            CurrentPosition = new Position(DebugLat, DebugLon);
#else
            CurrentPosition = await Geolocator.GetPositionAsync(GeolocationTimeout) ?? new Position();
#endif
            return CurrentPosition;
        }

        private async Task<bool> CanGetPositionAsync()
        {
            var permissions = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (permissions != PermissionStatus.Granted)
            {
                return false;
            }
            if (!Geolocator.IsGeolocationAvailable || !Geolocator.IsGeolocationEnabled)
            {
                return false;
            }
            return true;
        }
    }
}
