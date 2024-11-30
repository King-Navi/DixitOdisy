using System;
using System.IO;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor.Utilidades
{
    public static class GeneradorAleatorio
    {
        public static MemoryStream GenerarStreamAleatorio(int longitud)
        {
            byte[] entrada = new byte[longitud];
            Random aleatorio = new Random();
            aleatorio.NextBytes(entrada);

            return new MemoryStream(entrada);
        }
        public static Usuario GenerarUsuarioAleatorio()
        {
            string nombre = "JugadorPrueba" + new Random().Next(1000, 9999);
            return new Usuario {
                Nombre = nombre,
                ContraseniaHASH = DAO.Utilidad.ObtenerSHA256Hash("Contraseña" + new Random().Next(1000, 9999)),
                Correo = $"{nombre.ToLower()}@gmail.com",
                FotoUsuario = GeneradorAleatorio.GenerarStreamAleatorio(20)
            };
        }
        public static int GenerarIdValido()
        {
            return new Random().Next(1, 9999);
        }
    }
}
