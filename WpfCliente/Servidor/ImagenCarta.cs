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
                    {
                        return Imagen.CargarImagenDesdeRecursos();
                    }

                    var imagen = new BitmapImage();
                    imagen.BeginInit();
                    imagen.StreamSource = new MemoryStream(ImagenStream);
                    imagen.CacheOption = BitmapCacheOption.OnLoad;
                    imagen.EndInit();
                    imagen.Freeze();
                    Imagen.EsImagenValida(imagen);
                    return imagen;
                }
                catch (FileFormatException excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                }
                catch (NotSupportedException excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                }
                return Imagen.CargarImagenDesdeRecursos();
            }
        }
    }
}

