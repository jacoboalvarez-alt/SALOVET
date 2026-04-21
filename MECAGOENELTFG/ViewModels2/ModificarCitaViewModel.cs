using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;
using System.Collections.ObjectModel;

namespace MECAGOENELTFG.ViewModels2
{
    [QueryProperty(nameof(CitaActual), "CitaActual")]
    public partial class ModificarCitaViewModel : ObservableObject
    {
        private readonly CitasAPIService _citasService;

        // Cita recibida por navegación
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(InfoCita))]
        private CitaItem? citaActual;

        // Listas
        public ObservableCollection<string> HorasDisponibles { get; } = new();

        // Selecciones
        [ObservableProperty] private DateTime fechaSeleccionada = DateTime.Today.AddDays(1);
        [ObservableProperty] private string? horaSeleccionada;

        // Estado UI
        [ObservableProperty] private bool isLoading = false;
        [ObservableProperty] private string mensajeError = string.Empty;

        // Límites fecha
        public DateTime FechaMinima => DateTime.Today.AddDays(1);
        public DateTime FechaMaxima => DateTime.Today.AddMonths(3);

        // Texto resumen de la cita actual
        public string InfoCita => CitaActual != null
            ? $"{CitaActual.Titulo}\n{CitaActual.FechaFormateada} · {CitaActual.Fecha:HH:mm}\n{CitaActual.Veterinario}\n{CitaActual.NombreMascota}"
            : string.Empty;

        public ModificarCitaViewModel()
        {
            _citasService = new CitasAPIService();
        }

        // Se dispara cuando llega la cita por QueryProperty
        partial void OnCitaActualChanged(CitaItem? value)
        {
            if (value != null)
            {
                // Precargamos la fecha actual de la cita como punto de partida
                FechaSeleccionada = value.Fecha.Date > FechaMinima
                    ? value.Fecha.Date
                    : FechaMinima;

                _ = ActualizarHorasAsync();
            }
        }

        partial void OnFechaSeleccionadaChanged(DateTime value) => _ = ActualizarHorasAsync();

        private async Task ActualizarHorasAsync()
        {
            HoraSeleccionada = null;
            HorasDisponibles.Clear();

            if (CitaActual == null) return;

            // Necesitamos el IdProf — lo obtenemos de la cita completa via API
            var citaCompleta = await _citasService.ObtenerCita(CitaActual.Id);
            if (citaCompleta == null) return;

            var citasDelDia = await _citasService.ObtenerCitasPorFecha(FechaSeleccionada);

            var ocupadas = citasDelDia
                .Where(c => c.IdProf == citaCompleta.IdProf
                         && c.Estado != EstadoCita.CANCELADA
                         && c.IdCita != CitaActual.Id) // excluimos la propia cita
                .Select(c => c.FechaHora.Hour)
                .ToHashSet();

            for (int hora = 6; hora <= 22; hora++)
            {
                if (!ocupadas.Contains(hora))
                    HorasDisponibles.Add($"{hora}:00");
            }
        }

        [RelayCommand]
        private async Task Modificar()
        {
            MensajeError = string.Empty;

            if (CitaActual == null) return;

            if (string.IsNullOrWhiteSpace(HoraSeleccionada))
            {
                MensajeError = "Tienes que elegir una hora disponible.";
                return;
            }

            var hora = int.Parse(HoraSeleccionada.Split(':')[0]);
            var nuevaFechaHora = new DateTime(
                FechaSeleccionada.Year,
                FechaSeleccionada.Month,
                FechaSeleccionada.Day,
                hora, 0, 0);

            // Obtenemos la cita completa para el PUT
            IsLoading = true;
            var citaCompleta = await _citasService.ObtenerCita(CitaActual.Id);
            if (citaCompleta == null)
            {
                MensajeError = "No se pudo obtener la cita. Inténtalo de nuevo.";
                IsLoading = false;
                return;
            }

            citaCompleta.FechaHora = nuevaFechaHora;
            var ok = await _citasService.ActualizarCita(CitaActual.Id, citaCompleta);
            IsLoading = false;

            if (ok)
            {
                await Shell.Current.DisplayAlert("¡Listo!", "Cita modificada correctamente.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                MensajeError = "No se pudo modificar la cita. Inténtalo de nuevo.";
            }
        }

        [RelayCommand]
        private async Task Cancelar() => await Shell.Current.GoToAsync("..");
    }
}