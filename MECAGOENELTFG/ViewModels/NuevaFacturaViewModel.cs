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

    public class ProfesionalPickerItem
    {
        public Profesional Profesional { get; }
        public string NombreCompleto => $"{Profesional.NomProf} {Profesional.ApeProf}".Trim();

        public ProfesionalPickerItem(Profesional profesional) => Profesional = profesional;
    }

    public class CitaPickerItem
    {
        public Cita Cita { get; }

        // Formato: "Juan García tratamiento a Toby: Fecha 12/04/2025 10:30"
        public string ResumenCita =>
            $"{NombreCliente} tratamiento a {NombreMascota}: Fecha {Cita.FechaHora:dd/MM/yyyy HH:mm}";

        private string NombreCliente => Cita.Cliente is not null
            ? $"{Cita.Cliente.NombreCli} {Cita.Cliente.ApeCli}".Trim()
            : $"Cliente #{Cita.IdCliente}";

        private string NombreMascota => Cita.Mascota?.NombreMasc ?? $"Mascota #{Cita.IdMascota}";

        public CitaPickerItem(Cita cita) => Cita = cita;
    }

    public class ServicioPickerItem
    {
        public Servicio Servicio { get; }
        public string ResumenServicio => $"{Servicio.NomServicio}  —  {Servicio.Precio:C2}";

        public ServicioPickerItem(Servicio servicio) => Servicio = servicio;
    }

    // ─── Item en la lista de servicios añadidos ───────────────────────────────────

    public class ServicioAgregadoViewModel
    {
        public Servicio Servicio { get; }
        public string NomServicio => Servicio.NomServicio;
        public float Precio => Servicio.Precio;

        public ServicioAgregadoViewModel(Servicio servicio) => Servicio = servicio;
    }

    // ─── ViewModel principal ──────────────────────────────────────────────────────

    public partial class NuevaFacturaViewModel : ObservableObject
    {
        private readonly ProfesionalAPIService _profesionalService;
        private readonly CitasAPIService _citasService;
        private readonly ServiciosService _serviciosService;
        private readonly FacturasService _facturasService;

        public NuevaFacturaViewModel(
            ProfesionalAPIService profesionalService,
            CitasAPIService citasService,
            ServiciosService serviciosService,
            FacturasService facturasService)
        {
            _profesionalService = profesionalService;
            _citasService = citasService;
            _serviciosService = serviciosService;
            _facturasService = facturasService;

            CargarDatosIniciales();
        }

        // ── Picker 1: Profesionales ───────────────────────────────────────────────

        [ObservableProperty]
        private ObservableCollection<ProfesionalPickerItem> _profesionales = [];

        [ObservableProperty]
        private ProfesionalPickerItem? _profesionalSeleccionado;

        partial void OnProfesionalSeleccionadoChanged(ProfesionalPickerItem? value)
        {
            // Limpiamos la selección de cita al cambiar de profesional
            CitaSeleccionada = null;
            Citas.Clear();
            SinCitas = false;

            if (value is not null)
                CargarCitasCommand.Execute(value.Profesional.IdProf);

            ActualizarPuedeCrear();
        }

        // ── Picker 2: Citas ───────────────────────────────────────────────────────

        [ObservableProperty]
        private ObservableCollection<CitaPickerItem> _citas = [];

        [ObservableProperty]
        private CitaPickerItem? _citaSeleccionada;

        [ObservableProperty]
        private bool _cargandoCitas;

        [ObservableProperty]
        private bool _hayCitas;

        [ObservableProperty]
        private bool _sinCitas;

        partial void OnCitaSeleccionadaChanged(CitaPickerItem? value) => ActualizarPuedeCrear();

        [RelayCommand]
        private async Task CargarCitas(int idProfesional)
        {
            CargandoCitas = true;
            HayCitas = false;
            SinCitas = false;

            var lista = await _citasService.ObtenerCitasPorProfesional(idProfesional);

            // Solo mostramos citas que no estén canceladas y que no tengan factura aún
            // Si necesitas filtrar más (p.ej. solo COMPLETADAS), ajusta el Where aquí
            var items = lista
                .Where(c => c.Estado != EstadoCita.CANCELADA)
                .Select(c => new CitaPickerItem(c))
                .ToList();

            Citas = new ObservableCollection<CitaPickerItem>(items);
            HayCitas = items.Count > 0;
            SinCitas = items.Count == 0;

            CargandoCitas = false;
        }

        // ── Picker 3: Servicios disponibles ──────────────────────────────────────

        [ObservableProperty]
        private ObservableCollection<ServicioPickerItem> _servicios = [];

        [ObservableProperty]
        private ServicioPickerItem? _servicioSeleccionado;

        [ObservableProperty]
        private bool _hayServicioSeleccionado;

        partial void OnServicioSeleccionadoChanged(ServicioPickerItem? value)
            => HayServicioSeleccionado = value is not null;

        // ── Lista de servicios añadidos + total ───────────────────────────────────

        [ObservableProperty]
        private ObservableCollection<ServicioAgregadoViewModel> _serviciosAgregados = [];

        [ObservableProperty]
        private decimal _montoTotal;

        [ObservableProperty]
        private bool _hayServiciosAgregados;

        [ObservableProperty]
        private bool _sinServiciosAgregados = true;

        [RelayCommand]
        private void AgregarServicio()
        {
            if (ServicioSeleccionado is null) return;

            ServiciosAgregados.Add(new ServicioAgregadoViewModel(ServicioSeleccionado.Servicio));
            RecalcularTotal();

            // Resetear picker tras añadir
            ServicioSeleccionado = null;
        }

        [RelayCommand]
        private void EliminarServicio(ServicioAgregadoViewModel item)
        {
            ServiciosAgregados.Remove(item);
            RecalcularTotal();
        }

        private void RecalcularTotal()
        {
            MontoTotal = ServiciosAgregados.Sum(s => (decimal)s.Precio);
            HayServiciosAgregados = ServiciosAgregados.Count > 0;
            SinServiciosAgregados = ServiciosAgregados.Count == 0;
            ActualizarPuedeCrear();
        }

        // ── Validación global ─────────────────────────────────────────────────────

        [ObservableProperty]
        private bool _puedeCrear;

        private void ActualizarPuedeCrear()
        {
            PuedeCrear = ProfesionalSeleccionado is not null
                      && CitaSeleccionada is not null
                      && ServiciosAgregados.Count > 0;
        }

        // ── Carga inicial ─────────────────────────────────────────────────────────

        private async void CargarDatosIniciales()
        {
            var profs = await _profesionalService.ObtenerVeterinarios();
            Profesionales = new ObservableCollection<ProfesionalPickerItem>(
                profs.Select(p => new ProfesionalPickerItem(p)));

            var servicios = await _serviciosService.ObtenerTodosServicios();
            Servicios = new ObservableCollection<ServicioPickerItem>(
                servicios.Select(s => new ServicioPickerItem(s)));
        }

        // ── Crear Factura ─────────────────────────────────────────────────────────

        [RelayCommand]
        private async Task CrearFactura()
        {
            if (CitaSeleccionada is null || MontoTotal <= 0) return;

            var nuevaFactura = new Factura
            {
                IdCita = CitaSeleccionada.Cita.IdCita,
                Monto = MontoTotal,
                FechaEmision = DateTime.Now,
                EstadoPago = EstadoPago.PENDIENTE
            };

            bool ok = await _facturasService.CrearFactura(nuevaFactura);

            if (ok)
            {
                await Shell.Current.DisplayAlert(
                    "Factura creada",
                    $"Se ha creado la factura por {MontoTotal:C2} correctamente.",
                    "Aceptar");

                // Volvemos a la pantalla de facturas
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "No se pudo crear la factura. Comprueba la conexión e inténtalo de nuevo.",
                    "Aceptar");
            }
        }

        [RelayCommand]
        public async void Volver() 
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
