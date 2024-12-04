using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace AppFarmacia.Converters
{
    public class LeidoToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool leido)
            {
                return leido ? Colors.LightGray : Colors.LightBlue;
            }
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
