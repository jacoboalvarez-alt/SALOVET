using MECAGOENELTFG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Services
{
    public class ServiciosService
    {
        private readonly HttpClient _httpClient;
        private const string BaseURL = "http://localhost:5201/api/servicios";
        
        public ServiciosService() 
        { 
            _httpClient = new HttpClient();
        }

        public async Task <List<Servicio>> ObtenerTodosServicios()
        {
            try 
            {
                return await _httpClient.GetFromJsonAsync<List<Servicio>>(BaseURL) ?? new List<Servicio>();
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Error al obtener los servicios: {ex.Message}");
                return new List<Servicio>();
            }
        }

        public async Task <Servicio> ObtenerPorId(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Servicio>($"{BaseURL}/{id}");
            }
            catch (Exception ex) 
            {
                Console.Write($"Error al encontrar Servicio con Id {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> Crear(Servicio servicio)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseURL, servicio);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error no se ha podido crear el servicio {servicio.NomServicio} : {ex.Message}");            
                return false;
            }
        }

        public async Task<bool> Actualizar(Servicio servicio)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseURL}/{servicio.IdServicio}", servicio);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error no se ha podido actualizar el servicio: {ex.Message}");
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
                Console.WriteLine("No se ha podido eliminar el servicio con la ID: ");
                return false;
            }
        }

    }
}
