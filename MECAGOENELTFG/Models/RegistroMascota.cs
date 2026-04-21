using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Models
{
    public class RegistroMascota
    {
        public int IdRegistro { get; set; }
        public int IdMascota { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFinal { get; set; }

        // Relación
        public Mascota Mascota { get; set; }


    }
}
