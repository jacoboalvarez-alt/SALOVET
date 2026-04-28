namespace SalovetAPI.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Pass { get; set; }
        public bool Profesional { get; set; }


        // Navegación
        public int? IdCliente { get; set; }
        public Cliente? Cliente { get; set; }

        //FK para los profesionales
        public int? IdProf {  get; set; }
        public Profesional? ProfesionalNav { get; set; }

        //Boleano para 1er inicio de sesion
        public bool? Primero {  get; set; }

        public Usuario(int idUsuario, string username, string? pass, bool profesional,  int? idCliente, Cliente? cliente, int? idProf = null, Profesional? profesionalNav = null)
        {
            IdUsuario = idUsuario;
            Username = username;
            Pass = pass;
            Profesional = profesional;
            IdCliente = idCliente;
            Cliente = cliente;
            IdProf = idProf;
            ProfesionalNav = profesionalNav;
        }

        public Usuario() { }
    }
}
