using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;


namespace GrayWolf.Converters
{
    public class IsEnabledToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is bool isEnabled))
            {
                throw new ArgumentException($"{nameof(IsEnabledToOpacityConverter)} requires boolean value");
            }

            if(!double.TryParse($"{parameter}", out var opacity))
            {
                opacity = 0.4;
            }

            return isEnabled ? 1 : opacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
