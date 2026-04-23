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
    public class FacturasService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions; 
        private const string BaseURL = "http://localhost:5201/api/facturas";

        public FacturasService()
        {
            _httpClient = new HttpClient();
            _jsonOptions = new JsonSerializerOptions             
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
        }

        public async Task<List<Factura>> ObtenerTodasFacturas()
        {
            try
            {
                var json = await _httpClient.GetStringAsync(BaseURL); 
                return JsonSerializer.Deserialize<List<Factura>>(json, _jsonOptions) ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener facturas: {ex.Message}");
                return new();
            }
        }

        public async Task<Factura?> ObtenerPorId(int id)
        {
            try
            {
                var json = await _httpClient.GetStringAsync($"{BaseURL}/{id}");
                return JsonSerializer.Deserialize<Factura>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener factura {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CrearFactura(Factura factura)
        {
            try
            {
                // Construimos solo los campos que necesita la API, sin navegación
                var dto = new
                {
                    IdCita = factura.IdCita,
                    Monto = factura.Monto,
                    FechaEmision = factura.FechaEmision,
                    EstadoPago = (int)factura.EstadoPago  // Forzamos entero explícitamente
                };

                var json = JsonSerializer.Serialize(dto, _jsonOptions);
                Console.WriteLine("=== JSON ENVIADO ===");
                Console.WriteLine(json);

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(BaseURL, content);
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"=== STATUS: {(int)response.StatusCode} ===");
                Console.WriteLine($"=== BODY: {body} ===");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== EXCEPCIÓN: {ex.Message} ===");
                return false;
            }
        }

        // Problema 2 ↓
        public async Task<bool> MarcarComoPagada(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync(
                    $"{BaseURL}/{id}/pagar",
                    null); 
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al marcar como pagada la factura {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseURL}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar factura {id}: {ex.Message}");
                return false;
            }
        }
    }
}
