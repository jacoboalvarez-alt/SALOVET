using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;
using System.Collections.ObjectModel;

namespace MECAGOENELTFG.ViewModels2
{
    public partial class PerfilPageViewModel : ObservableObject
    {
        private readonly ClienteApiService _clienteService;
        private readonly MascotaApiService _mascotaService;

        [ObservableProperty] private string nombreCompleto = "";
        [ObservableProperty] private string iniciales = "";
        [ObservableProperty] private string clienteDesde = "";
        [ObservableProperty] private string email = "";
        [ObservableProperty] private string telefono = "";
        [ObservableProperty] private int edad = 0;
        [ObservableProperty] private Cliente clienteLogueado;
        public ObservableCollection<MascotaItem> Mascotas { get; } = new();

        public PerfilPageViewModel()
        {
            _clienteService = new ClienteApiService();
            _mascotaService = new MascotaApiService();
            CargarDatosAsync().ConfigureAwait(false);
        }

        public async Task CargarDatosAsync()
        {
            int id = SessionService.IdClienteActual;
            Console.WriteLine($">>> ID cargado: {id}");
            if (id == 0) return;

            var cliente = await _clienteService.ObtenerPorId(id);
            if (cliente == null) return;

            NombreCompleto = $"{cliente.NombreCli} {cliente.ApeCli}";
            Iniciales = $"{cliente.NombreCli.FirstOrDefault()}{cliente.ApeCli.FirstOrDefault()}".ToUpper();
            Email = cliente.Correo;
            Telefono = cliente.Tel ?? "Sin teléfono";
            Edad = cliente.Edad; 
            ClienteDesde = "Cliente activo";

            var mascotas = await _mascotaService.ObtenerPorCliente(id);
            Mascotas.Clear();
            foreach (var m in mascotas)
            {
                Mascotas.Add(new MascotaItem
                {
                    Nombre = m.NombreMasc,
                    Descripcion = $"{m.Especie}{(m.Raza != null ? " · " + m.Raza : "")} · {m.Edad} años",
                    Icono = m.Especie.ToLower() switch
                    {
                        "perro" => "🐕",
                        "gato" => "🐈",
                        _ => "🐾"
                    }
                });
            }
        }

        [RelayCommand]
        private async Task Editar()
        {
            // Verificamos que tengamos datos antes de intentar navegar
            if (ClienteLogueado == null) return;

            var navigationParameter = new Dictionary<string, object>
         {
        { "ClienteActual", ClienteLogueado }
                 };

            // Ojo: He corregido el paréntesis que tenías mal puesto
            await Shell.Current.GoToAsync("ClienteEditarPerfilPage", navigationParameter);
        }


        //[RelayCommand]
        //private async Task VerMascota(MascotaItem mascota) =>
        //    await Shell.Current.GoToAsync("EditPagePet");

        [RelayCommand]
        private async Task AgregarMascota() =>
            await Shell.Current.GoToAsync("AgregarMascotaClient");

        [RelayCommand]
        private async Task CerrarSesion()
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Cerrar sesión", "¿Seguro que quieres salir?", "Sí", "Cancelar");
            if (confirmar)
            {
                SessionService.IdClienteActual = 0;
                await Shell.Current.GoToAsync("//Login");
            }
        }
    }

    // Clase auxiliar de presentación
    public class MascotaItem
    {
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string Icono { get; set; } = "";
    }
}