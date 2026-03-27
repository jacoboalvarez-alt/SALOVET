using MECAGOENELTFG.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Converters
{
    internal class EstadoColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is EstadoCita estado ? estado switch
            {
                EstadoCita.PENDIENTE => Color.FromArgb("#FF9800"),
                EstadoCita.CONFIRMADA => Color.FromArgb("#4CAF50"),
                EstadoCita.CANCELADA => Color.FromArgb("#F44336"),
                EstadoCita.COMPLETADA => Color.FromArgb("#2EBBB0"),
                _ => Color.FromArgb("#90A4AE")
            } : Color.FromArgb("#90A4AE");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
