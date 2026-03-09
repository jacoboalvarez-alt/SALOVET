using SalovetAPI.Models;
using System.Net.Http.Json;

namespace SalovetAPI.Services
{
    public class UsuarioApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:4000/api/usuarios";

        public UsuarioApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Usuario>> ObtenerTodos()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Usuario>>(BaseUrl) ?? new List<Usuario>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                return new List<Usuario>();
            }
        }

        public async Task<Usuario?> ObtenerPorId(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Usuario>($"{BaseUrl}/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<Usuario?> ObtenerPorUsername(string username)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Usuario>($"{BaseUrl}/username/{username}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> Crear(Usuario usuario)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, usuario);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Actualizar(Usuario usuario)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{usuario.IdUsuario}", usuario);
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