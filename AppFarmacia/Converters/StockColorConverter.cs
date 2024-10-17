using System.Globalization;
namespace AppFarmacia.Converters
{
    public class StockColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int ultimoStock && parameter is int puntoReposicion)
            {
                return ultimoStock < puntoReposicion;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
