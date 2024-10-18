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
        public static (DAOLibreria.ModeloBD.Usuario, DAOLibreria.ModeloBD.UsuarioCuenta) PrepararUsuarioExistente()
        {
            string gamertagExistente = "UsuarioExistente";
            string contraseniaExistente = "Contrasena1234";
            string hashContraseniaExistente = Utilidad.ObtenerSHA256Hash(contraseniaExistente);
            string correoExistente = "usuarioexistente@ejemplo.com";
            byte[] fotoPerfilExistente = Utilidad.GenerarBytesAleatorios(256);

            var usuarioCuentaExistente = new DAOLibreria.ModeloBD.UsuarioCuenta
            {
                gamertag = gamertagExistente,
                hashContrasenia = hashContraseniaExistente,
                correo = correoExistente
            };

            var usuarioExistente = new DAOLibreria.ModeloBD.Usuario
            {
                gamertag = gamertagExistente,
                fotoPerfil = fotoPerfilExistente,
                idUsuarioCuenta = usuarioCuentaExistente.idUsuarioCuenta
            };
            return (usuarioExistente, usuarioCuentaExistente);
        }
    }

}
