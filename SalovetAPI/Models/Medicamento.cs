namespace SalovetAPI.Models
{
    public class Medicamento
    {
        public int IdMedica { get; set; }
        public string NomMedica { get; set; } = string.Empty;
        public float Gramos { get; set; }
        public int Stock { get; set; }
        public bool Estado { get; set; }
    }
}
