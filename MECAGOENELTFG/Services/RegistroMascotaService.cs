using MECAGOENELTFG.Models;
using System.Net.Http.Json;

namespace MECAGOENELTFG.Services
{
    public class RegistroMascotaService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5201/api/RegistroMascota";

        public RegistroMascotaService()
        {
            _httpClient = new HttpClient();
        }

        // ============================
        // GET: Obtener todos los registros
        // ============================
        public async Task<List<RegistroMascota>> ObtenerTodos()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<RegistroMascota>>(BaseUrl)
                       ?? new List<RegistroMascota>();
            }
            catch
            {
                Console.WriteLine("Error al obtener los registros médicos");
                return new List<RegistroMascota>();
            }
        }

        // ============================
        // GET: Obtener registros por mascota
        // ============================
        public async Task<List<RegistroMascota>> ObtenerPorMascota(int idMascota)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<RegistroMascota>>(
                    $"{BaseUrl}/mascota/{idMascota}"
                ) ?? new List<RegistroMascota>();
            }
            catch
            {
                Console.WriteLine($"No se pudieron recuperar los registros de la mascota {idMascota}");
                return new List<RegistroMascota>();
            }
        }

        // ============================
        // POST: Crear un nuevo registro
        // ============================
        public async Task<bool> Crear(RegistroMascota registro)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, registro);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al crear: {response.StatusCode} - {body}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                Console.WriteLine($"StatusCode: {ex.StatusCode}");
                Console.WriteLine($"Inner: {ex.InnerException?.Message}");
                return false;
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Timeout o cancelación: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción general: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine($"Inner: {ex.InnerException?.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        // ============================
        // PUT: Actualizar un registro
        // ============================
        public async Task<bool> Actualizar(int id, RegistroMascota registro)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", registro);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                Console.WriteLine($"Error al actualizar el registro {id}");
                return false;
            }
        }

        // ============================
        // DELETE: Eliminar un registro
        // ============================
        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                Console.WriteLine($"Error al eliminar el registro {id}");
                return false;
            }
        }
    }
}
