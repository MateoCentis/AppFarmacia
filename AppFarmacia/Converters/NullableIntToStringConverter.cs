using System.Globalization;

namespace AppFarmacia.Converters
{
    public class NullableIntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "-";
            }

            // Verificar si es int directamente
            if (value is int intValue)
            {
                return intValue.ToString();
            }

            // Verificar si es un tipo nullable int
            var type = value.GetType();
            if (type == typeof(int?) || Nullable.GetUnderlyingType(type) == typeof(int))
            {
                var nullableInt = (int?)value;
                if (nullableInt.HasValue)
                {
                    return nullableInt.Value.ToString();
                }
                return "-";
            }

            return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

