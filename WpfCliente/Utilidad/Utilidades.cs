using System;
using System.Collections.Generic;
using System.Windows.Media;
using Path = System.IO.Path;

namespace WpfCliente.Utilidad
{
    internal class Utilidades
    {
        private static readonly List<Color> BackgroundColors = new List<Color>
        {
            Color.FromRgb(245, 245, 220), // Beige
            Color.FromRgb(255, 239, 219), // Hueso
            Color.FromRgb(211, 211, 211), // Gris Claro
            Color.FromRgb(255, 248, 220), // Maíz
            Color.FromRgb(240, 230, 140),  // Kaki Claro
            Color.FromRgb(255, 182, 193), // Rosa Pastel
            Color.FromRgb(176, 224, 230), // Azul Pastel
            Color.FromRgb(255, 222, 173), // Durazno Pastel
            Color.FromRgb(221, 160, 221), // Lavanda
            Color.FromRgb(255, 250, 205)  // Limón Pastel
        };

        public static SolidColorBrush GetColorAleatorio()
        {
            Random random = new Random();
            Color colorAleatorio = BackgroundColors[random.Next(BackgroundColors.Count)];
            return new SolidColorBrush(colorAleatorio);
        }

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
