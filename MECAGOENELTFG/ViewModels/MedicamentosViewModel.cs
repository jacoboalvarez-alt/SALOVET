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
    partial class MedicamentosViewModel : ObservableObject
    {
        private readonly MedicamentosAPIService _service;

        [ObservableProperty]
        private ObservableCollection<Medicamento> medicamentos;

        [ObservableProperty]
        private bool isLoading;

        // Filtros
        [ObservableProperty]
        private string filtroNombre = string.Empty;

        [ObservableProperty]
        private string filtroBajoStock = string.Empty;

        [ObservableProperty]
        private string? filtroEstado;

        public List<string> EstadosDisponibles { get; } = new()
        {
                "(Todos)",
                "Activos",     
                "Inactivos",   
                "Sin stock"
        };

        public MedicamentosViewModel()
        {
            _service = new MedicamentosAPIService();
            medicamentos = new ObservableCollection<Medicamento>();
            filtroEstado = EstadosDisponibles[0];
        }

        [RelayCommand]
        public async Task CargarMedicamentos()
        {
            if (IsLoading) return;
            try
            {
                IsLoading = true;
                var lista = await _service.ObtenerMedicamentos();
                Medicamentos.Clear();
                foreach (var m in lista) Medicamentos.Add(m);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error",
                    $"No se pudieron cargar los medicamentos: {ex.Message}", "OK");
            }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        public async Task Filtrar()
        {
            if (IsLoading) return;
            try
            {
                IsLoading = true;

                // Carga base — bajo stock tiene prioridad si se especifica
                List<Medicamento> lista;
                if (int.TryParse(FiltroBajoStock, out int limite) && limite > 0)
                    lista = await _service.ObtenerBajoStock(limite);
                else
                    lista = await _service.ObtenerMedicamentos();

                // Filtro por nombre
                if (!string.IsNullOrWhiteSpace(FiltroNombre))
                    lista = lista.Where(m =>
                        m.NomMedica.Contains(FiltroNombre, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                // Filtro por estado/disponibilidad
                if (FiltroEstado == "Activos")
                    lista = lista.Where(m => m.Estado).ToList();
                else if (FiltroEstado == "Inactivos")
                    lista = lista.Where(m => !m.Estado).ToList();
                else if (FiltroEstado == "Sin stock")
                    lista = lista.Where(m => m.Stock == 0).ToList();

                Medicamentos.Clear();
                foreach (var m in lista) Medicamentos.Add(m);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al filtrar: {ex.Message}", "OK");
            }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        public async Task LimpiarFiltros()
        {
            FiltroNombre = string.Empty;
            FiltroBajoStock = string.Empty;
            FiltroEstado = EstadosDisponibles[0];
            await CargarMedicamentos();
        }

        [RelayCommand]
        public async Task AumentarStock(Medicamento medicamento)
        {
            if (medicamento == null) return;
            try
            {
                int nuevoStock = medicamento.Stock + 1;
                bool ok = await _service.ActualizarStock(medicamento.IdMedica, nuevoStock);
                if (ok) medicamento.Stock = nuevoStock;

                // Forzar refresco visual del item
                var idx = Medicamentos.IndexOf(medicamento);
                if (idx >= 0)
                {
                    Medicamentos.RemoveAt(idx);
                    Medicamentos.Insert(idx, medicamento);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al actualizar stock: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task DisminuirStock(Medicamento medicamento)
        {
            if (medicamento == null) return;
            if (medicamento.Stock <= 0)
            {
                await Shell.Current.DisplayAlert("Aviso", "El stock ya está en 0.", "OK");
                return;
            }
            try
            {
                int nuevoStock = medicamento.Stock - 1;
                bool ok = await _service.ActualizarStock(medicamento.IdMedica, nuevoStock);
                if (ok) medicamento.Stock = nuevoStock;

                var idx = Medicamentos.IndexOf(medicamento);
                if (idx >= 0)
                {
                    Medicamentos.RemoveAt(idx);
                    Medicamentos.Insert(idx, medicamento);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al actualizar stock: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task EliminarMedicamento(Medicamento medicamento)
        {
            if (medicamento == null) return;
            bool confirmar = await Shell.Current.DisplayAlert("Confirmar",
                $"¿Eliminar {medicamento.NomMedica}?", "Sí", "No");
            if (!confirmar) return;

            try
            {
                IsLoading = true;
                bool ok = await _service.EliminarMedicamento(medicamento.IdMedica);
                if (ok)
                {
                    Medicamentos.Remove(medicamento);
                    await Shell.Current.DisplayAlert("Éxito", "Medicamento eliminado correctamente.", "OK");
                }
                else
                    await Shell.Current.DisplayAlert("Error", "No se pudo eliminar el medicamento.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al eliminar: {ex.Message}", "OK");
            }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        public async Task AgregarMedicamento() =>
            await Shell.Current.GoToAsync("MedicamentoForm");

        [RelayCommand]
        public async Task EditarMedicamento(Medicamento medicamento)
        {
            if (medicamento == null) return;
            SessionService.MedicamentoEdicion = medicamento;
            await Shell.Current.GoToAsync("MedicamentoForm");
        }

        [RelayCommand]
        public async Task IrADashboard()
        {
            await Shell.Current.GoToAsync("ProfDashBoard");
        }
    }
}
