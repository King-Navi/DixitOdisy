using System.IO;
using System;
using System.Windows.Media.Imaging;
using WpfCliente.Utilidad;
using System.ComponentModel;

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

                    var imagen = new BitmapImage();
                    imagen.BeginInit();
                    if (FotoUsuario.CanSeek)
                        FotoUsuario.Position = 0;
                    imagen.StreamSource = FotoUsuario;
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
