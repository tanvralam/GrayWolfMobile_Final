using GrayWolf.Enums;
using System;
using System.Globalization;


namespace GrayWolf.Converters
{
    public class ZipProtectionModeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           if(value==null)
            {
                return "";
            }
            if (!(value is ZipProtectionMode mode))
            {
                throw new ArgumentException();
            }

            switch (mode)
            {
                case ZipProtectionMode.NoEncryption:
                    return Localization.Localization.Password_NoEncryption;
                case ZipProtectionMode.DefaultPassword:
                    return Localization.Localization.Password_DefaultPassword;
                case ZipProtectionMode.CustomPassword:
                    return Localization.Localization.Password_CustomPassword;
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
