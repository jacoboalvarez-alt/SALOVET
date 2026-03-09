namespace SalovetAPI.Models
{
    public class Mascota
    {
        public int IdMascota { get; set; }
        public int IdCliente { get; set; }
        public string NombreMasc { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty;
        public string? Raza { get; set; }
        public int? Edad { get; set; }

        // Navegación
        public Cliente Cliente { get; set; } = null!;
    }
}
