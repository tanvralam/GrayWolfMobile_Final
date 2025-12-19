using System;
using System.Globalization;



namespace GrayWolf.Converters
{
    public class ImageSourceConverterBase
    {
        protected string GetFormattedValue(string value)
        {
            var formattedName = value;
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                formattedName = $"{value}.png";
            }
            return formattedName;
        }
    }

    [ContentProperty("Name")]
    public class ImageSourceExtension : ImageSourceConverterBase, IMarkupExtension<string>
    {
        public string Name { get; set; }

        public string ProvideValue(IServiceProvider serviceProvider)
        {
            return GetFormattedValue(Name);
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<string>).ProvideValue(serviceProvider);
        }
    }

    public class ImageSourceConverter : ImageSourceConverterBase, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetFormattedValue($"{value}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
