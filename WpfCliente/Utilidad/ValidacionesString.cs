using System.Net.Mail;
using System;
using System.Text.RegularExpressions;

namespace WpfCliente.Utilidad
{
    public class ValidacionesString
    {
        private const string CARACTERES_VALIDOS = "^[a-zA-Z0-9_]+$";
        private const string EMAIL_VALIDO = "^(?=.{5,100}$)[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
        private const string SIMBOLOS_VALIDOS = "^(?=.*[\\W_])";
        private const string NOMBRE_INVITADO = "guest";
        private const string ESPACIO_CARACTER = " ";
        private const string PALABRA_RESERVADA_GUEST = "guest";


        public static bool EsGamertagValido(string gamertag)
        {
            bool validacionGamertag = false;

            if (string.IsNullOrWhiteSpace(gamertag))
            {
                return validacionGamertag;
            }
            if (gamertag.Contains(ESPACIO_CARACTER))
            {
                return validacionGamertag;
            }
            if (gamertag.Contains(PALABRA_RESERVADA_GUEST))
            {
                return validacionGamertag;
            }
            if (gamertag.IndexOf(PALABRA_RESERVADA_GUEST, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return validacionGamertag;
            }
            if (gamertag.Contains(NOMBRE_INVITADO))
            {
                return validacionGamertag;
            }
            if (gamertag.IndexOf(NOMBRE_INVITADO, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return validacionGamertag;
            }
            try
            {
                Regex regex = new Regex(CARACTERES_VALIDOS, RegexOptions.None, TimeSpan.FromMilliseconds(50));
                return regex.IsMatch(gamertag.Trim());
            }
            catch (RegexMatchTimeoutException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion) 
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            return validacionGamertag;

        }

        public static bool EsCorreoValido(string correo)
        {
            bool validacionCorreo = false;

            if (string.IsNullOrWhiteSpace(correo))
            {
                return validacionCorreo;
            }
            if (correo.Contains(" "))
            {
                return validacionCorreo;
            }
            try
            {
                MailAddress mail = new MailAddress(correo);
            }
            catch (FormatException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);

                return validacionCorreo;
            }
            catch(Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);

                return validacionCorreo;
            }

            try
            {
                Regex regex = new Regex(EMAIL_VALIDO, RegexOptions.None, TimeSpan.FromMilliseconds(50));
                return regex.IsMatch(correo.Trim());
            }
            catch (RegexMatchTimeoutException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            return validacionCorreo;

        }

        public static bool EsSimboloValido(string password)
        {
            try
            {
                Regex regex = new Regex(SIMBOLOS_VALIDOS, RegexOptions.None, TimeSpan.FromMilliseconds(50));

                return regex.IsMatch(password.Trim());
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
