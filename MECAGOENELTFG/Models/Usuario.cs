using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Models
{
    internal class Usuario
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Pass { get; set; }

        public bool Profesional { get; set; }
        public int? IdCliente { get; set; }

        // Navegación
        public Cliente? Cliente { get; set; }

        public Usuario(string user, string pas, bool p, int idCli, Cliente cli) {
            Username = user;    
            Pass = pas;
            Profesional = p;
            IdCliente = idCli;
            Cliente = cli;
        }

        public Usuario() { }
    }
}
