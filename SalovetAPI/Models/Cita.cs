using System.Text.Json.Serialization;

namespace SalovetAPI.Models
{
    public enum EstadoCita
    {
        PENDIENTE,
        CONFIRMADA,
        CANCELADA,
        COMPLETADA
    }

    public class Cita
    {
        public int IdCita { get; set; }
        public int IdCliente { get; set; }
        public int IdProf { get; set; }
        public int IdMascota { get; set; }
        public DateTime FechaHora { get; set; }
        public EstadoCita Estado { get; set; } = EstadoCita.PENDIENTE;
        public string? Descripcion { get; set; }

        // Navegación
        public Cliente? Cliente { get; set; } = null!;
        public Profesional? Profesional { get; set; } = null!;
        public Mascota? Mascota { get; set; } = null!;
    }
}
