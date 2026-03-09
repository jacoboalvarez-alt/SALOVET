using MECAGOENELTFG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Services
{
    internal class AuthService
    {

        private readonly HttpClient _httpClient;
        private const string Base_URL = "http://localhost:5201/api/usuarios";

        public AuthService() {
        
           _httpClient =  new HttpClient();
        }

        public async Task<Usuario>? LoginAsync(string Username, string Password)
        {
            try
            {
                var usuario = await _httpClient.GetFromJsonAsync<Usuario>($"{Base_URL}/username/{Username}");

                
                Console.WriteLine($"Usuario: {usuario?.Username}, Pass BD: '{usuario?.Pass}', Pass introducida hasheada: '{HashHelper.HashText(Password)}'");

                if (usuario == null || !HashHelper.VerifyHash(Password, usuario.Pass))
                {
                    return null;
                }
                return usuario;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al hacer el login: {ex.Message}");
                return null;
            }
        }
    }
}
