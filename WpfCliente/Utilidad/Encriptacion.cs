using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfCliente.Utilidad
{
    public static class Encriptacion
    {
        internal static string OcuparSHA256(string entrada)
        {
            return BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(entrada))).Replace("-", "");
        }
    }
}
