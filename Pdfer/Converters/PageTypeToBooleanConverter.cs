using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Tooler.Common.Enum;

namespace Tooler.Converters
{
    public class PageTypeToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PageType selectedPage && Enum.TryParse(parameter.ToString(), out PageType pageType))
            {
                return selectedPage == pageType;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Enum.Parse(typeof(PageType), parameter.ToString());
            }
            return Binding.DoNothing;
        }
    }
}
