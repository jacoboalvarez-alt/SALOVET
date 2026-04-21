namespace SalovetAPI.Models
{
    public class RegistroMascota
    {
        public int IdRegistro { get; set; }
        public int IdMascota { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFinal { get; set; }

        // Relación
        public Mascota? Mascota { get; set; }
    }
}
