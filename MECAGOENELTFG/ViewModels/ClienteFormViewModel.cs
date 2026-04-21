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
    public partial class ClienteFormViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ClienteApiService _service;

        [ObservableProperty]
        private int clienteId;

        [ObservableProperty]
        private string nombreCli = string.Empty;

        [ObservableProperty]
        private string apeCli = string.Empty;

        [ObservableProperty]
        private int edad;

        [ObservableProperty]
        private string correo = string.Empty;

        [ObservableProperty]
        private string tel = string.Empty;

        [ObservableProperty]
        private string tituloFormulario = "Agregar Cliente";

        [ObservableProperty]
        private bool isLoading;

        public bool EsEdicion => ClienteId > 0;

        public ClienteFormViewModel()
        {
            _service = new ClienteApiService();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query) 
        {
            if (query.TryGetValue("clienteId", out var value) &&
                 int.TryParse(value?.ToString(), out int id) && id > 0)
            {
                ClienteId = id;
            }
        }

        partial void OnClienteIdChanged(int value)
        {
            if (value > 0)
            {
                TituloFormulario = "Editar Cliente";
                _ = CargarCliente();
            }
            else
            {
                TituloFormulario = "Agregar Cliente";
            }
        }

        private async Task CargarCliente()
        {
            try
            {
                IsLoading = true;
                var cliente = await _service.ObtenerPorId(ClienteId);

                if (cliente != null)
                {
                    NombreCli = cliente.NombreCli;
                    ApeCli = cliente.ApeCli;
                    Edad = cliente.Edad;
                    Correo = cliente.Correo;
                    Tel = cliente.Tel ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"Error al cargar cliente: {ex.Message}",
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
            if (string.IsNullOrWhiteSpace(NombreCli))
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "El nombre es obligatorio",
                    "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(ApeCli))
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "El apellido es obligatorio",
                    "OK");
                return;
            }

            if (Edad <= 0 || Edad > 120)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "La edad debe estar entre 1 y 120",
                    "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Correo) || !Correo.Contains("@"))
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "El correo no es válido",
                    "OK");
                return;
            }

            try
            {
                IsLoading = true;

                var cliente = new Cliente
                {
                    IdCliente = ClienteId,
                    NombreCli = NombreCli.Trim(),
                    ApeCli = ApeCli.Trim(),
                    Edad = Edad,
                    Correo = Correo.Trim(),
                    Tel = Tel?.Trim()
                };

                bool resultado;

                if (EsEdicion)
                {
                    resultado = await _service.Actualizar(cliente);
                }
                else
                {
                    resultado = await _service.Crear(cliente);
                }

                if (resultado)
                {
                    await Shell.Current.DisplayAlert(
                        "Éxito",
                        EsEdicion ? "Cliente actualizado correctamente" : "Cliente creado correctamente",
                        "OK");

                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert(
                        "Error",
                        "No se pudo guardar el cliente",
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
            if (!string.IsNullOrEmpty(NombreCli) || !string.IsNullOrEmpty(ApeCli))
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

        [RelayCommand]
        public static async Task IrADashboard() 
        {
            await Shell.Current.GoToAsync("ClientesPage");
        }
    }
}
