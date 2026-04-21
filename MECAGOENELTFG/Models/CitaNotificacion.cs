using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Models
{
    public class CitaNotificacion
    {
        public Cita Cita { get; }
        public bool EsHoy { get; }

        // Verde para hoy, amarillo para próximas
        public Color ColorFondo => EsHoy
            ? Color.FromArgb("#D4EDDA")
            : Color.FromArgb("#FFF8DC");

        public Color ColorBorde => EsHoy
            ? Color.FromArgb("#28A745")
            : Color.FromArgb("#FFC107");

        public Color ColorEtiqueta => EsHoy
            ? Color.FromArgb("#155724")
            : Color.FromArgb("#856404");

        public string Etiqueta => EsHoy ? "HOY" : "PRÓXIMA";

        public string HoraTexto => Cita.FechaHora.ToString("HH:mm");

        public string FechaTexto => EsHoy
            ? "Hoy"
            : Cita.FechaHora.ToString("dd/MM/yyyy");

        public string MascotaNombre => Cita.Mascota?.NombreMasc ?? $"Mascota #{Cita.IdMascota}";
        public string ClienteNombre => Cita.Cliente?.NombreCli ?? $"Cliente #{Cita.IdCliente}";
        public string Descripcion => string.IsNullOrWhiteSpace(Cita.Descripcion)
            ? "Sin descripción"
            : Cita.Descripcion;

        public CitaNotificacion(Cita cita, bool esHoy)
        {
            Cita = cita;
            EsHoy = esHoy;
        }
    }
}
