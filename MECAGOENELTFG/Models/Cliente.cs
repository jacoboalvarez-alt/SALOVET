using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Models
{
    public  class Cliente
    {
        public int IdCliente { get; set; }
        public string NombreCli { get; set; } = string.Empty;
        public string ApeCli { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string Correo { get; set; } = string.Empty;
        public string? Tel { get; set; }

        // Navegación
        public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();

        public Cliente(string nom, string apem, int edad, string mail,string tel) { 
            NombreCli = nom;
            ApeCli = apem;
            Edad = edad;
            Correo = mail;
            Tel = tel;
        }

        public Cliente() { }
    }
}
