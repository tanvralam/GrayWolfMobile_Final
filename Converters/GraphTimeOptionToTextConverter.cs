using GrayWolf.Enums;
using System;
using System.Globalization;


namespace GrayWolf.Converters
{
    public class GraphTimeOptionToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
                if (!(value is GraphTimeOption option))
            {
                throw new ArgumentException();                
            }

            switch (option)
            {
                case GraphTimeOption.FifteenMinutes:
                    return Localization.Localization.Chart_FifteenMinutes;
                case GraphTimeOption.OneHour:
                    return Localization.Localization.Chart_OneHour;
                case GraphTimeOption.EightHours:
                    return Localization.Localization.Chart_EightHours;
                case GraphTimeOption.Day:
                    return Localization.Localization.Chart_Day;
                case GraphTimeOption.Everything:
                    return Localization.Localization.Chart_Everything;
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
