using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Models
{
    internal class Medicamento
    {
        public int IdMedica { get; set; }
        public string NomMedica { get; set; } = string.Empty;
        public float Gramos { get; set; }
        public int Stock { get; set; }
        public bool Estado { get; set; }

    }
}
