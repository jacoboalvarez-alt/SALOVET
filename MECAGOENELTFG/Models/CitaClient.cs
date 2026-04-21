using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Models
{
    public class CitaClient
    {
        public int IdCita { get; set; }
        public int IdCliente { get; set; }
        public int IdProf { get; set; }
        public int IdMascota { get; set; }
        public DateTime FechaHora { get; set; }
        public EstadoCita Estado { get; set; } = EstadoCita.PENDIENTE;
        public string? Descripcion { get; set; }

        // Navegación
        [JsonIgnore]
        public Cliente? Cliente { get; set; } = null!;
        [JsonIgnore]
        public Profesional? Profesional { get; set; } = null!;
        [JsonIgnore]
        public Mascota? Mascota { get; set; } = null!;
    }
}
