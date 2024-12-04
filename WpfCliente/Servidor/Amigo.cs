

using System.IO;
using System;
using System.Windows.Media.Imaging;
using WpfCliente.Utilidad;
using System.ComponentModel;

namespace WpfCliente.ServidorDescribelo
{
    public  partial class Amigo : INotifyPropertyChanged
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
                        using (var memoryStream = new MemoryStream(Foto))
                        {
                            bitmapImagen = new BitmapImage();
                            bitmapImagen.BeginInit();
                            bitmapImagen.StreamSource = memoryStream;
                            bitmapImagen.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImagen.EndInit();
                            bitmapImagen.Freeze();
                        }
                        
                    }
                    Imagen.EsImagenValida(bitmapImagen);
                    return bitmapImagen;
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
            set
            {
                bitmapImagen = value;
            }
        }
    }
}
