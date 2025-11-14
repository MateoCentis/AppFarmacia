using System.Globalization;

namespace AppFarmacia.Converters
{
    public class DateOnlyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Manejar null explícitamente
            if (value == null)
            {
                return "-";
            }
            
            if (value is DateOnly dateOnly)
            {
                return dateOnly.ToString("dd/MM/yyyy");
            }
            
            // Si no es DateOnly, mostrar texto amigable
            return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

