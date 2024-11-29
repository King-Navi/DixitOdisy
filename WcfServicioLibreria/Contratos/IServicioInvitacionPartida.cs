using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IUsuarioSesionCallback))]
    public interface IServicioInvitacionPartida
    {
        /// <summary>
        /// Envia una invitación de partida a un usuario receptor.
        /// </summary>
        /// <param name="usuarioEmisor">El identificador del usuario que emite la invitación.</param>
        /// <param name="codigoSala">El código de la sala a la que se invita al receptor.</param>
        /// <param name="usuarioReceptor">El identificador del usuario que recibe la invitación.</param>
        /// <returns>True si la invitación fue enviada con éxito; False en caso contrario.</returns>
        [OperationContract]
        bool EnviarInvitacion(InvitacionPartida invitacion);
    }

    public partial interface IUsuarioSesionCallback
    {
        /// <summary>
        /// Recibe una invitación a una partida. Este método es unidireccional y no devuelve ninguna respuesta al emisor.
        /// </summary>
        /// <param name="invitacion">El objeto invitación que contiene detalles de la partida a la que se está invitando al usuario.</param>
        [OperationContract(IsOneWay = true)]
        void RecibirInvitacionCallback(InvitacionPartida invitacion);
    }
}
