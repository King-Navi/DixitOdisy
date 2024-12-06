using System.ServiceModel;
using System;
using WcfServicioLibreria.Modelo;
using System.Collections.Concurrent;
using WcfServicioLibreria.Contratos;

namespace Pruebas.Servidor.Utilidades
{
    internal class ImagenCallbackImplementacion : CommunicationObjectImplementado, IImagenCallback
    {
        public ConcurrentDictionary<string, ImagenCarta> ImagenCartasMazo { get; set; } = new ConcurrentDictionary<string, ImagenCarta>();
        public ConcurrentDictionary<string, ImagenCarta> ImagenCartasTodos { get; set; } = new ConcurrentDictionary<string, ImagenCarta>();
        public ImagenCallbackImplementacion() { }


        public void RecibirGrupoImagenCallback(ImagenCarta imagen)
        {
            if (imagen == null)
            {
                Console.WriteLine("RecibirGrupoImagenCallback: Imagen es nula, operación ignorada.");
                return;
            }

            if (ImagenCartasTodos.TryAdd(imagen.IdImagen, imagen))
            {
                Console.WriteLine($"Imagen {imagen.IdImagen} añadida al grupo.");
            }
            else
            {
                Console.WriteLine($"RecibirGrupoImagenCallback: No se pudo añadir la imagen {imagen.IdImagen}.");
            }
        }

        public void RecibirImagenCallback(ImagenCarta imagen)
        {
            if (imagen == null)
            {
                Console.WriteLine("RecibirImagenCallback: Imagen es nula, operación ignorada.");
                return;
            }

            if (ImagenCartasMazo.TryAdd(imagen.IdImagen, imagen))
            {
                Console.WriteLine($"Imagen {imagen.IdImagen} añadida al mazo.");
            }
            else
            {
                Console.WriteLine($"RecibirImagenCallback: No se pudo añadir la imagen {imagen.IdImagen}.");
            }
        }
    }
}

