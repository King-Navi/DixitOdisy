using Microsoft.Win32;
using System;
using System.IO;
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

        internal static bool EsImagenValida(string rutaImagen)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                using (FileStream stream = new FileStream(rutaImagen, FileMode.Open, FileAccess.Read))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }
                return true;
            }
            catch
            {
                return false;
            };
        }
        internal static bool EsImagenValida(Stream image)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                
                    bitmap.BeginInit();
                    bitmap.StreamSource = image;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                
                return true;
            }
            catch
            {
                return false;
            };
        }

        internal static string SelecionarRutaImagen()
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
