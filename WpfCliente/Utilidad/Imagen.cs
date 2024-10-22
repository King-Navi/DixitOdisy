using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfCliente.Utilidad
{
    public static class Imagen
    {
        public static BitmapImage ConvertirStreamABitmapImagen(Stream stream)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze(); 
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
    }
}
