using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Converters
{
    internal class BoolToEstadoBgConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && b
                ? Color.FromArgb("#4CAF50")
                : Color.FromArgb("#90A4AE");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
