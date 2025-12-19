using System;
using System.Globalization;


namespace GrayWolf.Converters
{
    public class IsEqualToResultMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length != 4)
            {
                throw new ArgumentException();
            }
            var obj1 = values[0];
            var obj2 = values[1];
            if (obj1 == null && obj2 == null)
            {
                return true;
            }
            if(obj1 == null)
            {
                return false;
            }
            var isEqual = obj1.Equals(obj2);
            if (isEqual)
            {
                return values[2];
            }
            return values[3];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
