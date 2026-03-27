using MECAGOENELTFG.Models;
using System.Text;
using System.Text.Json;

namespace MECAGOENELTFG.Services
{
    internal class CitasAPIService
    {
        private readonly HttpClient _httpClient;
        private const string BaseURL = "http://localhost:5201/api/citas";
        private readonly JsonSerializerOptions _jsonOptions;

        public CitasAPIService()
        {
            _httpClient = new HttpClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
        }

        // GET: api/citas
        public async Task<List<Cita>> ObtenerCitas()
        {
            try
            {
                var json = await _httpClient.GetStringAsync(BaseURL);
                return JsonSerializer.Deserialize<List<Cita>>(json, _jsonOptions) ?? new List<Cita>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Cita>();
            }
        }

        // GET: api/citas/5
        public async Task<Cita?> ObtenerCita(int id)
        {
            try
            {
                var json = await _httpClient.GetStringAsync($"{BaseURL}/{id}");
                return JsonSerializer.Deserialize<Cita>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        // GET: api/citas/cliente/5
        public async Task<List<Cita>> ObtenerCitasPorCliente(int idCliente)
        {
            try
            {
                var json = await _httpClient.GetStringAsync($"{BaseURL}/cliente/{idCliente}");
                return JsonSerializer.Deserialize<List<Cita>>(json, _jsonOptions) ?? new List<Cita>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Cita>();
            }
        }

        // GET: api/citas/profesional/5
        public async Task<List<Cita>> ObtenerCitasPorProfesional(int idProfesional)
        {
            try
            {
                var json = await _httpClient.GetStringAsync($"{BaseURL}/profesional/{idProfesional}");
                return JsonSerializer.Deserialize<List<Cita>>(json, _jsonOptions) ?? new List<Cita>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Cita>();
            }
        }

        // GET: api/citas/estado/PENDIENTE
        public async Task<List<Cita>> ObtenerCitasPorEstado(string estado)
        {
            try
            {
                var json = await _httpClient.GetStringAsync($"{BaseURL}/estado/{estado}");
                return JsonSerializer.Deserialize<List<Cita>>(json, _jsonOptions) ?? new List<Cita>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Cita>();
            }
        }

        // GET: api/citas/fecha/2025-02-10
        public async Task<List<Cita>> ObtenerCitasPorFecha(DateTime fecha)
        {
            try
            {
                var fechaFormateada = fecha.ToString("yyyy-MM-dd");
                var json = await _httpClient.GetStringAsync($"{BaseURL}/fecha/{fechaFormateada}");
                return JsonSerializer.Deserialize<List<Cita>>(json, _jsonOptions) ?? new List<Cita>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Cita>();
            }
        }

        // POST: api/citas
        public async Task<Cita?> CrearCita(Cita cita)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(cita, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(BaseURL, content);
                response.EnsureSuccessStatusCode();

                var jsonRespuesta = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Cita>(jsonRespuesta, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        // PUT: api/citas/5
        public async Task<bool> ActualizarCita(int id, Cita cita)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(cita, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{BaseURL}/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        // PATCH: api/citas/5/estado
        public async Task<bool> CambiarEstadoCita(int id, string nuevoEstado)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(nuevoEstado, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"{BaseURL}/{id}/estado", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        // DELETE: api/citas/5
        public async Task<bool> EliminarCita(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseURL}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}