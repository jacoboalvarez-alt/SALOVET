using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;
using System.Collections.ObjectModel;

namespace MECAGOENELTFG.ViewModels
{
    internal partial class ProfDashBoardModelView : ObservableObject
    {
        private readonly CitasAPIService _citasService = new();

        [ObservableProperty]
        private ObservableCollection<CitaNotificacion> _notificaciones = new();

        [ObservableProperty]
        private bool _hayNotificaciones;

        [ObservableProperty]
        private bool _cargando;

        public ProfDashBoardModelView()
        {
            _ = CargarNotificacionesAsync();
        }
       

        private async Task CargarNotificacionesAsync()
        {
            Cargando = true;

            try
            {
                var idProf = SessionService.IdProfesional;
                var citas = await _citasService.ObtenerCitasPorProfesional(idProf);

                var hoy = DateTime.Today;
                var lista = new ObservableCollection<CitaNotificacion>();

                // Citas de HOY activas
                var citasHoy = citas
                    .Where(c => c.FechaHora.Date == hoy
                             && c.Estado != EstadoCita.CANCELADA
                             && c.Estado != EstadoCita.COMPLETADA)
                    .OrderBy(c => c.FechaHora);

                foreach (var c in citasHoy)
                    lista.Add(new CitaNotificacion(c, esHoy: true));

                // Citas PRÓXIMAS (hoy+1 hasta hoy+14)
                var citasProximas = citas
                    .Where(c => c.FechaHora.Date > hoy
                             && c.FechaHora.Date <= hoy.AddDays(14)
                             && (c.Estado == EstadoCita.PENDIENTE
                              || c.Estado == EstadoCita.CONFIRMADA))
                    .OrderBy(c => c.FechaHora);

                foreach (var c in citasProximas)
                    lista.Add(new CitaNotificacion(c, esHoy: false));

                Notificaciones = lista;
                HayNotificaciones = lista.Any();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando notificaciones: {ex.Message}");
            }
            finally
            {
                Cargando = false;
            }
        }

        [RelayCommand]
        public async Task RefrescarNotificaciones()
            => await CargarNotificacionesAsync();

        // ── Navegación ──────────────────────────────────────────────

        [RelayCommand]
        public async Task IrAClientes()
            => await Shell.Current.GoToAsync("ClientesPage");

        [RelayCommand]
        public async Task IrAMascotas()
            => await Shell.Current.GoToAsync("MascotasPageGeneral");

        [RelayCommand]
        public async Task IrACitas()
            => await Shell.Current.GoToAsync("CitasPage");

        [RelayCommand]
        public async Task IrAMedicamentos()
            => await Shell.Current.GoToAsync("MedicamentosPage");

        [RelayCommand]
        public async Task IrALogin()
            => await Shell.Current.GoToAsync("///Login");

        [RelayCommand]
        public async Task IrAAsistente()
            => await Shell.Current.GoToAsync("AsistenteVetProf");

        [RelayCommand]
        public async Task IrAFacturas()
            => await Shell.Current.GoToAsync("FacturasPage");
    }
}