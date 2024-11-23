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

        public static bool EsImagenValida(string rutaImagen, Window window)
        {
            bool resultado = false;
            try
            {
                FileInfo fileInfo = new FileInfo(rutaImagen);
                if (fileInfo.Length > 5 * 1024 * 1024)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLimiteImagenSuperado,
                        Properties.Idioma.mensajeLimiteImagenSuperado, window);
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
                    Properties.Idioma.mensajeArchivoNoEncontrado, window);
            }
            catch (UnauthorizedAccessException)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeAccesoDenegadoArchivo, window);
            }
            catch (FileFormatException)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeArchivoInvalido, window);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeErrorInesperado + ex.Message, window);
            }
            return resultado;
        }
        public static bool EsImagenValida(Stream image, Window window)
        {
            bool resultado = false;
            try
            {
                BitmapImage bitmap = new BitmapImage();
                
                    bitmap.BeginInit();
                    bitmap.StreamSource = image;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                
                resultado = true;
            }
            catch (FileNotFoundException)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeArchivoNoEncontrado, window);
            }
            catch (UnauthorizedAccessException)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeAccesoDenegadoArchivo, window);
            }
            catch (FileFormatException)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeArchivoInvalido, window);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeErrorInesperado + ex.Message, window);
            }
            return resultado;
        }

        public static string SelecionarRutaImagen()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Seleccionar una imagen",
                Filter = "Archivos de imagen (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png"
            };

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }
    }
}
