using System.IO;
using System;
using System.Windows.Media.Imaging;
using WpfCliente.Utilidad;

namespace WpfCliente.ServidorDescribelo
{
    public partial class Usuario
    {
        public BitmapImage BitmapImagen
        {
            get
            {
                try
                {
                    if (FotoUsuario == null)
                        return Imagen.CargarImagenDesdeRecursos();

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    if (FotoUsuario.CanSeek)
                        FotoUsuario.Position = 0;
                    bitmap.StreamSource = FotoUsuario;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    Imagen.EsImagenValida(bitmap);
                    return bitmap;
                }
                catch (FileFormatException excepcion)
                {
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                }
                catch (NotSupportedException excepcion)
                {
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                }
                return Imagen.CargarImagenDesdeRecursos();
            }
        }
    }
}
