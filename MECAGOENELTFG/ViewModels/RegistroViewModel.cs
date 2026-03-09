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
    internal partial class RegistroViewModel : ObservableObject
    {

        private readonly RegistroService _service;

        //DATOS DEL CLIENTE

        [ObservableProperty]
        public string nombre = string.Empty;

        [ObservableProperty]
        public string apellidos = string.Empty;

        [ObservableProperty]
        public string correo = string.Empty;

        [ObservableProperty]
        public string? telefono = string.Empty;

        [ObservableProperty]
        public int edad = 0;

        
        //DATOS DEL USUARIO

        [ObservableProperty]
        public string usuario = string.Empty;
        
        [ObservableProperty]
        public string pass = string.Empty;

        [ObservableProperty]
        public string confirmarPass = string.Empty;

        // ESTADO

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string mensajeError = string.Empty;

        public RegistroViewModel() { 
            _service = new RegistroService();
        }

        [RelayCommand]
        public async Task Registrar()
        {
            if (string.IsNullOrWhiteSpace(Nombre) ||
                string.IsNullOrWhiteSpace(Apellidos) ||
                string.IsNullOrWhiteSpace(Correo) ||
                string.IsNullOrWhiteSpace(Usuario) ||
                string.IsNullOrWhiteSpace(Pass) ||
                string.IsNullOrWhiteSpace(ConfirmarPass))
            {
                MensajeError = "Por favor, rellena todos los campos obligatorios.";
                return;
            }

            if (Pass != ConfirmarPass) {
                MensajeError = "Las contraseñas no coinciden";
                return;
            }

            try
            {
                IsLoading = true;
                MensajeError = string.Empty;

                // Construir objetos Cliente y Usuario
                var cliente = new Cliente(
                    Nombre,
                    Apellidos,
                    Edad,
                    Correo,
                    Telefono ?? string.Empty
                );

                var usuarionew = new Usuario(
                    Usuario,
                    Pass,
                    false,
                    0,      // IdCliente se asignará en el servicio tras crear el cliente
                    cliente
                );

                //LLAMAMOS AL SERVICIO

                string? error = await _service.RegistroAsync(usuarionew, cliente);

                if (error != null)
                {
                    MensajeError = error;
                    return;
                }

                await Shell.Current.DisplayAlert(
                    $"Bienvenido:{Nombre} ",
                    "Cuenta creada correctamente. Ya tienes acceso.",
                    "OK"
                    );
                await Shell.Current.GoToAsync("///Login");
            }
            catch (Exception ex) {
                await Shell.Current.DisplayAlert(
                    "Error Inesperado:",
                    $"Error: {ex.Message}",
                    "OK"
                    );        
             }
            finally
            { IsLoading = false; }
        }

        [RelayCommand]
        public static async Task IrALogin() {
            await Shell.Current.GoToAsync("///Login");
        }
    }
}
