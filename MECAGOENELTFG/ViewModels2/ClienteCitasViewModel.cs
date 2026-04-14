using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.ViewModels2
{
    public partial class ClienteCitasViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MostrarProximas))]
        [NotifyPropertyChangedFor(nameof(MostrarPasadas))]
        private string filtroActivo = "Todas";

        public bool MostrarProximas => FiltroActivo is "Todas" or "Próximas";
        public bool MostrarPasadas => FiltroActivo is "Todas" or "Pasadas";

        [ObservableProperty]
        private bool sinCitasProximas;

        public ObservableCollection<CitaItem> CitasProximas { get; } = new();
        public ObservableCollection<CitaItem> CitasPasadas { get; } = new();

        public void ClientesCitasViewModel()
        {
            CargarDatosEjemplo();
        }

        private void CargarDatosEjemplo()
        {
            CitasProximas.Add(new CitaItem
            {
                Id = 1,
                Titulo = "Revisión anual",
                Fecha = new DateTime(2026, 4, 15, 10, 30, 0),
                Veterinario = "Dr. García",
                NombreMascota = "Rocky · Golden Retriever"
            });
            CitasProximas.Add(new CitaItem
            {
                Id = 2,
                Titulo = "Desparasitación",
                Fecha = new DateTime(2026, 5, 3, 9, 0, 0),
                Veterinario = "Dr. López",
                NombreMascota = "Luna · Siamés"
            });

            CitasPasadas.Add(new CitaItem
            {
                Id = 3,
                Titulo = "Vacunación",
                Fecha = new DateTime(2026, 3, 22, 9, 0, 0),
                Veterinario = "Dr. López",
                NombreMascota = "Rocky · Golden Retriever"
            });
            CitasPasadas.Add(new CitaItem
            {
                Id = 4,
                Titulo = "Consulta general",
                Fecha = new DateTime(2026, 2, 10, 11, 0, 0),
                Veterinario = "Dr. García",
                NombreMascota = "Luna · Siamés"
            });

            SinCitasProximas = CitasProximas.Count == 0;
        }

        [RelayCommand]
        private void CambiarFiltro(string filtro) => FiltroActivo = filtro;

        [RelayCommand]
        private async Task SolicitarCita()
        {
            await Shell.Current.GoToAsync("SolicitarCitaPage");
        }

        [RelayCommand]
        private async Task ModificarCita(CitaItem cita)
        {
            await Shell.Current.GoToAsync($"ModificarCitaPage?id={cita.Id}");
        }

        [RelayCommand]
        private async Task CancelarCita(CitaItem cita)
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Cancelar cita",
                $"¿Seguro que quieres cancelar la cita del {cita.FechaFormateada}?",
                "Sí, cancelar", "Volver");

            if (!confirmar) return;

            CitasProximas.Remove(cita);
            SinCitasProximas = CitasProximas.Count == 0;
        }
    }

    public class CitaItem
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Veterinario { get; set; } = string.Empty;
        public string NombreMascota { get; set; } = string.Empty;

        public string Dia => Fecha.Day.ToString("D2");
        public string MesCorto => Fecha.ToString("MMM").ToUpper()[..3];
        public string HoraYVeterinario => $"{Fecha:HH:mm} · {Veterinario}";
        public string FechaFormateada => Fecha.ToString("d 'de' MMMM");
    }
}
