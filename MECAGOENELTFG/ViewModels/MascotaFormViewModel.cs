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

        // TipoAnimal = lo que el picker muestra (Perro / Gato / Otro)
        // Especie    = lo que se guarda en el modelo (puede ser texto libre si Otro)
        [ObservableProperty] private string tipoAnimal = string.Empty;
        [ObservableProperty] private string especiePersonalizada = string.Empty; // solo visible cuando Otro
        [ObservableProperty] private string raza = string.Empty;
        [ObservableProperty] private string edad = string.Empty;
        [ObservableProperty] private string sexo = string.Empty;
        [ObservableProperty] private string color = string.Empty;
        [ObservableProperty] private string tamano = string.Empty;
        [ObservableProperty] private string tipoPelo = string.Empty;
        [ObservableProperty] private bool vacunado;
        [ObservableProperty] private string notas = string.Empty;

        // ── Selector de cliente ───────────────────────────────────────
        [ObservableProperty] private List<Cliente> clientesDisponibles = new();
        [ObservableProperty] private Cliente? clienteSeleccionado;
        [ObservableProperty] private bool mostrarSelectorCliente = false;

        // ── UI state ──────────────────────────────────────────────────
        [ObservableProperty] private string tituloFormulario = "Agregar Mascota";
        [ObservableProperty] private string nombreCliente = string.Empty;
        [ObservableProperty] private bool isLoading;

        // Visibilidad dinámica según tipo
        [ObservableProperty] private bool sizeFieldVisible;   // solo Perro
        [ObservableProperty] private bool hairFieldVisible;   // solo Gato
        [ObservableProperty] private bool especieFieldVisible; // solo Otro

        public bool EsEdicion => MascotaId > 0;

        // ── Listas para los Pickers ───────────────────────────────────
        // El picker principal solo distingue los 3 tipos
        public List<string> TiposAnimal { get; } = new() { "Perro", "Gato", "Otro" };
        public List<string> TamanosDisponibles { get; } = new() { "Pequeño", "Mediano", "Grande" };
        public List<string> TiposPeloDisponibles { get; } = new() { "Corto", "Largo", "Sin pelo" };

        public MascotaFormViewModel()
        {
            _mascotaService = new MascotaApiService();
            _clienteService = new ClienteApiService();
            MostrarSelectorCliente = true;
            _ = CargarClientes();
        }

        // ── Lógica de visibilidad al cambiar tipo ─────────────────────
        partial void OnTipoAnimalChanged(string value)
        {
            SizeFieldVisible = value == "Perro";
            HairFieldVisible = value == "Gato";
            EspecieFieldVisible = value == "Otro";
        }

        // ── Lógica del selector de cliente ───────────────────────────
        partial void OnClienteIdChanged(int value)
        {
            // Ya no controla la visibilidad del picker, solo carga el nombre si viene por query
            if (value > 0) _ = CargarNombreCliente();
        }

        partial void OnClienteSeleccionadoChanged(Cliente? value)
        {
            if (value == null) return;
            ClienteId = value.IdCliente;
            NombreCliente = $"{value.NombreCli} {value.ApeCli}";
        }

        partial void OnMascotaIdChanged(int value)
        {
            if (value > 0)
            {
                TituloFormulario = "Editar Mascota";
                MostrarSelectorCliente = false; 
                _ = CargarMascota();
            }
            else
            {
                TituloFormulario = "Agregar Mascota";
                MostrarSelectorCliente = true; 
            }
        }

        // ── Carga de clientes para el picker ─────────────────────────
        private async Task CargarClientes()
        {
            try
            {
                var lista = await _clienteService.ObtenerTodos();
                ClientesDisponibles = lista?.ToList() ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar clientes: {ex.Message}");
            }
        }

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

        // ── Carga de mascota para edición ─────────────────────────────
        private async Task CargarMascota()
        {
            try
            {
                IsLoading = true;
                var mascota = await _mascotaService.ObtenerPorId(MascotaId);
                if (mascota == null) return;

                NombreMasc = mascota.NombreMasc;
                Raza = mascota.Raza ?? string.Empty;
                Edad = mascota.Edad?.ToString() ?? string.Empty;
                Sexo = mascota.Sexo ?? string.Empty;
                Color = mascota.Color ?? string.Empty;
                Tamano = mascota.Tamano ?? string.Empty;
                TipoPelo = mascota.TipoPelo ?? string.Empty;
                Vacunado = mascota.Vacunado;
                Notas = mascota.Notas ?? string.Empty;
                ClienteId = mascota.IdCliente;

                // Restaurar TipoAnimal y EspeciePersonalizada según lo guardado
                string especieGuardada = mascota.Especie ?? string.Empty;
                if (especieGuardada == "Perro" || especieGuardada == "Gato")
                {
                    TipoAnimal = especieGuardada;
                }
                else
                {
                    TipoAnimal = "Otro";
                    EspeciePersonalizada = especieGuardada; // "Ave", "Conejo", texto libre...
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

            if (string.IsNullOrWhiteSpace(TipoAnimal))
            {
                await Shell.Current.DisplayAlert("Error", "El tipo de animal es obligatorio", "OK");
                return;
            }

            if (TipoAnimal == "Otro" && string.IsNullOrWhiteSpace(EspeciePersonalizada))
            {
                await Shell.Current.DisplayAlert("Error", "Indica la especie del animal", "OK");
                return;
            }

            if (ClienteId <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Debes seleccionar un cliente", "OK");
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

            // La especie guardada en el modelo:
            // Perro → "Perro", Gato → "Gato", Otro → texto libre
            string especieAGuardar = TipoAnimal == "Otro"
                ? EspeciePersonalizada.Trim()
                : TipoAnimal;

            try
            {
                IsLoading = true;
                var mascota = new Mascota
                {
                    IdMascota = MascotaId,
                    IdCliente = ClienteId,
                    NombreMasc = NombreMasc.Trim(),
                    Especie = especieAGuardar,
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

        [RelayCommand]
        public async Task Cancelar()
        {
            bool confirmar = true;
            if (!string.IsNullOrEmpty(NombreMasc) || !string.IsNullOrEmpty(TipoAnimal))
                confirmar = await Shell.Current.DisplayAlert("Confirmar", "¿Descartar los cambios?", "Sí", "No");

            if (confirmar)
                await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task Volver() 
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}