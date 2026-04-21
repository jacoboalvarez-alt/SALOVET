using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MECAGOENELTFG.Models;
using MECAGOENELTFG.Services;

namespace MECAGOENELTFG.ViewModels
{
    [QueryProperty(nameof(MascotaId), "mascotaId")]
    [QueryProperty(nameof(ClienteId), "clienteId")]
    public partial class MascotaFormViewModel : ObservableObject
    {
        private readonly MascotaApiService _mascotaService;
        private readonly ClienteApiService _clienteService;

        // ── Query params ──────────────────────────────────────────────
        [ObservableProperty] private int mascotaId;
        [ObservableProperty] private int clienteId;

        // ── Campos del formulario ─────────────────────────────────────
        [ObservableProperty] private string nombreMasc = string.Empty;
        [ObservableProperty] private string especie = string.Empty;
        [ObservableProperty] private string raza = string.Empty;
        [ObservableProperty] private string edad = string.Empty;  // string para el Entry numérico
        [ObservableProperty] private string sexo = string.Empty;
        [ObservableProperty] private string color = string.Empty;
        [ObservableProperty] private string tamano = string.Empty;
        [ObservableProperty] private string tipoPelo = string.Empty;
        [ObservableProperty] private bool vacunado;
        [ObservableProperty] private string notas = string.Empty;

        // ── UI state ──────────────────────────────────────────────────
        [ObservableProperty] private string tituloFormulario = "Agregar Mascota";
        [ObservableProperty] private string nombreCliente = string.Empty;
        [ObservableProperty] private bool isLoading;

        // Visibilidad dinámica según especie
        [ObservableProperty] private bool sizeFieldVisible;    // solo Perro
        [ObservableProperty] private bool hairFieldVisible;    // solo Gato
        [ObservableProperty] private bool speciesFieldVisible; // el resto

        public bool EsEdicion => MascotaId > 0;

        // ── Listas para los Pickers ───────────────────────────────────
        public List<string> EspeciesDisponibles { get; } = new()
            { "Perro", "Gato", "Ave", "Conejo", "Hámster", "Reptil", "Pez", "Otro" };

        public List<string> TamanosDisponibles { get; } = new()
            { "Pequeño", "Mediano", "Grande" };

        public List<string> TiposPeloDisponibles { get; } = new()
            { "Corto", "Largo", "Sin pelo" };

        public MascotaFormViewModel()
        {
            _mascotaService = new MascotaApiService();
            _clienteService = new ClienteApiService();
        }

        // ── Lógica de visibilidad al cambiar especie ──────────────────
        partial void OnEspecieChanged(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            string esp = value.ToLower();
            SizeFieldVisible = esp == "perro";
            HairFieldVisible = esp == "gato";
            SpeciesFieldVisible = esp != "perro" && esp != "gato";
        }

        partial void OnMascotaIdChanged(int value)
        {
            if (value > 0)
            {
                TituloFormulario = "Editar Mascota";
                _ = CargarMascota();
            }
            else
            {
                TituloFormulario = "Agregar Mascota";
            }
        }

        partial void OnClienteIdChanged(int value)
        {
            if (value > 0) _ = CargarNombreCliente();
        }

        // ── Carga de datos ────────────────────────────────────────────
        private async Task CargarNombreCliente()
        {
            try
            {
                var cliente = await _clienteService.ObtenerPorId(ClienteId);
                if (cliente != null)
                    NombreCliente = $"{cliente.NombreCli} {cliente.ApeCli}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar cliente: {ex.Message}");
            }
        }

        private async Task CargarMascota()
        {
            try
            {
                IsLoading = true;
                var mascota = await _mascotaService.ObtenerPorId(MascotaId);

                if (mascota != null)
                {
                    NombreMasc = mascota.NombreMasc;
                    Especie = mascota.Especie;         // dispara OnEspecieChanged → visibilidad
                    Raza = mascota.Raza ?? string.Empty;
                    Edad = mascota.Edad?.ToString() ?? string.Empty;
                    Sexo = mascota.Sexo ?? string.Empty;
                    Color = mascota.Color ?? string.Empty;
                    Tamano = mascota.Tamano ?? string.Empty;
                    TipoPelo = mascota.TipoPelo ?? string.Empty;
                    Vacunado = mascota.Vacunado;
                    Notas = mascota.Notas ?? string.Empty;
                    ClienteId = mascota.IdCliente;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cargar mascota: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ── Guardar ───────────────────────────────────────────────────
        [RelayCommand]
        public async Task Guardar()
        {
            if (string.IsNullOrWhiteSpace(NombreMasc))
            {
                await Shell.Current.DisplayAlert("Error", "El nombre de la mascota es obligatorio", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Especie))
            {
                await Shell.Current.DisplayAlert("Error", "La especie es obligatoria", "OK");
                return;
            }

            if (ClienteId <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "No se ha especificado un cliente válido", "OK");
                return;
            }

            int? edadParsed = null;
            if (!string.IsNullOrWhiteSpace(Edad))
            {
                if (!int.TryParse(Edad, out int edadVal) || edadVal < 0 || edadVal > 50)
                {
                    await Shell.Current.DisplayAlert("Error", "La edad debe ser un número entre 0 y 50", "OK");
                    return;
                }
                edadParsed = edadVal;
            }

            try
            {
                IsLoading = true;

                var mascota = new Mascota
                {
                    IdMascota = MascotaId,
                    IdCliente = ClienteId,
                    NombreMasc = NombreMasc.Trim(),
                    Especie = Especie.Trim(),
                    Raza = string.IsNullOrWhiteSpace(Raza) ? null : Raza.Trim(),
                    Edad = edadParsed,
                    Sexo = string.IsNullOrWhiteSpace(Sexo) ? null : Sexo,
                    Color = string.IsNullOrWhiteSpace(Color) ? null : Color.Trim(),
                    Tamano = string.IsNullOrWhiteSpace(Tamano) ? null : Tamano,
                    TipoPelo = string.IsNullOrWhiteSpace(TipoPelo) ? null : TipoPelo,
                    Vacunado = Vacunado,
                    Notas = string.IsNullOrWhiteSpace(Notas) ? null : Notas.Trim()
                };

                bool resultado = EsEdicion
                    ? await _mascotaService.Actualizar(mascota)
                    : await _mascotaService.Crear(mascota);

                if (resultado)
                {
                    await Shell.Current.DisplayAlert(
                        "Éxito",
                        EsEdicion ? "Mascota actualizada correctamente" : "Mascota creada correctamente",
                        "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo guardar la mascota", "OK");
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

        // ── Cancelar ──────────────────────────────────────────────────
        [RelayCommand]
        public async Task Cancelar()
        {
            bool confirmar = true;

            if (!string.IsNullOrEmpty(NombreMasc) || !string.IsNullOrEmpty(Especie))
            {
                confirmar = await Shell.Current.DisplayAlert(
                    "Confirmar", "¿Descartar los cambios?", "Sí", "No");
            }

            if (confirmar)
                await Shell.Current.GoToAsync("..");
        }
    }
}