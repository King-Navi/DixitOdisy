using System.ServiceModel;
using System;
using WcfServicioLibreria.Modelo;
using System.Collections.Concurrent;
using WcfServicioLibreria.Contratos;
using System.Collections.Generic;

namespace Pruebas.Servidor.Utilidades
{
    internal class ImagenMazoCallbackImplementacion : CommunicationObjectImplementado, IImagenMazoCallback
    {
        public ConcurrentDictionary<string, ImagenCarta> ImagenCartasMazo { get; set; } = new ConcurrentDictionary<string, ImagenCarta>();
        public ImagenMazoCallbackImplementacion() { }

        public void RecibirImagenCallback(ImagenCarta imagen)
        {
            if (imagen == null || string.IsNullOrEmpty(imagen.IdImagen))
            {
                throw new ArgumentNullException(nameof(imagen), "La imagen o su identificador no puede ser nulo.");
            }
            ImagenCartasMazo.AddOrUpdate(
                imagen.IdImagen,
                imagen, 
                (key, existingValue) => imagen 
            );
        }

        public void RecibirVariasImagenCallback(List<ImagenCarta> imagenes)
        {
            if (imagenes == null || imagenes.Count == 0)
            {
                throw new ArgumentNullException(nameof(imagenes), "La lista de imágenes no puede ser nula o vacía.");
            }

            foreach (var imagen in imagenes)
            {
                if (imagen == null || string.IsNullOrEmpty(imagen.IdImagen))
                {
                    throw new ArgumentException("La imagen o su identificador no puede ser nulo.", nameof(imagenes));
                }
                ImagenCartasMazo.AddOrUpdate(
                    imagen.IdImagen,
                    imagen, 
                    (key, existingValue) => imagen 
                );
            }
        }

    }
}

