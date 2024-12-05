using System.Net.Mail;
using System;
using System.Text.RegularExpressions;

namespace WpfCliente.Utilidad
{
    public static class ValidacionesString
    {
        public const string CARACTERES_VALIDOS = "^[a-zA-Z0-9_]+$";
        public const string EMAIL_VALIDO = "^(?=.{5,100}$)[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
        public const string SIMBOLOS_VALIDOS = "^(?=.*[\\W_])";
        public const string NOMBRE_INVITADO = "guest";
        public const string ESPACIO_CARACTER = " ";
        public const string PALABRA_RESERVADA_GUEST = "guest";
        public const int TIEMPO_MILISEGUNDOS = 50;
        public const int LONGITUD_MAXIMA_GAMERTAG = 20;
        public const int CERO = 0;



        public static bool EsGamertagValido(string gamertag)
        {
            bool validacionGamertag = false;

            if (string.IsNullOrWhiteSpace(gamertag))
            {
                return validacionGamertag;
            }
            if (gamertag.Length > LONGITUD_MAXIMA_GAMERTAG)
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
            if (gamertag.IndexOf(PALABRA_RESERVADA_GUEST, StringComparison.OrdinalIgnoreCase) >= CERO)
            {
                return validacionGamertag;
            }
            if (gamertag.Contains(NOMBRE_INVITADO))
            {
                return validacionGamertag;
            }
            if (gamertag.IndexOf(NOMBRE_INVITADO, StringComparison.OrdinalIgnoreCase) >= CERO)
            {
                return validacionGamertag;
            }
            try
            {
                Regex regex = new Regex(CARACTERES_VALIDOS, RegexOptions.None, TimeSpan.FromMilliseconds(TIEMPO_MILISEGUNDOS));
                return regex.IsMatch(gamertag.Trim());
            }
            catch (RegexMatchTimeoutException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion) 
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
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
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);

                return validacionCorreo;
            }
            catch(Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);

                return validacionCorreo;
            }

            try
            {
                Regex regex = new Regex(EMAIL_VALIDO, RegexOptions.None, TimeSpan.FromMilliseconds(TIEMPO_MILISEGUNDOS));
                return regex.IsMatch(correo.Trim());
            }
            catch (RegexMatchTimeoutException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            return validacionCorreo;

        }

        public static bool EsSimboloValido(string password)
        {
            try
            {
                Regex regex = new Regex(SIMBOLOS_VALIDOS, RegexOptions.None, TimeSpan.FromMilliseconds(TIEMPO_MILISEGUNDOS));

                return regex.IsMatch(password.Trim());
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
