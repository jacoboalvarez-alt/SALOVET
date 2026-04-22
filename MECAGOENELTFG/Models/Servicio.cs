using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Models
{
    public class Servicio
    {
        public int IdServicio { get; set; }
        public string NomServicio { get; set; } = string.Empty;
        public float Precio { get; set; }
        public string? Descripcion { get; set; }
    }
}
