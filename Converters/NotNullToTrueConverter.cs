using System;
using System.Globalization;



namespace GrayWolf.Converters
{
    public class NotNullToTrueConverter : IMarkupExtension, IValueConverter
    {
        private static NotNullToTrueConverter _instance;
        public static NotNullToTrueConverter Instance => _instance ?? (_instance = new NotNullToTrueConverter());
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
    }
}