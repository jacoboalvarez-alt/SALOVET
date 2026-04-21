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
    partial class CitasViewModel : ObservableObject
    {
        private readonly CitasAPIService _service;
        private readonly ClienteApiService _clienteService;
        private readonly ProfesionalAPIService _profService;

        [ObservableProperty]
        private ObservableCollection<Cita> citas;

        [ObservableProperty]
        private bool isLoading;

        // Filtros
        [ObservableProperty] 
        private Cliente? filtroClienteSeleccionado;

        [ObservableProperty] 
        private Profesional? filtroProfesionalSeleccionado;


        [ObservableProperty]
        private DateTime filtroFecha = DateTime.Today;

        [ObservableProperty]
        private string? filtroEstado;


        //Listas 
        public ObservableCollection<Cliente> ClientesFiltro { get; set; } = new();
        public ObservableCollection<Profesional> ProfesionalesFiltro { get; set; } = new();

        public List<string> EstadosDisponibles { get; } = new()
        {
            "(Todos)",
            nameof(EstadoCita.PENDIENTE),
            nameof(EstadoCita.CONFIRMADA),
            nameof(EstadoCita.CANCELADA),
            nameof(EstadoCita.COMPLETADA)
        };

        public CitasViewModel()
        {
            _service = new CitasAPIService();
            _clienteService = new ClienteApiService();
            _profService = new ProfesionalAPIService();
            citas = new ObservableCollection<Cita>();
            filtroEstado = EstadosDisponibles[0];
        }

        public async Task InitializeAsync()
        {
            await CargarCitas();
            await CargarFiltrosAsync();
        }

        private async Task CargarFiltrosAsync()
        {
            var clientes = await _clienteService.ObtenerTodos();
            var profesionales = await _profService.ObtenerVeterinarios();

            ClientesFiltro.Clear();

            ClientesFiltro.Add(new Cliente { NombreCli = "(Todos)", ApeCli = "" });
            foreach (var c in clientes) ClientesFiltro.Add(c);

            ProfesionalesFiltro.Clear();
            ProfesionalesFiltro.Add(new Profesional { NomProf = "(Todos)", ApeProf = "" });
            foreach (var p in profesionales) ProfesionalesFiltro.Add(p);

            FiltroClienteSeleccionado = ClientesFiltro[0];
            FiltroProfesionalSeleccionado = ProfesionalesFiltro[0];
        }

        [RelayCommand]
        public async Task CargarCitas()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                var lista = await _service.ObtenerCitas();

                Citas.Clear();
                foreach (var cita in lista)
                    Citas.Add(cita);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error",
                    $"No se pudieron cargar las citas: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task Filtrar()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                List<Cita> lista;

                // Cliente seleccionado y no es "(Todos)"
                if (FiltroClienteSeleccionado?.IdCliente > 0)
                {
                    lista = await _service.ObtenerCitasPorCliente(FiltroClienteSeleccionado.IdCliente);
                }
                // Profesional seleccionado y no es "(Todos)"
                else if (FiltroProfesionalSeleccionado?.IdProf > 0)
                {
                    lista = await _service.ObtenerCitasPorProfesional(FiltroProfesionalSeleccionado.IdProf);
                }
                else if (!string.IsNullOrEmpty(FiltroEstado) && FiltroEstado != "(Todos)")
                {
                    lista = await _service.ObtenerCitasPorEstado(FiltroEstado);
                }
                else
                {
                    lista = await _service.ObtenerCitasPorFecha(FiltroFecha);
                }

                Citas.Clear();
                foreach (var cita in lista) Citas.Add(cita);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al filtrar: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task LimpiarFiltros()
        {
            FiltroClienteSeleccionado = ClientesFiltro.FirstOrDefault();
            FiltroProfesionalSeleccionado = ProfesionalesFiltro.FirstOrDefault();
            FiltroFecha = DateTime.Today;
            FiltroEstado = EstadosDisponibles[0];
            await CargarCitas();
        }

        [RelayCommand]
        public async Task EliminarCita(Cita cita)
        {
            if (cita == null) return;

            bool confirmar = await Shell.Current.DisplayAlert(
                "Confirmar",
                $"¿Eliminar la cita #{cita.IdCita}?",
                "Sí", "No");

            if (!confirmar) return;

            try
            {
                IsLoading = true;
                bool resultado = await _service.EliminarCita(cita.IdCita);

                if (resultado)
                {
                    Citas.Remove(cita);
                    await Shell.Current.DisplayAlert("Éxito",
                        "Cita eliminada correctamente", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error",
                        "No se pudo eliminar la cita", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error",
                    $"Error al eliminar: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task AgregarCita()
        {
            await Shell.Current.GoToAsync("CitasForm");
        }

        [RelayCommand]
        public async Task EditarCita(Cita cita)
        {
            if (cita == null) return;
            SessionService.CitaEdicion = cita;
            await Shell.Current.GoToAsync("EditarCita");
        }

        [RelayCommand]
        public static async Task IrADashboard()
        {
            await Shell.Current.GoToAsync("ProfDashBoard");
        }
    }
}
