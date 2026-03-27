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

        [ObservableProperty]
        private ObservableCollection<Cita> citas;

        [ObservableProperty]
        private bool isLoading;

        // Filtros
        [ObservableProperty]
        private string filtroIdCliente = string.Empty;

        [ObservableProperty]
        private string filtroIdProfesional = string.Empty;

        [ObservableProperty]
        private DateTime filtroFecha = DateTime.Today;

        [ObservableProperty]
        private string? filtroEstado;

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
            citas = new ObservableCollection<Cita>();
            filtroEstado = EstadosDisponibles[0];
        }

        public async Task InitializeAsync()
        {
            await CargarCitas();
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

                // Prioridad: ID Cliente → ID Profesional → Estado → Fecha
                if (int.TryParse(FiltroIdCliente, out int idCliente) && idCliente > 0)
                {
                    lista = await _service.ObtenerCitasPorCliente(idCliente);
                }
                else if (int.TryParse(FiltroIdProfesional, out int idProf) && idProf > 0)
                {
                    lista = await _service.ObtenerCitasPorProfesional(idProf);
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
                foreach (var cita in lista)
                    Citas.Add(cita);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error",
                    $"Error al filtrar: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task LimpiarFiltros()
        {
            FiltroIdCliente = string.Empty;
            FiltroIdProfesional = string.Empty;
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
            await Shell.Current.GoToAsync("citaform");
        }

        [RelayCommand]
        public async Task EditarCita(Cita cita)
        {
            if (cita == null) return;
            await Shell.Current.GoToAsync($"citaform?citaId={cita.IdCita}");
        }

        [RelayCommand]
        public static async Task IrADashboard()
        {
            await Shell.Current.GoToAsync("ProfDashBoard");
        }
    }
}
