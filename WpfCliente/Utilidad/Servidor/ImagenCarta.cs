using System.IO;
using System.Windows.Media.Imaging;

namespace WpfCliente.ServidorDescribelo
{
    public partial class ImagenCarta
    {
        // Propiedad que convierte MemoryStream a BitmapImage solo una vez y lo reutiliza
        private BitmapImage _bitmapImagen;
        // Propiedad para convertir el MemoryStream en BitmapImage
        public BitmapImage BitmapImagen
        {
            get
            {
                if (ImagenStream == null)
                    return null;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(ImagenStream.ToArray()); // Convertimos el stream para asegurarnos de que está en la posición correcta
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze(); // Para mejorar el rendimiento en WPF
                return bitmap;
            }
        }
    }
}

