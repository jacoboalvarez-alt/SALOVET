using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Converters
{
    internal class StockColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is int stock ? stock switch
            {
                0 => Color.FromArgb("#F44336"),   // rojo - sin stock
                <= 5 => Color.FromArgb("#FF9800"),   // naranja - crítico
                <= 15 => Color.FromArgb("#FFC107"),   // amarillo - bajo
                _ => Color.FromArgb("#4CAF50")    // verde - ok
            } : Color.FromArgb("#90A4AE");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
