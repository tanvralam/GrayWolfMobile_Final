using System;
using System.Globalization;
using System.Linq;


namespace GrayWolf.Converters
{
    public class AnyTrueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //return true if all values are boolean and equals to true
            return values.Any(x => x is bool b && b);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (!(value is bool b) || targetTypes.Any(t => !t.IsAssignableFrom(typeof(bool))))
                return null;

            if (b)
                return targetTypes.Select(t => (object)true).ToArray();
            else
                return null;
        }
    }
}
