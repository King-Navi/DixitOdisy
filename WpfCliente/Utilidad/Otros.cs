using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
