namespace SalovetAPI.Models
{
    public enum GradoProfesional
    {
        VETERINARIO,
        AYUDANTE,
        PROFESIONAL
    }

    public class Profesional
    {
        public int IdProf { get; set; }
        public string NomProf { get; set; } = string.Empty;
        public string ApeProf { get; set; } = string.Empty;
        public int? Edad { get; set; }
        public string Correo { get; set; } = string.Empty;
        public GradoProfesional Grado { get; set; }
    }
}
