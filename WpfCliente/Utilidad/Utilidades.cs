using System;
using System.Collections.Generic;
using System.Windows.Media;
using Path = System.IO.Path;

namespace WpfCliente.Utilidad
{
    internal class Utilidades
    {
        public const double OPACIDAD_MAXIMA = 1;
        public const double OPACIDAD_MINIMA = 0.5;
        private static Random random = new Random();

        private static readonly List<Color> BackgroundColors = new List<Color>
        {
            Color.FromRgb(245, 245, 220), 
            Color.FromRgb(255, 239, 219),
            Color.FromRgb(211, 211, 211),
            Color.FromRgb(255, 248, 220),
            Color.FromRgb(240, 230, 140), 
            Color.FromRgb(255, 182, 193), 
            Color.FromRgb(176, 224, 230), 
            Color.FromRgb(255, 222, 173), 
            Color.FromRgb(221, 160, 221), 
            Color.FromRgb(255, 250, 205)  
        };

        public static SolidColorBrush ObtenerColorAleatorio()
        {
            Color colorAleatorio = BackgroundColors[random.Next(BackgroundColors.Count)];
            return new SolidColorBrush(colorAleatorio);
        }

        public static string ConstruirRutaAbsoluta(string rutaRelativa)
        {
            string rutaAbsoluta = "";

            if (rutaRelativa != null)
            {
                rutaAbsoluta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rutaRelativa);
            }

            return rutaAbsoluta;
        }

        public static string GenerarGamertagInvitado()
        {
            int numeroAleatorio = random.Next(10000, 99999);
            return $"guest-{numeroAleatorio}";
        }
    }
}
