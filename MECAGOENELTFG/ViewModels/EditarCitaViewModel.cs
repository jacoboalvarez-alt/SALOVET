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
using System.Windows.Input;

namespace MECAGOENELTFG.ViewModels
{
    public partial class EditarCitaViewModel : ObservableObject
    {
        private readonly CitasAPIService _citasService;
        private readonly ProfesionalAPIService _profService;

        public string NombreCliente { get; }
        public string NombreMascota {  get; }
        public string FechaHoraTexto { get; }

        [ObservableProperty] private Profesional? profesionalSeleccionado;
        [ObservableProperty] private EstadoCita estadoSeleccionado;
        [ObservableProperty] private string? descripcion;
        [ObservableProperty] private bool isLoading;

        // Listas para los Pickers
        public ObservableCollection<Profesional> Veterinarios { get; } = new();
        public List<EstadoCita> EstadosDisponibles { get; } = Enum.GetValues<EstadoCita>().ToList();

        private readonly Cita _citaOriginal;

        public ICommand VolverCommand { get; }

        public EditarCitaViewModel(Cita cita)
        {
            _citasService = new CitasAPIService();
            _profService = new ProfesionalAPIService();
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

            _citaOriginal = cita;

            // Campos informativos
            NombreCliente = cita.Cliente != null
                ? $"{cita.Cliente.NombreCli} {cita.Cliente.ApeCli}"
                : $"Cliente #{cita.IdCliente}";

            NombreMascota = cita.Mascota?.NombreMasc ?? $"Mascota #{cita.IdMascota}";
            FechaHoraTexto = cita.FechaHora.ToString("dd/MM/yyyy HH:mm");

            // Campos editables
            EstadoSeleccionado = cita.Estado;
            Descripcion = cita.Descripcion;
        }

        public async Task CargarDatosAsync()
        {
            IsLoading = true;

            var veterinarios = await _profService.ObtenerVeterinarios();
            Veterinarios.Clear();
            foreach (var v in veterinarios) Veterinarios.Add(v);

            // Selecciona el profesional actual
            ProfesionalSeleccionado = Veterinarios
                .FirstOrDefault(v => v.IdProf == _citaOriginal.IdProf);

            IsLoading = false;
        }

        [RelayCommand]
        private async Task Guardar()
        {
            if (ProfesionalSeleccionado == null)
            {
                await Shell.Current.DisplayAlert("Error", "Selecciona un veterinario.", "OK");
                return;
            }

            var citaActualizada = new Cita
            {
                IdCita = _citaOriginal.IdCita,
                IdCliente = _citaOriginal.IdCliente,
                IdMascota = _citaOriginal.IdMascota,
                IdProf = ProfesionalSeleccionado.IdProf,
                FechaHora = _citaOriginal.FechaHora,
                Estado = EstadoSeleccionado,
                Descripcion = string.IsNullOrWhiteSpace(Descripcion) ? null : Descripcion
            };

            IsLoading = true;
            bool ok = await _citasService.ActualizarCita(_citaOriginal.IdCita, citaActualizada);
            IsLoading = false;

            if (ok)
            {
                await Shell.Current.DisplayAlert("Éxito", "Cita actualizada correctamente.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo actualizar la cita.", "OK");
            }
        }
    }
}
