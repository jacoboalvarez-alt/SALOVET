using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Models
{
    public static class HashHelper
    {
        /// <summary>
        /// Transforma un texto plano en un hash SHA256 en formato Base64.
        /// </summary>
        public static string HashText(string text)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Compara un texto plano con un hash SHA256.
        /// Devuelve true si el texto coincide con el hash.
        /// </summary>
        public static bool VerifyHash(string text, string hash)
        {
            var hashedText = HashText(text);
            return hashedText == hash;
        }
    }
}
