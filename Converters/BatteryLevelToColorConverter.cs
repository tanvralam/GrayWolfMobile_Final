using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;



namespace GrayWolf.Converters
{
    public class BatteryLevelToColorConverter : IMarkupExtension, IValueConverter
    {
        private readonly Color _defaultColor;
        private readonly Color _okColor;
        private readonly Color _lowColor;
        private readonly Color _criticalColor;


        private static BatteryLevelToColorConverter _instance;
        public static BatteryLevelToColorConverter Instance => _instance ?? (_instance = new BatteryLevelToColorConverter());
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }

        public BatteryLevelToColorConverter()
        {
            _defaultColor = (Color)Application.Current.Resources["BatteryColorDefault"];
            _lowColor = (Color)Application.Current.Resources["BatteryColorLow"];
            _criticalColor = (Color)Application.Current.Resources["BatteryColorCritical"];
            _okColor = (Color)Application.Current.Resources["BatteryColorOk"];

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null) return _defaultColor;
            switch (value.ToString())
            {
                case "OK":
                    {
                        return _okColor;
                    }
                case "Low":
                    {
                        return _lowColor;
                    }
                case "Critical":
                    {
                        return _criticalColor;
                    }
                default:
                    {
                        return _criticalColor;
                    }

            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
