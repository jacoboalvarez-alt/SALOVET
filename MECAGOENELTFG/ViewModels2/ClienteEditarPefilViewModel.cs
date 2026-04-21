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
    [QueryProperty(nameof(ClienteActual), "ClienteActual")]
    public partial class ClienteEditarPefilViewModel : ObservableObject
    {
        [ObservableProperty]
        private Cliente clienteActual;

        private readonly ClienteApiService _service;

        [ObservableProperty]
        private string nombre;

        [ObservableProperty]
        private string apellidos;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string telefono;

        [ObservableProperty]
        private int edad; //Se va a mostrar pero no se va a poder editar

        public ClienteEditarPefilViewModel() 
        {
            _service = new ClienteApiService();
        }

        partial void OnClienteActualChanged(Cliente Value)
        {
            if (Value == null) return;
            Nombre = Value.NombreCli;
            Apellidos = Value.ApeCli;
            Email = Value.Correo;
            Telefono = Value.Tel;
            Edad = Value.Edad;
        }

        [RelayCommand]
        private async Task Guardar() 
        {
            if (ClienteActual == null) return;

            ClienteActual.NombreCli = Nombre;
            ClienteActual.ApeCli = Apellidos;
            ClienteActual.Correo = Email;
            ClienteActual.Tel = Telefono;
            // Edad no se modifica

            bool ok = await _service.Actualizar(ClienteActual);
            if (ok)
            {
                await Shell.Current.DisplayAlert("¡Listo!", "Perfil actualizado correctamente.", "OK");
                await Shell.Current.GoToAsync(".."); 
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo actualizar el perfil.", "OK");
            }
        }

        [RelayCommand]
        private async Task Cancelar() 
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
