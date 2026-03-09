using SalovetAPI.Models;

namespace SalovetAPI.Services
{
    public class ClienteApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5201/api/clientes"; 

        public ClienteApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Cliente>> ObtenerTodos()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Cliente>>(BaseUrl) ?? new List<Cliente>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener clientes: {ex.Message}");
                return new List<Cliente>();
            }
        }

        public async Task<Cliente?> ObtenerPorId(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Cliente>($"{BaseUrl}/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> Crear(Cliente cliente)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, cliente);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
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
