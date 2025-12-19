using GrayWolf.Enums;
using System;
using System.Globalization;


namespace GrayWolf.Converters
{
    public class BatteryStatusToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BatteryStatus status)
            {
                return status == BatteryStatus.LOW || status == BatteryStatus.CRITICAL ? "LowBattery.png" : null;
            }
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
