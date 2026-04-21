using MECAGOENELTFG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Services
{
    internal class ProfesionalAPIService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5201/api/profesionales";
        private readonly JsonSerializerOptions _jsonOptions;

        public ProfesionalAPIService() 
        {
            _httpClient = new HttpClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
        }

        public async Task<List<Profesional>> ObtenerTodos()
        {
            try
            {
                var json = await _httpClient.GetStringAsync(BaseUrl);
                var lista = JsonSerializer.Deserialize<List<Profesional>>(json, _jsonOptions);
                return lista ?? new List<Profesional>();
            } catch (Exception ex) 
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Profesional>();
            }
        }

        public async Task<Profesional> ObtenerProfPorID(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Profesional>($"{BaseUrl}/{id}");
            }
            catch (Exception ex) 
            {
                return null;
            }
        }

        public async Task<List<Profesional>> ObtenerVeterinarios()
        {
            try
            {
                var json = await _httpClient.GetStringAsync($"{BaseUrl}/grado/VETERINARIO");
                return JsonSerializer.Deserialize<List<Profesional>>(json, _jsonOptions) ?? new List<Profesional>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener veterinarios: {ex.Message}");
                return new List<Profesional>();
            }
        }


    }
}
