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
        public const int RANGO_INVITADO_MINIMA = 10000;
        public const int RANGO_INVITADO_MAXIMO = 99999;
        private static Random aleatorio = new Random();

        private static readonly List<Color> ColoresFondo = new List<Color>
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
            Color colorAleatorio = ColoresFondo[aleatorio.Next(ColoresFondo.Count)];
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
            int numeroAleatorio = aleatorio.Next(RANGO_INVITADO_MINIMA, RANGO_INVITADO_MAXIMO);
            return $"guest-{numeroAleatorio}";
        }
    }
}
