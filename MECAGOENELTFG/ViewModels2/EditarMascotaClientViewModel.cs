using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.ViewModels2
{
    [QueryProperty(nameof(MascotaActual), "MascotaActual")]
    public partial class EditarMascotaClientViewModel : ObservableObject
    {
        private readonly MascotaApiService _service;

        [ObservableProperty] private Mascota mascotaActual;
        [ObservableProperty] private string nombreMasc = "";
        [ObservableProperty] private string edad = "";
        [ObservableProperty] private string sexo = "";
        [ObservableProperty] private string color = "";
        [ObservableProperty] private string tamano = "";
        [ObservableProperty] private string tipoPelo = "";
        [ObservableProperty] private bool vacunado = false;
        [ObservableProperty] private string notas = "";

        [ObservableProperty] private bool sizeFieldVisible = false;
        [ObservableProperty] private bool hairFieldVisible = false;
        [ObservableProperty] private bool speciesFieldVisible = false;


        public EditarMascotaClientViewModel() 
        {
            _service = new MascotaApiService();
        }

        partial void OnMascotaActualChanged(Mascota value)
        {
            if (value == null) return;
            NombreMasc = value.NombreMasc;
            Edad = value.Edad.ToString();
            Sexo = value.Sexo;
            Color = value.Color;
            Tamano = value.Tamano;
            TipoPelo = value.TipoPelo; 
            Vacunado = value.Vacunado;
            Notas = value.Notas;

            string especie = value.Especie.ToLower();
            SizeFieldVisible = especie == "perro";
            HairFieldVisible = especie == "gato";
            SpeciesFieldVisible = especie != "perro" && especie != "gato";
        }

        [RelayCommand]
        private async Task Guardar() 
        { 
            if(MascotaActual == null) return;

            MascotaActual.NombreMasc = NombreMasc;
            MascotaActual.Edad = int.TryParse(Edad, out int e) ? e: null;
            MascotaActual.Sexo = string.IsNullOrWhiteSpace(Sexo) ? null : Sexo;
            MascotaActual.Color = string.IsNullOrWhiteSpace (Color) ? null : Color;
            MascotaActual.Tamano = string.IsNullOrWhiteSpace(Tamano) ? null: Tamano;
            MascotaActual.TipoPelo = string.IsNullOrWhiteSpace(TipoPelo) ? null : TipoPelo;
            MascotaActual.Vacunado = Vacunado;
            MascotaActual.Notas = string.IsNullOrWhiteSpace(Notas) ? null : Notas;

            bool ok = await _service.Actualizar(MascotaActual);
            if (ok)
            {
                await Shell.Current.DisplayAlert("!Listo¡", $"La mascota:{MascotaActual.NombreMasc} ha sido actualizado/a correctamente", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else 
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo actualizar la mascota revista todos los campos", "OK");
            }
        }

        [RelayCommand]
        private async Task Eliminar() 
        {
            if (MascotaActual == null) return;
            bool confirmar = await Shell.Current.DisplayAlert(
                "¿Elimnar Mascota?",
                $"¿Estas seguro que quieres eliminar a {MascotaActual.NombreMasc}?",
                "Si, Eliminar", "Cancelar"
                );
            if (!confirmar) return;

            bool confirmar2 = await Shell.Current.DisplayAlert(
                "Confirmar Eliminacion",
                $"Esta accion eliminara todos los datos de {MascotaActual.NombreMasc} esta accion no es reversible y sera permanente",
                "Entendido, quiero eliminarlo", "Cancelar"
                );

            bool ok = await _service.Eliminar(MascotaActual.IdMascota);
            if (ok) 
            {
                await Shell.Current.DisplayAlert("Listo", "Mascota eliminada correctamente.","OK");
                await Shell.Current.GoToAsync("..");
            }
            else 
            {
                await Shell.Current.DisplayAlert("Error", "No se ha podido eliminar la Mascota", "OK");
            }
        }

        [RelayCommand]
        private async Task Cancelar() => await Shell.Current.GoToAsync("..");
    }
}
