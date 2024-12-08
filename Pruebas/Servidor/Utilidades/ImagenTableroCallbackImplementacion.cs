using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor.Utilidades
{
    internal class ImagenTableroCallbackImplementacion : CommunicationObjectImplementado, IImagenesTableroCallback
    {
        public ConcurrentDictionary<string, ImagenCarta> ImagenTablero { get; set; } = new ConcurrentDictionary<string, ImagenCarta>();

        public void RecibirGrupoImagenCallback(List<ImagenCarta> imagenes)
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
                ImagenTablero.AddOrUpdate(
                    imagen.IdImagen,
                    imagen,
                    (key, existingValue) => imagen
                );
            }
        }
    }
}
