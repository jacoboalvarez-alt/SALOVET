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

        [RelayCommand]
        private async Task GuardarCommand() 
        {
            Cliente nuevo = new Cliente(nombre,apellidos,edad,email,telefono);
            _service.Actualizar(nuevo);
        }

        [RelayCommand]
        private async Task CancelarCommand() 
        {
            await Shell.Current.GoToAsync("PerfilPage");
        }
    }
}
