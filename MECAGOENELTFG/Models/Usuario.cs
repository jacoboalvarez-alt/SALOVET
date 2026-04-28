using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Pass { get; set; }

        public bool Profesional { get; set; }
        public int? IdCliente { get; set; }

        // Navegación
        public Cliente? Cliente { get; set; }

        public int? IdProf {  get; set; }

        //Primer inicio de sesion
        public bool Primero { get; set; }

        public Usuario(string user, string pass, bool profesional, int? idCliente, Cliente? cliente, int? idProf = null)
        {
            Username = user;
            Pass = pass;
            Profesional = profesional;
            IdCliente = idCliente;
            Cliente = cliente;
            IdProf = idProf;
        }

        public Usuario() { }
    }
}
