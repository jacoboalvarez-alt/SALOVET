namespace SalovetAPI.Models
{
    public class Registro
    {
        public int IdRegistro { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string? TipoActividad { get; set; }
    }
}
