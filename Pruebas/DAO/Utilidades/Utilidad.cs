using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas.DAO
{
    public static class Utilidad
    {
        public static string ObtenerSHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static byte[] GenerarBytesAleatorios(int longitud)
        {
            byte[] bytes = new byte[longitud];
            Random rnd = new Random();
            rnd.NextBytes(bytes);
            return bytes;
        }
    }
}
