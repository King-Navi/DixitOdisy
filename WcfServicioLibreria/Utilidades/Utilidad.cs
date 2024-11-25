using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WcfServicioLibreria.Utilidades
{
    public static class Utilidad
    {
        public static byte[] StreamABytes(Stream stream)
        {
            using (MemoryStream memoriaStream = new MemoryStream())
            {
                if (stream == null)
                {
                    return null;
                }
                stream.CopyTo(memoriaStream);
                return memoriaStream.ToArray();
            }
        }
        /// <summary>
        /// Genera un identificador único de 6 caracteres alfanuméricos para las salas.
        /// </summary>
        /// <returns>Un identificador de sala único.</returns>
        public static string GenerarIdUnico()
        {
            const string CARACTERES = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int LONGITUD_ID = 6;
            StringBuilder resultado = new StringBuilder(LONGITUD_ID);
            try
            {
                byte[] datosAleatorios = new byte[LONGITUD_ID];

                using (var generador = RandomNumberGenerator.Create())
                {
                    generador.GetBytes(datosAleatorios);
                }

                for (int i = 0; i < LONGITUD_ID; i++)
                {
                    int indice = datosAleatorios[i] % CARACTERES.Length;
                    resultado.Append(CARACTERES[indice]);
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarFatalException(excepcion);
            }
            return resultado.ToString();
        }
    }
}
