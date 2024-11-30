using System;
using System.IO;
using System.Windows.Media.Imaging;
using WpfCliente.Utilidad;

namespace WpfCliente.ServidorDescribelo
{
    public partial class ImagenCarta
    {
        public BitmapImage BitmapImagen
        {
            get
            {
                try
                {
                    if (ImagenStream == null)
                        return Imagen.CargarImagenDesdeRecursos();

                    var imagen = new BitmapImage();
                    imagen.BeginInit();
                    imagen.StreamSource = new MemoryStream(ImagenStream.ToArray());
                    imagen.CacheOption = BitmapCacheOption.OnLoad;
                    imagen.EndInit();
                    imagen.Freeze();
                    Imagen.EsImagenValida(imagen);
                    return imagen;
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

