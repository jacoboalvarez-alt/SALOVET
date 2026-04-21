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
    public partial class FormCitasClienteViewModel : ObservableObject
    {
        //Servicios
        private readonly MascotaApiService _mascotaService;
        private readonly ProfesionalAPIService _profService;
        private readonly CitasAPIService _citasService;

        //Listas
        public ObservableCollection<Mascota> Mascotas { get; set; } = new();
        public ObservableCollection<Profesional> Veterinarios { get; set; } = new();
        public ObservableCollection<string> HorasDisponibles {  get; set; } = new();

        //Selecciones
        [ObservableProperty] private Mascota? mascotaSeleccionada;
        [ObservableProperty] private Profesional? profesionalSeleccionado;
        [ObservableProperty] private DateTime fechaSeleccionada = DateTime.Today.AddDays(1);
        [ObservableProperty] private string? horaSeleccionada;
        [ObservableProperty] private string? descripcion = string.Empty;

        //Estado UI
        [ObservableProperty] private bool isLoading = false;
        [ObservableProperty] private string mensajeError = string.Empty;

        //Fecha minima = mañana
        public DateTime FechaMinima => DateTime.Today.AddDays(1);
        public DateTime FechaMaxima => DateTime.Today.AddMonths(3);

        public FormCitasClienteViewModel() 
        {
            _mascotaService = new MascotaApiService();  
            _profService = new ProfesionalAPIService();
            _citasService = new CitasAPIService();  
        }

        public async Task CargarDatosAsync() 
        {
            IsLoading = true;
            int id_cliente = SessionService.IdClienteActual;
            var mascotas = await _mascotaService.ObtenerPorCliente(id_cliente);
            var veterinarios = await _profService.ObtenerVeterinarios();

            Mascotas.Clear();
            foreach (var m in mascotas) Mascotas.Add(m);

            Veterinarios.Clear();
            foreach (var v in veterinarios) Veterinarios.Add(v);

            IsLoading = false;
        }

        partial void OnFechaSeleccionadaChanged(DateTime value) => _ = ActualizarHorasAsync();
        partial void OnProfesionalSeleccionadoChanged(Profesional? value) => _ = ActualizarHorasAsync();

        private async Task ActualizarHorasAsync() 
        {
            HoraSeleccionada = null;
            HorasDisponibles.Clear();

            if (ProfesionalSeleccionado == null) return;

            var citasDelDia = await _citasService.ObtenerCitasPorFecha(FechaSeleccionada);
            var ocupadas = citasDelDia
                .Where(c => c.IdProf == ProfesionalSeleccionado.IdProf && c.Estado != EstadoCita.CANCELADA)
                .Select(c => c.FechaHora.Hour)
                .ToHashSet();

            for (int hora = 6; hora <= 22; hora++) 
            {
                if (!ocupadas.Contains(hora)) 
                {
                    HorasDisponibles.Add($"{hora}:00");
                }
            }
                      
        }

        [RelayCommand]
        private async Task Solicitar() 
        { 
            MensajeError = string.Empty;
            if (MascotaSeleccionada == null) { MensajeError = "No has seleccionado una mascota."; return; }
            if (ProfesionalSeleccionado == null) { MensajeError = "No has seleccionado un veterinario"; return; }
            if (string.IsNullOrWhiteSpace(HoraSeleccionada)) { MensajeError = "Tienes que elegir una hora disponible."; return; }

            var hora = int.Parse(HoraSeleccionada.Split(':')[0]);
            var fechaHora = new DateTime(
                FechaSeleccionada.Year,
                FechaSeleccionada.Month,
                FechaSeleccionada.Day,
                hora, 0, 0);

            var nueva2 = new CitaClient
            {
                IdCliente = SessionService.IdClienteActual,
                IdProf = ProfesionalSeleccionado.IdProf,
                IdMascota = MascotaSeleccionada.IdMascota,
                FechaHora = fechaHora,
                Estado = EstadoCita.PENDIENTE,
                Descripcion = string.IsNullOrWhiteSpace(Descripcion) ? null : Descripcion
            };

            IsLoading = true;
            var resultado = await _citasService.CrearCita2(nueva2);
            IsLoading = false;

            if (resultado != null)
            {
                await Shell.Current.DisplayAlert("¡Listo!", "Cita solicitada correctamente.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else 
            {
                MensajeError = "No se ha podido solicitar la cita. Intentalo de Nuevo";
            }
        }

        [RelayCommand]
        private async Task Cancelar() 
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
