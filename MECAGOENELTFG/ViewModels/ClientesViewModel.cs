using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.ViewModels
{
    internal partial class ClientesViewModel : ObservableObject
    {
        private readonly ClienteApiService _service;

        [ObservableProperty]
        private ObservableCollection<Cliente> clientes;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool isLoading;

        public ClientesViewModel()
        {
            _service = new ClienteApiService();
            clientes = new ObservableCollection<Cliente>();

        }

        public async Task InitializeAsync()
        {
            await CargarClientes();
        }

        [RelayCommand]
        public async Task CargarClientes()
        {
            if (IsLoading) return;



            try
            {
                var lista2 = await _service.ObtenerTodos();

                // Log temporal para depurar
                Console.WriteLine($"Registros recibidos: {lista2.Count}");
                if (lista2.Any())
                    Console.WriteLine($"Primer cliente: {lista2[0].IdCliente} - {lista2[0].NombreCli}");

                IsLoading = true;
                var lista = await _service.ObtenerTodos();

                Clientes.Clear();
                foreach (var cliente in lista)
                {
                    Clientes.Add(cliente);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"No se pudieron cargar los clientes: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        public async Task EliminarCliente(Cliente cliente)
        {
            if (cliente == null) return;

            bool confirmar = await Shell.Current.DisplayAlert(
                "Confirmar",
                $"¿Eliminar a {cliente.NombreCli} {cliente.ApeCli}?",
                "Sí",
                "No");

            if (!confirmar) return;

            try
            {
                IsLoading = true;
                bool resultado = await _service.Eliminar(cliente.IdCliente);

                if (resultado)
                {
                    Clientes.Remove(cliente);
                    await Shell.Current.DisplayAlert(
                        "Éxito",
                        "Cliente eliminado correctamente",
                        "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert(
                        "Error",
                        "No se pudo eliminar el cliente",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"Error al eliminar: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task VerMascotas(Cliente cliente)
        {
            if (cliente == null) return;

            // Navegar a la página de mascotas pasando el ID del cliente
            await Shell.Current.GoToAsync($"mascotas?clienteId={cliente.IdCliente}");
        }

        [RelayCommand]
        public async Task AgregarCliente()
        {
            // Navegar a página de formulario
            await Shell.Current.GoToAsync("clienteform");
        }

        [RelayCommand]
        public async Task EditarCliente(Cliente cliente)
        {
            if (cliente == null) return;

            // Navegar a página de formulario con el ID
            await Shell.Current.GoToAsync($"clienteform?clienteId={cliente.IdCliente}");
        }

        [RelayCommand]
        public static async Task IrADashboard() 
        {
            await Shell.Current.GoToAsync("ProfDashBoard");
        }
    }
}
