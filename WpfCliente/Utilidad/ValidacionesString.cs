using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfCliente.Utilidad
{
    internal class ValidacionesString
    {
        private const string GAMERTAG_VALIDO = "^[a-zA-Z0-9_]+$";
        private const string EMAIL_VALIDO = "^(?=.{5,100}$)[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
        private const string SIMBOLOS_VALIDOS = "^(?=.*[\\W_])";



        public static bool IsValidGamertag(string password)
        {
            Regex regex = new Regex(GAMERTAG_VALIDO);

            return regex.IsMatch(password.Trim());
        }

        public static bool IsValidEmail(string email)
        {
            Regex regex = new Regex(EMAIL_VALIDO);

            return regex.IsMatch(email.Trim());
        }

        public static bool IsValidSymbol(string password)
        {
            Regex regex = new Regex(SIMBOLOS_VALIDOS);

            return regex.IsMatch(password.Trim());
        }
    }
}
