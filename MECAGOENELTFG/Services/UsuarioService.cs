using MECAGOENELTFG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Services
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        private const string BASEURL = "http://localhost:5201/api/usuarios";

        public UsuarioService() 
        { 
            _httpClient = new HttpClient();
        }

        public async Task<bool> Actualizar(Usuario usuario)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BASEURL}/{usuario.IdUsuario}", usuario);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($">>> ERROR COMPLETO: {ex}");  
                return false;
            }
        }

        public async Task<bool> Crear(Usuario usuario) 
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BASEURL, usuario);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error: {ex} Definicion: {ex.Message}");
                return false;
            }
        }
    }
}

