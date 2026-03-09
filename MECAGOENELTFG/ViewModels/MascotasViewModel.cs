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
    [QueryProperty(nameof(ClienteId), "clienteId")]
    internal partial class MascotasViewModel : ObservableObject
    {
        private readonly MascotaApiService _mascotaService;
        private readonly ClienteApiService _clienteService;

        [ObservableProperty]
        private ObservableCollection<Mascota> mascotas;

        [ObservableProperty]
        private int clienteId;

        [ObservableProperty]
        private string nombreCliente = "Mascotas";

        [ObservableProperty]
        private bool isLoading;

        public MascotasViewModel()
        {
            _mascotaService = new MascotaApiService();
            _clienteService = new ClienteApiService();
            mascotas = new ObservableCollection<Mascota>();
        }

        partial void OnClienteIdChanged(int value)
        {
            if (value > 0)
            {
                _ = CargarMascotasDelCliente();
            }
        }


        [RelayCommand]
        public async Task CargarMascotasDelCliente()
        {
            if (ClienteId <= 0) return;

            try
            {
                IsLoading = true;

                // Cargar información del cliente
                var cliente = await _clienteService.ObtenerPorId(ClienteId);
                if (cliente != null)
                {
                    NombreCliente = $"Mascotas de {cliente.NombreCli} {cliente.ApeCli}";
                }

                // Cargar mascotas
                var lista = await _mascotaService.ObtenerPorCliente(ClienteId);

                Mascotas.Clear();
                foreach (var mascota in lista)
                {
                    Mascotas.Add(mascota);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"No se pudieron cargar las mascotas: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task EliminarMascota(Mascota mascota)
        {
            if (mascota == null) return;

            bool confirmar = await Shell.Current.DisplayAlert(
                "Confirmar",
                $"¿Eliminar a {mascota.NombreMasc}?",
                "Sí",
                "No");

            if (!confirmar) return;

            try
            {
                IsLoading = true;
                bool resultado = await _mascotaService.Eliminar(mascota.IdMascota);

                if (resultado)
                {
                    Mascotas.Remove(mascota);
                    await Shell.Current.DisplayAlert(
                        "Éxito",
                        "Mascota eliminada correctamente",
                        "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert(
                        "Error",
                        "No se pudo eliminar la mascota",
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
        public async Task AgregarMascota()
        {
            await Shell.Current.GoToAsync($"mascotaform?clienteId={ClienteId}");
        }

        [RelayCommand]
        public async Task EditarMascota(Mascota mascota)
        {
            if (mascota == null) return;

            await Shell.Current.GoToAsync($"mascotaform?mascotaId={mascota.IdMascota}&clienteId={ClienteId}");
        }

        [RelayCommand]
        public async Task Volver()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
