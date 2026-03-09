namespace SalovetAPI.Models
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

        public Usuario(int idUsuario, string username, string? pass, bool profesional, int? idCliente, Cliente? cliente)
        {
            IdUsuario = idUsuario;
            Username = username;
            Pass = pass;
            Profesional = profesional;
            IdCliente = idCliente;
            Cliente = cliente;
        }

        public Usuario() { }
    }
}
