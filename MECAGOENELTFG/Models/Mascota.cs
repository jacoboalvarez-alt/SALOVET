using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Models
{
    public class Mascota
    {
        public int IdMascota { get; set; }
        public int IdCliente { get; set; }
        public string NombreMasc { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty;
        public string? Raza { get; set; }
        public int? Edad { get; set; }

        [JsonIgnore]  // Esto rompe la referencia circular
        public Cliente? Cliente { get; set; }


        public Mascota() { }

        public Mascota(int idMascota, int idCliente, string nombreMasc, string especie, string? raza, int? edad, Cliente cliente)
        {
            IdMascota = idMascota;
            IdCliente = idCliente;
            NombreMasc = nombreMasc;
            Especie = especie;
            Raza = raza;
            Edad = edad;
            Cliente = cliente;
        }
    }
}
