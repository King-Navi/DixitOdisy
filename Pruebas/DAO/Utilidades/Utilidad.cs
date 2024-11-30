using Pruebas.Servidor.Utilidades;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Pruebas.DAO
{
    public static class Utilidad
    {
        private static Random random = new Random();

        public static string ObtenerSHA256Hash(string entrada)
        {

            using (SHA256 algoritmoSHA256 = SHA256.Create())
            {
                byte[] arregloByte = algoritmoSHA256.ComputeHash(Encoding.UTF8.GetBytes(entrada));
                StringBuilder stringContruido = new StringBuilder();
                foreach (byte byteActual in arregloByte)
                {
                    stringContruido.Append(byteActual.ToString("x2"));
                }
                return stringContruido.ToString();
            }
        }

        public static byte[] GenerarBytesAleatorios(int longitud)
        {
            byte[] bytes = new byte[longitud];
            Random aleatorio = new Random();
            aleatorio.NextBytes(bytes);
            return bytes;
        }

        public static DAOLibreria.ModeloBD.Usuario GenerarUsuarioDePrueba()
        {
            string gamertagAleatorio = "JugadorPrueba" + new Random().Next(1000, 9999);
            byte[] fotoPerfilAleatoria = Utilidad.GenerarBytesAleatorios(256);

            return new DAOLibreria.ModeloBD.Usuario
            {
                gamertag = gamertagAleatorio,
                fotoPerfil = fotoPerfilAleatoria
            };
        }

        public static string GenerarCadenaAleatoria(int longitud)
        {
            const string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(caracteres, longitud)
                                        .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static DAOLibreria.ModeloBD.UsuarioCuenta GenerarUsuarioCuentaDePrueba(string gamertag)
        {
            string contraseniaAleatoria = "Contraseña" + new Random().Next(1000, 9999);
            string hashContrasenia = Utilidad.ObtenerSHA256Hash(contraseniaAleatoria);
            string correoAleatorio = $"{gamertag.ToLower()}@ejemplo.com";

            return new DAOLibreria.ModeloBD.UsuarioCuenta
            {
                gamertag = gamertag,
                hashContrasenia = hashContrasenia,
                correo = correoAleatorio
            };
        }

    }

}
