using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Tooler.Common.Enum;

namespace Tooler.Converters
{
    public class MenuPageTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MenuPageType selectedPage && Enum.TryParse(parameter.ToString(), out MenuPageType pageType))
            {
                if (selectedPage == pageType)
                {
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(190, 230, 253));  // 返回想要的背景色
                }
                else
                {
                    return new SolidColorBrush(Colors.AliceBlue); 
                }
                
            }
            return new SolidColorBrush(Colors.AliceBlue); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Enum.Parse(typeof(MenuPageType), parameter.ToString());
            }
            return Binding.DoNothing;
        }
    }
}
