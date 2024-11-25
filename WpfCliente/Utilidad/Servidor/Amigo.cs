

using System.IO;
using System;
using System.Windows.Media.Imaging;
using WpfCliente.Utilidad;

namespace WpfCliente.ServidorDescribelo
{
    public  partial class Amigo
    {
        public string EstadoActual { get; set; }
        private BitmapImage bitmapImagen;
        public BitmapImage BitmapImagen
        {
            get
            {
                try
                {
                    if (bitmapImagen == null && Foto != null)
                    {
                        bitmapImagen = new BitmapImage();
                        bitmapImagen.BeginInit();
                        if (Foto.CanSeek)
                            Foto.Position = 0;
                        bitmapImagen.StreamSource = Foto;
                        bitmapImagen.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImagen.EndInit();
                        bitmapImagen.Freeze();
                    }
                    Imagen.EsImagenValida(bitmapImagen);
                    return bitmapImagen;
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
            set
            {
                bitmapImagen = value;
            }
        }
    }
}
