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
        private readonly UsuarioService _UsuarioService;

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

        [ObservableProperty]
        private string pass = string.Empty;

        [ObservableProperty]
        private string usuario = string.Empty;

        public bool EsEdicion => ClienteId > 0;

        public ClienteFormViewModel()
        {
            _service = new ClienteApiService();
            _UsuarioService = new UsuarioService();
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
            // --- Validaciones existentes ---
            if (string.IsNullOrWhiteSpace(NombreCli))
            {
                await Shell.Current.DisplayAlert("Error", "El nombre es obligatorio", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(ApeCli))
            {
                await Shell.Current.DisplayAlert("Error", "El apellido es obligatorio", "OK");
                return;
            }
            if (Edad <= 0 || Edad > 120)
            {
                await Shell.Current.DisplayAlert("Error", "La edad debe estar entre 1 y 120", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(Correo) || !Correo.Contains("@"))
            {
                await Shell.Current.DisplayAlert("Error", "El correo no es válido", "OK");
                return;
            }

            // --- Validaciones de usuario (solo al crear) ---
            if (!EsEdicion)
            {
                if (string.IsNullOrWhiteSpace(Usuario))
                {
                    await Shell.Current.DisplayAlert("Error", "El nombre de usuario es obligatorio", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(Pass) || Pass.Length < 6)
                {
                    await Shell.Current.DisplayAlert("Error", "La contraseña debe tener al menos 6 caracteres", "OK");
                    return;
                }
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
                    // Edición: solo actualiza el cliente (el usuario se gestiona aparte)
                    resultado = await _service.Actualizar(cliente);
                }
                else
                {
                    // Creación: primero el cliente, luego el usuario asociado
                    var clienteCreado = await _service.Crear(cliente);

                    if (clienteCreado == null)
                    {
                        await Shell.Current.DisplayAlert("Error", "No se pudo crear el cliente", "OK");
                        return;
                    }

                    Pass = HashHelper.HashText(Pass);

                    var usuario = new Usuario
                    {
                        Username = Usuario.Trim(),
                        Pass = Pass.Trim(),
                        Profesional = false,
                        IdCliente = clienteCreado.IdCliente,
                        Primero = true
                    };

                    resultado = await _UsuarioService.Crear(usuario);

                    if (!resultado)
                    {
                        // El cliente ya fue creado; avisa pero no bloquees
                        await Shell.Current.DisplayAlert(
                            "Advertencia",
                            "El cliente se creó pero hubo un error al crear el usuario. Contacte al administrador.",
                            "OK");
                        await Shell.Current.GoToAsync("..");
                        return;
                    }
                }

                if (resultado)
                {
                    await Shell.Current.DisplayAlert(
                        "Éxito",
                        EsEdicion ? "Cliente actualizado correctamente" : "Cliente y usuario creados correctamente",
                        "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo guardar el cliente", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al guardar: {ex.Message}", "OK");
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
            await Shell.Current.GoToAsync("..");
        }
    }
}
