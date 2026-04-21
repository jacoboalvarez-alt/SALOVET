using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;
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
        private readonly CitasAPIService _citaService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MostrarProximas))]
        [NotifyPropertyChangedFor(nameof(MostrarPasadas))]
        private string filtroActivo = "Todas";

        public bool MostrarProximas => FiltroActivo is "Todas" or "Próximas";
        public bool MostrarPasadas => FiltroActivo is "Todas" or "Pasadas";

        [ObservableProperty] private bool sinCitasProximas;

        public ObservableCollection<CitaItem> CitasProximas { get; } = new();
        public ObservableCollection<CitaItem> CitasPasadas { get; } = new();

        public ClienteCitasViewModel()
        {
            _citaService = new CitasAPIService();
        }

        public async Task CargarDatosAsync()
        {
            int id = SessionService.IdClienteActual;
            if (id == 0) return;

            var citas = await _citaService.ObtenerCitasPorCliente(id); // era ObtenerPorCliente
            var ahora = DateTime.Now;

            CitasProximas.Clear();
            CitasPasadas.Clear();

            foreach (var c in citas)
            {
                var item = new CitaItem
                {
                    Id = c.IdCita,
                    Titulo = c.Descripcion ?? "Cita veterinaria",
                    Fecha = c.FechaHora,
                    Veterinario = c.Profesional != null
                                    ? $"{c.Profesional.NomProf} {c.Profesional.ApeProf}"
                                    : "Veterinario",
                    NombreMascota = c.Mascota != null
                                    ? $"{c.Mascota.NombreMasc} · {c.Mascota.Especie}"
                                    : "Mascota",
                    Estado = c.Estado
                };

                if (c.FechaHora >= ahora &&
                    c.Estado is EstadoCita.PENDIENTE or EstadoCita.CONFIRMADA)
                    CitasProximas.Add(item);
                else
                    CitasPasadas.Add(item);
            }

            SinCitasProximas = CitasProximas.Count == 0;
        }

        [RelayCommand]
        private void CambiarFiltro(string filtro) => FiltroActivo = filtro;

        [RelayCommand]
        private async Task SolicitarCita() =>
            await Shell.Current.GoToAsync("FormCitasCliente");

        [RelayCommand]
        private async Task ModificarCita(CitaItem cita) =>
            await Shell.Current.GoToAsync("ModificarCitaPageClient",
                new Dictionary<string, object> { { "CitaActual", cita } });

        [RelayCommand]
        private async Task CancelarCita(CitaItem cita)
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Cancelar cita",
                $"¿Seguro que quieres cancelar la cita del {cita.FechaFormateada}?",
                "Sí, cancelar", "Volver");
            if (!confirmar) return;

            bool ok = await _citaService.CambiarEstadoCita(cita.Id, "CANCELADA");
            if (ok)
            {
                CitasProximas.Remove(cita);
                SinCitasProximas = CitasProximas.Count == 0;
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo cancelar la cita.", "OK");
            }
        }
    }

    public class CitaItem
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Veterinario { get; set; } = string.Empty;
        public string NombreMascota { get; set; } = string.Empty;
        public EstadoCita Estado { get; set; }

        public string Dia => Fecha.Day.ToString("D2");
        public string MesCorto => Fecha.ToString("MMM").ToUpper()[..3];
        public string HoraYVeterinario => $"{Fecha:HH:mm} · {Veterinario}";
        public string FechaFormateada => Fecha.ToString("d 'de' MMMM");
    }
}
