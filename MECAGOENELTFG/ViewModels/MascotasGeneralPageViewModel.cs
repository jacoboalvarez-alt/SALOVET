using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;
using System.Collections.ObjectModel;

namespace MECAGOENELTFG.ViewModels
{
    internal partial class MascotasGeneralPageViewModel : ObservableObject
    {
        private readonly MascotaApiService m_apiService;
        private readonly ClienteApiService m_clienteService;

        [ObservableProperty]
        private ObservableCollection<Mascota> mascotas;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool isLoading;

        public MascotasGeneralPageViewModel()
        {
            m_apiService = new MascotaApiService();
            m_clienteService = new ClienteApiService();
            mascotas = new ObservableCollection<Mascota>();
        }

        [RelayCommand]
        public async Task CargarMascotas()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                var lista = await m_apiService.ObtenerTodas();

                // Resolver el nombre del cliente para cada mascota
                foreach (var mascota in lista)
                {
                    try
                    {
                        var cliente = await m_clienteService.ObtenerPorId(mascota.IdCliente);
                        if (cliente != null)
                            mascota.Cliente = cliente;
                    }
                    catch
                    {
                        // Si falla la búsqueda de un cliente concreto, continuamos
                    }
                }

                Mascotas.Clear();
                foreach (var mascota in lista)
                    Mascotas.Add(mascota);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"No se han podido cargar las mascotas: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        public async Task EditarMascota(Mascota mascota)
        {
            if (mascota == null) return;
            await Shell.Current.GoToAsync($"mascotaform?mascotaId={mascota.IdMascota}");
        }

        [RelayCommand]
        public async Task EliminarMascota(Mascota mascota)
        {
            if (mascota == null) return;

            bool confirmar = await Shell.Current.DisplayAlert(
                "Confirmar",
                $"¿Eliminar a {mascota.NombreMasc}?",
                "Sí", "No");

            if (!confirmar) return;

            try
            {
                IsLoading = true;
                bool resultado = await m_apiService.Eliminar(mascota.IdMascota);

                if (resultado)
                {
                    Mascotas.Remove(mascota);
                    await Shell.Current.DisplayAlert("Éxito", "Mascota eliminada correctamente", "OK");
                }
                else
                    await Shell.Current.DisplayAlert("Error", "No se pudo eliminar la mascota", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al eliminar: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public static async Task IrADashboard()
        {
            await Shell.Current.GoToAsync("ProfDashBoard");
        }

        [RelayCommand]
        public static async Task IrACrearMascota() 
        {
            await Shell.Current.GoToAsync("mascotaform");
        }

        [RelayCommand]
        public static async Task IrAHistorial(Mascota mascota) 
        {
            if (mascota == null) return;
            await Shell.Current.GoToAsync($"RegistroMascota?idMascota={mascota.IdMascota}&nombre={mascota.NombreMasc}");
        }
    }
}