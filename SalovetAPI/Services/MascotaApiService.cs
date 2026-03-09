using SalovetAPI.Models;
namespace SalovetAPI.Services
{
    public class MascotaApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:4000/api/mascotas"; // Ajusta tu puerto

        public MascotaApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Mascota>> ObtenerTodas()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Mascota>>(BaseUrl) ?? new List<Mascota>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener mascotas: {ex.Message}");
                return new List<Mascota>();
            }
        }

        public async Task<List<Mascota>> ObtenerPorCliente(int idCliente)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Mascota>>($"{BaseUrl}/cliente/{idCliente}")
                    ?? new List<Mascota>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener mascotas del cliente: {ex.Message}");
                return new List<Mascota>();
            }
        }

        public async Task<Mascota?> ObtenerPorId(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Mascota>($"{BaseUrl}/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> Crear(Mascota mascota)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, mascota);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Actualizar(Mascota mascota)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{mascota.IdMascota}", mascota);
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
