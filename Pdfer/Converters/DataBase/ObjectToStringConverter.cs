using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tooler.Converters
{
    public sealed class ObjectToStringConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                if (value is Object)
                {
                    String fm = value.ToString();
                    return fm;
                }
                else return null;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                if (value is Object)
                {
                    String fm = value.ToString();
                    return fm;
                }
                else return null;
            }
            else
            {
                return null;
            }
        }
    }
}
