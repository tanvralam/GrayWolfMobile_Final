using GrayWolf.Enums;
using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;


namespace GrayWolf.Converters
{
    public class ProbeStatusToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProbeStatus status)
            {
                if ((status == ProbeStatus.STABILIZING) || (status == ProbeStatus.UNKNOWN)) return Colors.Gray;
                if ((status == ProbeStatus.PUMP_ERROR) || (status == ProbeStatus.LASER_ERROR)) return Colors.Red;
                return Colors.Black;
            }
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
