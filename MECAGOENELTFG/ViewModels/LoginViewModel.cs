using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.ViewModels
{
    internal partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string username = string.Empty;
        [ObservableProperty]

        private string password = string.Empty;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string mensajeError = string.Empty;


        public LoginViewModel() { 
            _authService = new AuthService();
        }

        [RelayCommand]
        public async Task Login() {

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) {
                MensajeError = "No has introducido las credenciales completas";
                return;
            }

            try
            {
                IsLoading = true;
                MensajeError = string.Empty;

                var usuario = await _authService.LoginAsync(Username, Password);

                if (usuario == null)
                {
                    MensajeError = "El usuario o contraseña incorrectos";
                    return;

                }


                if (usuario.Profesional)
                {
                    await Shell.Current.GoToAsync("ProfDashBoard");

                    username = string.Empty;
                    password = string.Empty;
                }
                else
                {
                    SessionService.IdClienteActual = (int)usuario.IdCliente;
                    await Shell.Current.GoToAsync("ClientDashBoard");
                    username = string.Empty;
                    password = string.Empty;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"Error al iniciar sesion: {ex.Message}",
                    "OK");
            }

            finally { IsLoading = false; }
        }

        [RelayCommand]
        public static async Task IrARegistro() {
            await Shell.Current.GoToAsync("Registro");
        }

    }
}
