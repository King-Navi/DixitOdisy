using System;
using Path = System.IO.Path;

namespace WpfCliente.Utilidad
{
    internal class Otros
    {
        public static string ConstruirAbsolutePath(string relativePath)
        {
            string absolutePath = "";

            if (relativePath != null)
            {
                absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            }

            return absolutePath;
        }
    }
}
