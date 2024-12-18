using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tooler.Converters
{
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null|| !(value is Boolean))
            {
                return Visibility.Collapsed;
            }
            if ((Boolean)value==true)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Boolean))
            {
                return Visibility.Collapsed;
            }
            if ((Boolean)value == true)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
    }
}
