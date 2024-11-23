using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfCliente.Utilidad
{
    public static class Imagen
    {
        public static BitmapImage ConvertirStreamABitmapImagen(Stream stream)
        {
            BitmapImage bitmap = new BitmapImage();
            try
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponentFatalException(ex);
                bitmap = null;
            }
            return bitmap;
        }

        public static void GuardarImagenComoJpg(Stream stream, string rutaArchivo)
        {
            using (var fileStream = new FileStream(rutaArchivo, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
        }

        public static MemoryStream ConvertirBitmapImageAMemoryStream(BitmapImage imageControl)
        {
            if (imageControl == null)
                return null;  
            MemoryStream memoryStream = new MemoryStream();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageControl));
            encoder.Save(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        public static bool EsImagenValida(string rutaImagen, Window ventana)
        {
            bool resultado = false;
            try
            {
                FileInfo fileInfo = new FileInfo(rutaImagen);
                if (fileInfo.Length > 5 * 1024 * 1024)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLimiteImagenSuperado,
                        Properties.Idioma.mensajeLimiteImagenSuperado, ventana);
                }
                else
                {
                    BitmapImage bitmap = new BitmapImage();
                    using (FileStream stream = new FileStream(rutaImagen, FileMode.Open, FileAccess.Read))
                    {
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                    }
                    resultado = true;
                }
            }
            catch (FileNotFoundException)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeArchivoNoEncontrado, ventana);
            }
            catch (UnauthorizedAccessException)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeAccesoDenegadoArchivo, ventana);
            }
            catch (FileFormatException)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeArchivoInvalido, ventana);
            }
            catch (Exception excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeErrorInesperado + excepcion.Message, ventana);
            }
            return resultado;
        }

        public static bool EsImagenValida(Stream imagen, Window ventana)
        {
            bool resultado = false;
            try
            {
                BitmapImage bitmap = new BitmapImage();
                
                    bitmap.BeginInit();
                    bitmap.StreamSource = imagen;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                
                resultado = true;
            }
            catch (FileNotFoundException)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeArchivoNoEncontrado, ventana);
            }
            catch (UnauthorizedAccessException)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeAccesoDenegadoArchivo, ventana);
            }
            catch (FileFormatException)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeArchivoInvalido, ventana);
            }
            catch (Exception excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeErrorInesperado + excepcion.Message, ventana);
            }
            return resultado;
        }

        public static string SelecionarRutaImagen()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = Properties.Idioma.tituloSeleccionarImagen, 
                Filter = string.Format("{0} (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png", Properties.Idioma.mensajeDescripcionArchivosImagen)
            };
            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }
    }
}
