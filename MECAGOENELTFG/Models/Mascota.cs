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

        public string? Sexo { get; set; }
        public string? Color { get; set; }
        public string? Tamano { get; set; }
        public string? TipoPelo { get; set; }
        public bool Vacunado { get; set; }
        public string? Notas { get; set; }

        [JsonIgnore]  // Esto rompe la referencia circular
        public Cliente? Cliente { get; set; }

        public List<object> RegistrosMascota { get; set; } = new();
        public Mascota() { }

        public Mascota(int idMascota, int idCliente, string nombreMasc, string especie, string? raza, int? edad, string? sexo, string? color, string? tamano, string? tipoPelo, bool vacunado, string? notas, Cliente? cliente)
        {
            IdMascota = idMascota;
            IdCliente = idCliente;
            NombreMasc = nombreMasc;
            Especie = especie;
            Raza = raza;
            Edad = edad;
            Sexo = sexo;
            Color = color;
            Tamano = tamano;
            TipoPelo = tipoPelo;
            Vacunado = vacunado;
            Notas = notas;
            Cliente = cliente;
        }
    }
}
