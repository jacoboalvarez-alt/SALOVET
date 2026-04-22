namespace SalovetAPI.Models
{
    public class Servicio
    {
        public int IdServicio { get; set; }
        public string NomServicio { get; set; } = string.Empty;
        public float Precio { get; set; }
        public string? Descripcion { get; set; }
    }
}
