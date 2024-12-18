using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tooler.Converters
{
    public sealed class TypeToStringConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            /*if (value.GetType() == typeof(Type))*/
            if (value is Type)
            {
                return (value as Type).Name;
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            if (value.GetType() == typeof(Type))
            {
                return (value as Type).Name;
            }
            else
            {
                return string.Empty;
            }
        }

        /*public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            if (value.ToString().Equals("string", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(string);
            }
            else if (value.ToString().Equals("int", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(int);
            }
            else if (value.ToString().Equals("DateTime", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(DateTime);
            }
            else if (value.ToString().Equals("decimal", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(decimal);
            }
            else if (value.ToString().Equals("double", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(double);
            }
            else if (value.ToString().Equals("float", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(float);
            }
            else if (value.ToString().Equals("long", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(long);
            }
            else if (value.ToString().Equals("bool", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(bool);
            }
            else
            {
                return null;
            }
        }*/
    }
}
