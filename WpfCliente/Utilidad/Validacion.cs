using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtilidadesLibreria;
using WpfCliente.GUI;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;
namespace WpfCliente
{
    public class Validacion
    {
        /// <summary>
        /// Devuelve un mensaje y booleano de exito si la contrasenia cumple las politicas
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Object> ValidarContrasenia(String contrasenia)
        {
            Dictionary<string, Object > resultado = new Dictionary<string, Object>();
            if (String.IsNullOrEmpty(contrasenia))
            {
                resultado.Add(Llaves.LLAVE_BOOLEANO, false);
                //TODO Unaay debe colcoar una internalizacion para este mensaje
                resultado.Add(Llaves.LLAVE_MENSAJE, "Es vacio");

                return resultado;
            }
            if (contrasenia.Length < 5 || contrasenia.Length > 20)
            {
                resultado.Add(Llaves.LLAVE_BOOLEANO, false);
                //TODO Unaay debe colcoar una internalizacion para este mensaje
                resultado.Add(Llaves.LLAVE_MENSAJE, "La contraseña debe tener entre 5 y 20 caracteres.");
                return resultado;
            }
            string caracteresPermitidos = @"^[a-zA-Z0-9#\$%\&]+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(contrasenia, caracteresPermitidos))
            {
                resultado.Add(Llaves.LLAVE_BOOLEANO, false);
                //TODO Unaay debe colcoar una internalizacion para este mensaje
                resultado.Add(Llaves.LLAVE_MENSAJE, "La contraseña contiene caracteres no permitidos.");
                return resultado;
            }
            resultado.Add(Llaves.LLAVE_BOOLEANO, true);
            //TODO Unaay debe colcoar una internalizacion para este mensaje
            resultado.Add(Llaves.LLAVE_MENSAJE, "La contraseña es válida.");
            return resultado;
        }

        public static bool ExisteSala(string codigoSala)
        {
            IServicioSala servicioSala = new ServicioSalaClient();
            return servicioSala.ValidarSala(codigoSala);
        }

        public static async Task<bool> ValidarConexion()
        {
            bool resultado = false;
            try
            {
                ServidorDescribelo.IServicioUsuario ping = new ServicioUsuarioClient();
                resultado = await ping.PingAsync();
            }
            catch (Exception excepcion)
            {
                //TODO: Manejar excepcion
                return resultado;
            }
            return resultado;
        }
      
    }
}
