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



        public static bool EsGamertagValido(string gamertag)
        {
            Regex regex = new Regex(CARACTERES_VALIDOS);
            bool validacionGamertag = true;

            if (string.IsNullOrWhiteSpace(gamertag))
            {
                validacionGamertag = false;
                return validacionGamertag;
            }
            if (gamertag.Contains(ESPACIO_CARACTER))
            {
                validacionGamertag = false;
            }
            if (gamertag.Contains(NOMBRE_INVITADO))
            {
                validacionGamertag = false;
            }
            if (gamertag.IndexOf(NOMBRE_INVITADO, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                validacionGamertag = false;
            }

            return regex.IsMatch(gamertag.Trim()) && validacionGamertag;
        }

        public static bool EsCorreoValido(string correo)
        {
            Regex regex = new Regex(EMAIL_VALIDO);
            bool validacionCorreo = true;
            try
            {
                MailAddress mail = new MailAddress(correo);
            }
            catch (FormatException)
            {
                validacionCorreo = false;
            }
            catch(Exception)
            {
                validacionCorreo = false;
            }

            if (string.IsNullOrWhiteSpace(correo))
            {
                validacionCorreo = false;
                return validacionCorreo;
            }
            if(correo.Contains(" "))
            {
                validacionCorreo = false;
                return validacionCorreo;
            }

            return regex.IsMatch(correo.Trim()) && validacionCorreo;
        }

        public static bool EsSimboloValido(string password)
        {
            Regex regex = new Regex(SIMBOLOS_VALIDOS);

            return regex.IsMatch(password.Trim());
        }
    }
}
