using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IImagenCallback))]
    public interface IServicioImagen
    {
        /// <summary>
        /// Solicita una imagen para la carta de un jugador.
        /// </summary>
        /// <param name="idPartida">El identificador único de la partida.</param>
        /// <returns>
        /// Un <see cref="Task{bool}"/> que indica si la imagen se envió con éxito.
        /// Devuelve <c>true</c> si se envió correctamente, de lo contrario <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Este método valida que la partida sea válida antes de enviar la imagen.
        /// </remarks>
        [OperationContract]
        Task<bool> SolicitarImagenCartaAsync(string idPartida);
        [OperationContract]
        bool MostrarTodasImagenes(string idPartida);
    }
    [ServiceContract]
    public interface IImagenCallback
    {
        [OperationContract(IsOneWay = true)]
        void RecibirImagenCallback(ImagenCarta imagen);

        [OperationContract(IsOneWay = true)]
        void RecibirGrupoImagenCallback(ImagenCarta imagen);
    }
}
