using System.Windows.Media.Imaging;

namespace WpfCliente.ServidorDescribelo
{
    public partial class Usuario
    {
        public BitmapImage BitmapImagen
        {
            get
            {
                if (FotoUsuario == null)
                    return null;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                if (FotoUsuario.CanSeek)
                    FotoUsuario.Position = 0;
                bitmap.StreamSource = FotoUsuario;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }
    }
}
