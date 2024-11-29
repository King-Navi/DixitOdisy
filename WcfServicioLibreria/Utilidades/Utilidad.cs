using System;
using System.IO;
using System.Reflection;
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

        public static string Generar6Caracteres()
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

        /// <summary>
        /// Evalúa si todas las propiedades públicas de un objeto tienen un valor válido.
        /// </summary>
        /// <param name="objeto">El objeto a evaluar.</param>
        /// <returns>True si todas las propiedades tienen valores válidos; False en caso contrario.</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el objeto es null.</exception>
        public static bool ValidarPropiedades(object objeto)
        {
            if (objeto == null)
            {
                throw new ArgumentNullException(nameof(objeto), "El objeto no puede ser null.");
            }
            var propiedades = objeto.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var propiedad in propiedades)
            {
                var valor = propiedad.GetValue(objeto);
                if (valor == null)
                {
                    return false;
                }
                if (propiedad.PropertyType == typeof(string) && string.IsNullOrWhiteSpace((string)valor))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
