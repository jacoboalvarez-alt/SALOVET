using MECAGOENELTFG.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MECAGOENELTFG.Services
{
    public class ClienteApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5201/api/clientes";
        private readonly JsonSerializerOptions _jsonOptions;

        public ClienteApiService()
        {
            _httpClient = new HttpClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
        }

        public async Task<List<Cliente>> ObtenerTodos()
        {
            try
            {
                var json = await _httpClient.GetStringAsync(BaseUrl);
                Console.WriteLine($"JSON recibido: {json}");

                var lista = JsonSerializer.Deserialize<List<Cliente>>(json, _jsonOptions);
                Console.WriteLine($"Clientes deserializados: {lista?.Count ?? 0}");

                return lista ?? new List<Cliente>();
            }
            catch (Exception ex)
            {
                // Muestra el error completo con stack trace
                Console.WriteLine($"ERROR COMPLETO: {ex}");
                return new List<Cliente>();
            }
        }

        public async Task<Cliente?> ObtenerPorId(int id)
        {
            try
            {
                var json = await _httpClient.GetStringAsync($"{BaseUrl}/{id}");
                Console.WriteLine($">>> JSON cliente: {json}");
                var cliente = JsonSerializer.Deserialize<Cliente>(json, _jsonOptions);
                Console.WriteLine($">>> Cliente deserializado: {cliente?.NombreCli}");
                return cliente;
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> ERROR ObtenerPorId: {ex.Message}");
                return null;
            }
        }

        public async Task<Cliente?> Crear(Cliente cliente)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, cliente);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Cliente>(json, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> Actualizar(Cliente cliente)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{cliente.IdCliente}", cliente);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }


    }
}
