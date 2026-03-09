using MECAGOENELTFG.Models;
using System.Net.Http.Json;


namespace MECAGOENELTFG.Services
{
    internal class RegistroService
    {
        private readonly HttpClient _httpClient;
        private const string UrlCliente = "http://localhost:5201/api/clientes";
        private const string UrlUsuario = "http://localhost:5201/api/usuarios";

        public RegistroService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Registra un cliente y su usuario asociado en la API.
        /// Devuelve null si todo fue bien, o un mensaje de error si algo falló.
        /// </summary>
        public async Task<string?> RegistroAsync(Usuario usuario, Cliente cliente)
        {
            try
            {
                var (idCliente, errorCliente) = await AgregarCliente(cliente);
                if (errorCliente != null) return errorCliente;

                usuario.IdCliente = idCliente;
                usuario.Pass = HashHelper.HashText(usuario.Pass!);
                usuario.Profesional = false;
                usuario.Cliente = null; 

                string? errorUsuario = await AgregarUsuario(usuario);
                return errorUsuario;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el registro: {ex.Message}");
                return $"Error inesperado: {ex.Message}";
            }
        }

        /// <summary>
        /// Envía el cliente a la API y devuelve su ID generado.
        /// Devuelve el ID y null si fue bien, o null y mensaje de error si falló.
        /// </summary>
        public async Task<(int? IdCliente, string? Error)> AgregarCliente(Cliente cliente)
        {

            try
            {
                var response = await _httpClient.PostAsJsonAsync(UrlCliente, cliente);

                // ← Añade estas dos líneas para ver qué responde la API
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {response.StatusCode}, Body: {responseBody}");

                if (!response.IsSuccessStatusCode)
                    return (null, "No se pudo crear el cliente.");

                var clienteCreado = await response.Content.ReadFromJsonAsync<Cliente>();
                return (clienteCreado?.IdCliente, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar cliente: {ex.Message}");
                return (null, $"Error al conectar con el servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Envía el usuario a la API.
        /// Devuelve null si se creó correctamente, o un mensaje de error si falló.
        /// Maneja el 409 Conflict si el username ya existe.
        /// </summary>
        public async Task<string?> AgregarUsuario(Usuario usuario)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(UrlUsuario, usuario);

                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {response.StatusCode}, Body: {responseBody}");

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    return "El nombre de usuario ya está en uso.";

                if (!response.IsSuccessStatusCode)
                    return "No se pudo crear el usuario.";

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar usuario: {ex.Message}");
                return $"Error al conectar con el servidor: {ex.Message}";
            }
        }
    }
}