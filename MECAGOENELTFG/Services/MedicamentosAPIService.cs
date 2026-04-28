using MECAGOENELTFG.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MECAGOENELTFG.Services
{
    public class MedicamentosAPIService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5201/api/medicamentos";
        private readonly JsonSerializerOptions _jsonOptions;

        public MedicamentosAPIService()
        {
            _httpClient = new HttpClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // GET: api/medicamentos
        public async Task<List<Medicamento>> ObtenerMedicamentos()
        {
            try
            {
                var json = await _httpClient.GetStringAsync(BaseUrl);
                return JsonSerializer.Deserialize<List<Medicamento>>(json, _jsonOptions) ?? new();
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); return new(); }
        }

        // GET: api/medicamentos/disponibles
        public async Task<List<Medicamento>> ObtenerDisponibles()
        {
            try
            {
                var json = await _httpClient.GetStringAsync($"{BaseUrl}/disponibles");
                return JsonSerializer.Deserialize<List<Medicamento>>(json, _jsonOptions) ?? new();
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); return new(); }
        }

        // GET: api/medicamentos/bajo-stock/{limite}
        public async Task<List<Medicamento>> ObtenerBajoStock(int limite)
        {
            try
            {
                var json = await _httpClient.GetStringAsync($"{BaseUrl}/bajo-stock/{limite}");
                return JsonSerializer.Deserialize<List<Medicamento>>(json, _jsonOptions) ?? new();
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); return new(); }
        }

        // PATCH: api/medicamentos/{id}/stock
        public async Task<bool> ActualizarStock(int id, int nuevoStock)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(nuevoStock),
                    Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync($"{BaseUrl}/{id}/stock", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); return false; }
        }

        // POST: api/medicamentos
        public async Task<Medicamento?> CrearMedicamento(Medicamento medicamento)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(medicamento, _jsonOptions),
                    Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(BaseUrl, content);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Medicamento>(json, _jsonOptions);
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); return null; }
        }

        // PUT: api/medicamentos/{id}
        public async Task<bool> ActualizarMedicamento(int id, Medicamento medicamento)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(medicamento, _jsonOptions),
                    Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{BaseUrl}/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); return false; }
        }

        // DELETE: api/medicamentos/{id}
        public async Task<bool> EliminarMedicamento(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); return false; }
        }
    }
}