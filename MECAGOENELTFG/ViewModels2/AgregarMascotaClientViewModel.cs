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
    public partial class AgregarMascotaClientViewModel : ObservableObject
    {

        private readonly MascotaApiService _mascotaService = new();

        // ─── Campos del formulario ────────────────────────────────────────

        [ObservableProperty] private string _nombreMascota = string.Empty;
        [ObservableProperty] private string _edad = string.Empty;
        [ObservableProperty] private string _raza = string.Empty;
        [ObservableProperty] private string _especie = string.Empty;
        [ObservableProperty] private string _color = string.Empty;
        [ObservableProperty] private string _sexo = string.Empty;
        [ObservableProperty] private string _tipoPelo = string.Empty;
        [ObservableProperty] private string _notas = string.Empty;
        [ObservableProperty] private bool _estaVacunado = false;
        [ObservableProperty] private string _vacunadoLabel = "No";

        // ─── Estado de la UI ──────────────────────────────────────────────

        [ObservableProperty] private bool _isFormVisible = false;
        [ObservableProperty] private bool _isLoading = false;
        [ObservableProperty] private bool _sizeFieldVisible = false;
        [ObservableProperty] private bool _hairFieldVisible = false;
        [ObservableProperty] private bool _speciesFieldVisible = false;
        [ObservableProperty] private string _selectedType = string.Empty;
        [ObservableProperty] private string _breedLabel = "Raza *";
        [ObservableProperty] private string _breedPlaceholder = "Ej: Labrador";
        [ObservableProperty] private string _saveButtonText = "💾 Guardar";
        [ObservableProperty] private string _tamanoSeleccionado = string.Empty;

        // ─── Comandos de tipo ─────────────────────────────────────────────

        [RelayCommand]
        private void SelectDog() => SelectType("dog");

        [RelayCommand]
        private void SelectCat() => SelectType("cat");

        [RelayCommand]
        private void SelectOther() => SelectType("other");

        private void SelectType(string type)
        {
            if (SelectedType == type) return;
            SelectedType = type;
            IsFormVisible = true;

            SaveButtonText = type switch
            {
                "dog" => "💾 Guardar perro",
                "cat" => "💾 Guardar gato",
                _ => "💾 Guardar mascota"
            };

            SizeFieldVisible = type == "dog";
            HairFieldVisible = type == "cat";
            SpeciesFieldVisible = type == "other";

            BreedLabel = type == "other" ? "Raza / tipo" : "Raza *";

            BreedPlaceholder = type switch
            {
                "dog" => "Ej: Labrador, Bulldog...",
                "cat" => "Ej: Persa, Siamés...",
                _ => "Si lo sabes (opcional)"
            };
        }

        // ─── Comando tamaño ───────────────────────────────────────────────

        [RelayCommand]
        private void SelectTamano(string tamano)
        {
            TamanoSeleccionado = tamano;
        }

        // ─── Toggle vacunado ──────────────────────────────────────────────

        partial void OnEstaVacunadoChanged(bool value)
        {
            VacunadoLabel = value ? "Sí" : "No";
        }

        // ─── Guardar ──────────────────────────────────────────────────────

        [RelayCommand]
        private async Task Guardar()
        {
            if (string.IsNullOrWhiteSpace(SelectedType))
            {
                await Shell.Current.DisplayAlert("Falta el tipo", "Por favor selecciona el tipo de mascota.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(NombreMascota))
            {
                await Shell.Current.DisplayAlert("Falta el nombre", "Por favor escribe el nombre de tu mascota.", "OK");
                return;
            }

            IsLoading = true;

            string especieResuelta = SelectedType switch
            {
                "dog" => "Perro",
                "cat" => "Gato",
                _ => string.IsNullOrWhiteSpace(Especie) ? "Otro" : Especie
            };

            var nueva = new Mascota
            {
                IdCliente = SessionService.IdClienteActual,
                NombreMasc = NombreMascota,
                Especie = especieResuelta,
                Raza = string.IsNullOrWhiteSpace(Raza) ? null : Raza,
                Edad = int.TryParse(Edad, out int edadP) ? edadP : null,
                Sexo = string.IsNullOrWhiteSpace(Sexo) ? null : Sexo,
                Color = string.IsNullOrWhiteSpace(Color) ? null : Color,
                Tamano = string.IsNullOrWhiteSpace(TamanoSeleccionado) ? null : TamanoSeleccionado,
                TipoPelo = string.IsNullOrWhiteSpace(TipoPelo) ? null : TipoPelo,
                Vacunado = EstaVacunado,
                Notas = string.IsNullOrWhiteSpace(Notas) ? null : Notas
            };

            bool ok = await _mascotaService.Crear(nueva);
            IsLoading = false;

            if (ok)
            {
                await Shell.Current.DisplayAlert("¡Listo!", $"{NombreMascota} ha sido añadido correctamente.", "OK");
                await Shell.Current.GoToAsync("PerfilPage");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo guardar la mascota. Inténtalo de nuevo.", "OK");
            }
        }

        // ─── Volver ───────────────────────────────────────────────────────

        [RelayCommand]
        private async Task Volver()
        {
            await Shell.Current.GoToAsync("PerfilPage");
        }
    }
}
