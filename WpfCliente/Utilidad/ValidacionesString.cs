using System.Net.Mail;
using System;
using System.Text.RegularExpressions;

namespace WpfCliente.Utilidad
{
    internal class ValidacionesString
    {
        private const string GAMERTAG_VALIDO = "^[a-zA-Z0-9_]+$";
        private const string EMAIL_VALIDO = "^(?=.{5,100}$)[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
        private const string SIMBOLOS_VALIDOS = "^(?=.*[\\W_])";



        public static bool EsGamertagValido(string gamertag)
        {
            Regex regex = new Regex(GAMERTAG_VALIDO);
            bool validacionGamertag = true;

            if (string.IsNullOrWhiteSpace(gamertag))
            {
                validacionGamertag = false;
            }
            if (gamertag.Contains(" "))
            {
                validacionGamertag = false;
            }
            if (gamertag.Contains("guest"))
            {
                validacionGamertag = false;
            }
            if (gamertag.IndexOf("guest", StringComparison.OrdinalIgnoreCase) >= 0)
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
            }catch(Exception)
            {
                validacionCorreo = false;
            }

            if (string.IsNullOrWhiteSpace(correo))
            {
                validacionCorreo = false;
            }
            if(correo.Contains(" "))
            {
                validacionCorreo = false;
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
