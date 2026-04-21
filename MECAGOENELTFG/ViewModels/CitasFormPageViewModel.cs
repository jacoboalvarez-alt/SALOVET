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

namespace MECAGOENELTFG.ViewModels
{
    public partial class CitasFormPageViewModel : ObservableObject
    {
        //Servicios
        private readonly ClienteApiService _clienteService;
        private readonly MascotaApiService _macotaService;
        private readonly ProfesionalAPIService _profService;
        private readonly CitasAPIService _citasService;

        //Listas
        public ObservableCollection<Cliente> Clientes { get; set; } = new();
        public ObservableCollection<Mascota> Mascotas { get; set; } = new();
        public ObservableCollection<Profesional> Veterinarios {  get; set; } = new();
        public ObservableCollection<string> HorasDisponibles { get; set; } = new();

        //Selecciones
        [ObservableProperty] private Cliente? clienteSeleccionado;
        [ObservableProperty] private Mascota? mascotaSeleccionada;
        [ObservableProperty] private Profesional? profesionalSeleccionado;
        [ObservableProperty] private DateTime fechaSeleccionada = DateTime.Today.AddDays(1);
        [ObservableProperty] private string? horaSeleccionada;
        [ObservableProperty] private string? descripcion = string.Empty;

        //Estado de la UI
        [ObservableProperty] private bool isLoading = false;
        [ObservableProperty] private bool hayMascotas = false;
        [ObservableProperty]private string mensajeError = string.Empty;

        //Fecha limite
        public DateTime FechaMinima => DateTime.Today.AddDays(1);
        public DateTime FechaMaxima => DateTime.Today.AddMonths(3);

        public CitasFormPageViewModel() 
        { 
           _clienteService = new ClienteApiService();
            _macotaService = new MascotaApiService();
            _profService = new ProfesionalAPIService();
            _citasService = new CitasAPIService();
        }

        public async Task CargarDatosAsync() 
        {
           IsLoading = true;

            var clientes = await _clienteService.ObtenerTodos();
            var veterinarios = await _profService.ObtenerVeterinarios();

            Clientes.Clear();
            foreach (var c in clientes) Clientes.Add(c);

            Veterinarios.Clear();
            foreach (var v in veterinarios) Veterinarios.Add(v);

            IsLoading = false;
        }

        // Al cambiar cliente → cargar sus mascotas
        partial void OnClienteSeleccionadoChanged(Cliente? value) => _ = CargarMascotasAsync(value);

        // Al cambiar fecha o veterinario → actualizar horas
        partial void OnFechaSeleccionadaChanged(DateTime value) => _ = ActualizarHorasAsync();
        partial void OnProfesionalSeleccionadoChanged(Profesional? value) => _ = ActualizarHorasAsync();


        private async Task CargarMascotasAsync(Cliente? cliente) 
        {
            MascotaSeleccionada = null;
            Mascotas.Clear();

            if (cliente == null) 
            { HayMascotas = false; return; }

            IsLoading = true;
            var lista = await _macotaService.ObtenerPorCliente(cliente.IdCliente);
            foreach (var m in lista) Mascotas.Add(m);
            HayMascotas = Mascotas.Count > 0;
            IsLoading = false;
        }

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

            for (int hora = 6; hora < 22; hora++) 
            {
                if (!ocupadas.Contains(hora))
                    HorasDisponibles.Add($"{hora}:00");
            }
        }

        [RelayCommand]
        private async Task Solicitar() 
        {
            MensajeError = string.Empty;


            if (ClienteSeleccionado == null) { MensajeError = "Selecciona un cliente."; return; }
            if (MascotaSeleccionada == null) { MensajeError = "Selecciona una mascota."; return; }
            if (ProfesionalSeleccionado == null) { MensajeError = "Selecciona un veterinario."; return; }
            if (string.IsNullOrWhiteSpace(HoraSeleccionada)) { MensajeError = "Selecciona una hora disponible."; return; }

            var hora = int.Parse(HoraSeleccionada.Split(':')[0]);
            var fechaHora = new DateTime(
                FechaSeleccionada.Year,
                FechaSeleccionada.Month,
                FechaSeleccionada.Day,
                hora, 0, 0);

            var nueva = new CitaClient
            {
                IdCliente = ClienteSeleccionado.IdCliente,
                IdProf = ProfesionalSeleccionado.IdProf,
                IdMascota = MascotaSeleccionada.IdMascota,
                FechaHora = fechaHora,
                Estado = EstadoCita.PENDIENTE,
                Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : Descripcion
            };

            IsLoading = true;
            var resultado = await _citasService.CrearCita2(nueva);
            IsLoading = false;

            if(resultado != null)
            {
                await Shell.Current.DisplayAlert("!Listo!", "Cita creada correctamente", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else 
            {
                MensajeError = "No se ha podido crear la cita. Intentalo de nuevo,";
            }
        }

        [RelayCommand]
        private async Task Cancelar() 
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
