using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Converters
{
    public class InvertedBoolConverter : IValueConverter
    {
        // Devuelve el valor booleano invertido (true → false, false → true)
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !(bool)(value ?? false);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !(bool)(value ?? false);
        }
    }
}
