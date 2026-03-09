using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.ViewModels
{
    [QueryProperty(nameof(MascotaId), "mascotaId")]
    [QueryProperty(nameof(ClienteId), "clienteId")]
    public partial class MascotaFormViewModel : ObservableObject
    {
        private readonly MascotaApiService _mascotaService;
        private readonly ClienteApiService _clienteService;

        [ObservableProperty]
        private int mascotaId;

        [ObservableProperty]
        private int clienteId;

        [ObservableProperty]
        private string nombreMasc = string.Empty;

        [ObservableProperty]
        private string especie = string.Empty;

        [ObservableProperty]
        private string raza = string.Empty;

        [ObservableProperty]
        private int edad;

        [ObservableProperty]
        private string tituloFormulario = "Agregar Mascota";

        [ObservableProperty]
        private string nombreCliente = string.Empty;

        [ObservableProperty]
        private bool isLoading;

        public bool EsEdicion => MascotaId > 0;

        // Lista de especies comunes
        public List<string> EspeciesDisponibles { get; } = new List<string>
        {
            "Perro",
            "Gato",
            "Ave",
            "Conejo",
            "Hámster",
            "Reptil",
            "Pez",
            "Otro"
        };

        public MascotaFormViewModel()
        {
            _mascotaService = new MascotaApiService();
            _clienteService = new ClienteApiService();
        }

        partial void OnMascotaIdChanged(int value)
        {
            if (value > 0)
            {
                TituloFormulario = "Editar Mascota";
                _ = CargarMascota();
            }
            else
            {
                TituloFormulario = "Agregar Mascota";
            }
        }

        partial void OnClienteIdChanged(int value)
        {
            if (value > 0)
            {
                _ = CargarNombreCliente();
            }
        }

        private async Task CargarNombreCliente()
        {
            try
            {
                var cliente = await _clienteService.ObtenerPorId(ClienteId);
                if (cliente != null)
                {
                    NombreCliente = $"Cliente: {cliente.NombreCli} {cliente.ApeCli}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar cliente: {ex.Message}");
            }
        }

        private async Task CargarMascota()
        {
            try
            {
                IsLoading = true;
                var mascota = await _mascotaService.ObtenerPorId(MascotaId);

                if (mascota != null)
                {
                    NombreMasc = mascota.NombreMasc;
                    Especie = mascota.Especie;
                    Raza = mascota.Raza ?? string.Empty;
                    Edad = mascota.Edad ?? 0;
                    ClienteId = mascota.IdCliente;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"Error al cargar mascota: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task Guardar()
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(NombreMasc))
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "El nombre de la mascota es obligatorio",
                    "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Especie))
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "La especie es obligatoria",
                    "OK");
                return;
            }

            if (Edad < 0 || Edad > 50)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "La edad debe estar entre 0 y 50 años",
                    "OK");
                return;
            }

            if (ClienteId <= 0)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "No se ha especificado un cliente válido",
                    "OK");
                return;
            }

            try
            {
                IsLoading = true;

                var mascota = new Mascota
                {
                    IdMascota = MascotaId,
                    IdCliente = ClienteId,
                    NombreMasc = NombreMasc.Trim(),
                    Especie = Especie.Trim(),
                    Raza = string.IsNullOrWhiteSpace(Raza) ? null : Raza.Trim(),
                    Edad = Edad > 0 ? Edad : null
                };

                bool resultado;

                if (EsEdicion)
                {
                    resultado = await _mascotaService.Actualizar(mascota);
                }
                else
                {
                    resultado = await _mascotaService.Crear(mascota);
                }

                if (resultado)
                {
                    await Shell.Current.DisplayAlert(
                        "Éxito",
                        EsEdicion ? "Mascota actualizada correctamente" : "Mascota creada correctamente",
                        "OK");

                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert(
                        "Error",
                        "No se pudo guardar la mascota",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"Error al guardar: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task Cancelar()
        {
            bool confirmar = true;

            // Si hay cambios, preguntar antes de salir
            if (!string.IsNullOrEmpty(NombreMasc) || !string.IsNullOrEmpty(Especie))
            {
                confirmar = await Shell.Current.DisplayAlert(
                    "Confirmar",
                    "¿Descartar los cambios?",
                    "Sí",
                    "No");
            }

            if (confirmar)
            {
                await Shell.Current.GoToAsync("..");
            }
        }

    }
}
