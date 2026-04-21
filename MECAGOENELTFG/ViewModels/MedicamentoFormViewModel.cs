using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MECAGOENELTFG.ViewModels
{
    public partial class MedicamentoFormViewModel : ObservableObject
    {
        private readonly MedicamentosAPIService _service;

        [ObservableProperty] private int idMedica;
        [ObservableProperty] private string nomMedica = string.Empty;
        [ObservableProperty] private float gramos;
        [ObservableProperty] private float precio;
        [ObservableProperty] private int stock;
        [ObservableProperty] private bool estado;

        public bool EsEdicion => IdMedica > 0;
        public string Titulo => EsEdicion ? "Editar Medicamento" : "Nuevo Medicamento";

        public ICommand VolverCommand { get; }

        public MedicamentoFormViewModel(Medicamento? medicamento = null) 
        {
            _service = new MedicamentosAPIService();
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            if(medicamento != null) 
            {
                IdMedica = medicamento.IdMedica;
                NomMedica = medicamento.NomMedica;
                Gramos = medicamento.Gramos;
                Precio = medicamento.Precio;
                Stock = medicamento.Stock;
                Estado = medicamento.Estado;
            }
        }

        [RelayCommand]
        private async Task Guardar() 
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(NomMedica))
            {
                await Shell.Current.DisplayAlert("Error", "El nombre es obligatorio.", "OK");
                return;
            }
            if (Gramos <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Los gramos deben ser mayor que 0.", "OK");
                return;
            }
            if (Precio <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "El precio debe ser mayor que 0.", "OK");
                return;
            }
            if (Stock < 0)
            {
                await Shell.Current.DisplayAlert("Error", "El stock no puede ser negativo.", "OK");
                return;
            }

            var medicamento = new Medicamento
            {
                IdMedica = IdMedica,
                NomMedica = NomMedica,
                Gramos = Gramos,
                Precio = Precio,
                Stock = Stock,
                Estado = Estado

            };

            bool ok;

            if (EsEdicion) 
            {
                ok = await _service.ActualizarMedicamento(IdMedica, medicamento);
            }
            else 
            {
                var resultado = await _service.CrearMedicamento(medicamento);
                ok = resultado != null;
            }

            if (ok) 
            {
                await Shell.Current.DisplayAlert("Exito", $"Se ha guardado el medicamento {nomMedica} correctamente", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else 
            {
                await Shell.Current.DisplayAlert("Error", "No se ha podido agregar correctamente el medicamento.", "OK");
            }
        }
    }
}
