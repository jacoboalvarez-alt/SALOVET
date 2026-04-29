using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;
using System.Collections.ObjectModel;

namespace MECAGOENELTFG.ViewModels
{
    // ─── Wrappers Picker ──────────────────────────────────────────────────────────

    public class MedicamentoAgregadoViewModel
    {
        public Medicamento Medicamento { get; }
        public string NomMedica { get; }
        public float Precio { get; }
        public string StockTexto { get; }

        public MedicamentoAgregadoViewModel(Medicamento medicamento)
        {
            Medicamento = medicamento;
            NomMedica = medicamento.NomMedica;
            Precio = medicamento.Precio;
            StockTexto = $"Stock: {medicamento.Stock}";
        }
    }

    public class MedicamentoPickerItem
    {
        public Medicamento Medicamento { get; }
        public string ResumenMedicamento { get; }

        public MedicamentoPickerItem(Medicamento medicamento)
        {
            Medicamento = medicamento;
            ResumenMedicamento = medicamento.Stock > 0
                ? $"{medicamento.NomMedica} ({medicamento.Gramos}g)  —  {medicamento.Precio:C2}  · Stock: {medicamento.Stock}"
                : $"{medicamento.NomMedica}  —  SIN STOCK";
        }
    }

    public class ProfesionalPickerItem
    {
        public Profesional Profesional { get; }
        public string NombreCompleto => $"{Profesional.NomProf} {Profesional.ApeProf}".Trim();

        public ProfesionalPickerItem(Profesional profesional) => Profesional = profesional;
    }

    public class CitaPickerItem
    {
        public Cita Cita { get; }
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
        private readonly MedicamentosAPIService _medicamentosService;

        public NuevaFacturaViewModel(
            ProfesionalAPIService profesionalService,
            CitasAPIService citasService,
            ServiciosService serviciosService,
            FacturasService facturasService,
            MedicamentosAPIService medicamentosService)
        {
            _profesionalService = profesionalService;
            _citasService = citasService;
            _serviciosService = serviciosService;
            _facturasService = facturasService;
            _medicamentosService = medicamentosService;

            CargarDatosIniciales();
        }

        // ── Picker 1: Profesionales ───────────────────────────────────────────────

        [ObservableProperty]
        private ObservableCollection<ProfesionalPickerItem> _profesionales = [];

        [ObservableProperty]
        private ProfesionalPickerItem? _profesionalSeleccionado;

        partial void OnProfesionalSeleccionadoChanged(ProfesionalPickerItem? value)
        {
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

            var items = lista
                .Where(c => c.Estado != EstadoCita.CANCELADA)
                .Select(c => new CitaPickerItem(c))
                .ToList();

            Citas = new ObservableCollection<CitaPickerItem>(items);
            HayCitas = items.Count > 0;
            SinCitas = items.Count == 0;

            CargandoCitas = false;
        }

        // ── Picker 3: Servicios ───────────────────────────────────────────────────

        [ObservableProperty]
        private ObservableCollection<ServicioPickerItem> _servicios = [];

        [ObservableProperty]
        private ServicioPickerItem? _servicioSeleccionado;

        [ObservableProperty]
        private bool _hayServicioSeleccionado;

        partial void OnServicioSeleccionadoChanged(ServicioPickerItem? value)
            => HayServicioSeleccionado = value is not null;

        // ── Lista de servicios añadidos ───────────────────────────────────────────

        [ObservableProperty]
        private ObservableCollection<ServicioAgregadoViewModel> _serviciosAgregados = [];

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
            ServicioSeleccionado = null;
        }

        [RelayCommand]
        private void EliminarServicio(ServicioAgregadoViewModel item)
        {
            ServiciosAgregados.Remove(item);
            RecalcularTotal();
        }

        // ── Picker 4: Medicamentos ────────────────────────────────────────────────

        [ObservableProperty]
        private ObservableCollection<MedicamentoPickerItem> _medicamentos = [];

        [ObservableProperty]
        private MedicamentoPickerItem? _medicamentoSeleccionado;

        [ObservableProperty]
        private bool _hayMedicamentoSeleccionado;

        partial void OnMedicamentoSeleccionadoChanged(MedicamentoPickerItem? value)
            => HayMedicamentoSeleccionado = value is not null && value.Medicamento.Stock > 0;

        // ── Lista de medicamentos añadidos ────────────────────────────────────────

        [ObservableProperty]
        private ObservableCollection<MedicamentoAgregadoViewModel> _medicamentosAgregados = [];

        [ObservableProperty]
        private bool _hayMedicamentosAgregados;

        [ObservableProperty]
        private bool _sinMedicamentosAgregados = true;

        [RelayCommand]
        private void AgregarMedicamento()
        {
            if (MedicamentoSeleccionado is null) return;

            if (MedicamentosAgregados.Any(m => m.Medicamento.IdMedica == MedicamentoSeleccionado.Medicamento.IdMedica))
            {
                Shell.Current.DisplayAlert("Aviso", "Este medicamento ya está añadido.", "Aceptar");
                return;
            }

            MedicamentosAgregados.Add(new MedicamentoAgregadoViewModel(MedicamentoSeleccionado.Medicamento));
            RecalcularTotal();
            HayMedicamentosAgregados = true;
            SinMedicamentosAgregados = false;
            MedicamentoSeleccionado = null;
        }

        [RelayCommand]
        private void EliminarMedicamento(MedicamentoAgregadoViewModel item)
        {
            MedicamentosAgregados.Remove(item);
            RecalcularTotal();
            HayMedicamentosAgregados = MedicamentosAgregados.Count > 0;
            SinMedicamentosAgregados = MedicamentosAgregados.Count == 0;
        }

        // ── Total ─────────────────────────────────────────────────────────────────

        [ObservableProperty]
        private decimal _montoTotal;

        private void RecalcularTotal()
        {
            var totalServicios = ServiciosAgregados.Sum(s => (decimal)s.Precio);
            var totalMedicamentos = MedicamentosAgregados.Sum(m => (decimal)m.Precio);

            MontoTotal = totalServicios + totalMedicamentos;
            HayServiciosAgregados = ServiciosAgregados.Count > 0;
            SinServiciosAgregados = ServiciosAgregados.Count == 0;
            ActualizarPuedeCrear();
        }

        // ── Validación ────────────────────────────────────────────────────────────

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

            var medicamentos = await _medicamentosService.ObtenerDisponibles();
            Medicamentos = new ObservableCollection<MedicamentoPickerItem>(
                medicamentos.Select(m => new MedicamentoPickerItem(m)));
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
                EstadoPago = EstadoPago.PENDIENTE,
                Cita = null!
            };

            bool ok = await _facturasService.CrearFactura(nuevaFactura);

            if (ok)
            {
                foreach (var med in MedicamentosAgregados)
                {
                    int nuevoStock = med.Medicamento.Stock - 1;
                    bool stockOk = await _medicamentosService.ActualizarStock(
                                          med.Medicamento.IdMedica, nuevoStock);

                    if (!stockOk)
                        Console.WriteLine($"⚠️ No se pudo actualizar stock de {med.NomMedica}");
                }

                await Shell.Current.DisplayAlert(
                    "Factura creada",
                    $"Factura creada correctamente por {MontoTotal:C2}.",
                    "Aceptar");

                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "No se pudo crear la factura. Inténtalo de nuevo.",
                    "Aceptar");
            }
        }

        // ── Volver ────────────────────────────────────────────────────────────────

        [RelayCommand]
        public async void Volver()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}