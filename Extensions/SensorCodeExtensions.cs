namespace GrayWolf.Extensions
{
    public static class SensorCodeExtensions
    {
        public static bool IsCoordinatesSensorCode(this int code)
        {
            return code == (int)Enums.SensorType.PRBSEN_LATITUDE || code == (int)Enums.SensorType.PRBSEN_LONGITUDE;
        }
    }
}
