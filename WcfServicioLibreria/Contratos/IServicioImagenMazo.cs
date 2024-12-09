using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IImagenMazoCallback))]
    public interface IServicioImagenMazo
    {
        /// <summary>
        /// Solicita una o más imágenes del mazo asociadas a una partida específica.
        /// </summary>
        /// <param name="idPartida">Identificador único de la partida.</param>
        /// <param name="numeroImagenes">Número de imágenes a solicitar. Valor predeterminado: 1.</param>
        /// <returns>Una tarea que indica si la solicitud fue exitosa.</returns>
        [OperationContract]
        Task<bool> SolicitarImagenMazoAsync(string idPartida, int numeroImagenes = 1);
    }

    [ServiceContract]
    public interface IImagenMazoCallback
    {
        /// <summary>
        /// Recibe una imagen específica como respuesta a una solicitud de imágenes.
        /// </summary>
        /// <param name="imagen">Objeto que representa la imagen de la carta.</param>
        [OperationContract(IsOneWay = true)]
        void RecibirImagenCallback(ImagenCarta imagen);
        /// <summary>
        /// Recibe una lista de imágenes como respuesta a una solicitud de múltiples imágenes.
        /// </summary>
        /// <param name="imagenes">Lista de objetos que representan las imágenes de las cartas.</param>
        [OperationContract(IsOneWay = true)]
        void RecibirVariasImagenCallback(List<ImagenCarta> imagenes);
    }
}
