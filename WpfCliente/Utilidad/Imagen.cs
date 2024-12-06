using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfCliente.Utilidad
{
    public static class Imagen
    {
        private const string RUTA_BASE_IMAGEN = "pack://application:,,,/WpfCliente;component/Recursos/pfp";
        private const string EXTENSION_IMAGEN_PNG = ".png";
        private const int LIMITE_TAMANO_ARCHIVO_5MB = 5 * 1024 * 1024;
        private const string FILTRO_ARCHIVOS_IMAGEN = "*.jpg;*.jpeg;*.png";
        private const int RANGO_MINIMO_IMAGENES = 1;
        private const int RANGO_MAXIMO_IMAGENES = 6;
        private const int BITS_POR_BYTE = 8;
        private const int DESPLAZAMIENTO = 0;
        public static byte[] fotoGlobal;

        public static BitmapImage ConvertirBytesABitmapImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            using (var stream = new MemoryStream(bytes))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }

        public static byte[] GuardarBitmapImageABytes(BitmapImage imagenControl)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(imagenControl));
                    encoder.Save(memoryStream);
                    fotoGlobal = memoryStream.ToArray();
                    return fotoGlobal;
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                return new byte[0];
            }
        }

        public static byte[] ObtenerFotoGlobal()
        {
            return fotoGlobal;
        }

        public static BitmapImage ConvertirStreamABitmapImagen(Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return null;

            BitmapImage bitmap = new BitmapImage();
            try
            {
                stream.Position = 0;
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                bitmap = null;
            }
            return bitmap;
        }

        [DebuggerStepThrough]
        public static bool EsImagenValida(BitmapImage bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException();
            }

            try
            {
                WriteableBitmap escritorBitmap = new WriteableBitmap(bitmap);
                int ancho = escritorBitmap.PixelWidth;
                int altura = escritorBitmap.PixelHeight;
                var pixel = new byte[ancho * altura * (escritorBitmap.Format.BitsPerPixel / BITS_POR_BYTE)];
                escritorBitmap.CopyPixels(pixel, ancho * (escritorBitmap.Format.BitsPerPixel / BITS_POR_BYTE), DESPLAZAMIENTO);
                return true;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                throw;
            }
        }

        public static bool EsImagenValida(string rutaImagen, Window ventana)
        {
            bool resultado = false;
            try
            {
                FileInfo fileInfo = new FileInfo(rutaImagen);
                if (fileInfo.Length > LIMITE_TAMANO_ARCHIVO_5MB)
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
            catch (FileNotFoundException excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeArchivoNoEncontrado, ventana);
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (UnauthorizedAccessException excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeAccesoDenegadoArchivo, ventana);
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (FileFormatException excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeArchivoInvalido, ventana);
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    excepcion.Message, ventana);
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            return resultado;
        }

        public static string ObtenerRutaImagenAleatoria()
        {
            Random random = new Random();
            int numeroImagen = random.Next(RANGO_MINIMO_IMAGENES, RANGO_MAXIMO_IMAGENES);
            return $"{RUTA_BASE_IMAGEN}{numeroImagen}{EXTENSION_IMAGEN_PNG}";
        }

        public static BitmapImage CargarImagenDesdeRecursos()
        {
            try
            {
                string uri = ObtenerRutaImagenAleatoria();
                return new BitmapImage(new Uri(uri));
            }
            catch (FileFormatException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (NotSupportedException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            return null;
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
            catch (FileNotFoundException excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeArchivoNoEncontrado, ventana);
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (UnauthorizedAccessException excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeAccesoDenegadoArchivo, ventana);
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (FileFormatException excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeArchivoInvalido, ventana);
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    excepcion.Message, ventana);
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            return resultado;
        }

        public static string SelecionarRutaImagen()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = Properties.Idioma.tituloSeleccionarImagen,
                Filter = $"{Properties.Idioma.mensajeDescripcionArchivosImagen} ({FILTRO_ARCHIVOS_IMAGEN})|{FILTRO_ARCHIVOS_IMAGEN}"
            };
            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }
    }
}
