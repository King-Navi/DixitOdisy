using System;
using System.Security.Cryptography;
using System.Text;

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
