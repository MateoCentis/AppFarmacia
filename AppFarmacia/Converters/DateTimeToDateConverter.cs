using System.Globalization;
namespace AppFarmacia.Converters
{
    public class DateTimeToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return dateTime; // Retorna el valor si es un DateTime
            }
            return DateTime.Now; // Valor por defecto si no es válido
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return dateTime; // Retorna el valor si es un DateTime
            }
            return DateTime.Now; // Valor por defecto si no es válido
        }
    }
}
