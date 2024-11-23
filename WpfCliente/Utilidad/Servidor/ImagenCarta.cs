using System.IO;
using System.Windows.Media.Imaging;

namespace WpfCliente.ServidorDescribelo
{
    public partial class ImagenCarta
    {
        private BitmapImage _bitmapImagen;
        public BitmapImage BitmapImagen
        {
            get
            {
                if (ImagenStream == null)
                    return null;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(ImagenStream.ToArray());
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }
    }
}

