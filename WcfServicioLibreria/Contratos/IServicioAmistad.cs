using System.Collections.Generic;
using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IAmistadCallBack))]
    public interface IServicioAmistad
    {
        /// <summary>
        /// Abre un canal para recibir peticiones de amistad.
        /// </summary>
        /// <param name="usuario">El usuario que abre el canal.</param>
        [OperationContract(IsOneWay = true)]
        void AbrirCanalParaPeticiones(Usuario usuario);
        /// <summary>
        /// Envia una solicitud de amistad a otro usuario.
        /// </summary>
        /// <param name="usuarioRemitente">El usuario que envía la solicitud.</param>
        /// <param name="destinatario">El identificador del usuario destinatario de la solicitud.</param>
        /// <returns>True si la solicitud fue enviada con éxito, False en caso contrario.</returns>
        /// <exception cref="SolicitudAmistadFalla">Se lanza si hay un error al enviar la solicitud.</exception>
        [OperationContract]
        [FaultContract(typeof(SolicitudAmistadFalla))]
        bool EnviarSolicitudAmistad (Usuario usuarioRemitente, string destinatario);
        /// <summary>
        /// Obtiene la lista de solicitudes de amistad pendientes de un usuario.
        /// </summary>
        /// <param name="usuario">El usuario cuyas solicitudes se desean obtener.</param>
        /// <returns>Una lista de solicitudes de amistad pendientes.</returns>
        [OperationContract]
        List<SolicitudAmistad> ObtenerSolicitudesAmistad (Usuario usuario);
        /// <summary>
        /// Acepta una solicitud de amistad.
        /// </summary>
        /// <param name="idRemitente">El identificador del usuario que envió la solicitud.</param>
        /// <param name="idDestinatario">El identificador del usuario que recibe la solicitud.</param>
        /// <returns>True si la solicitud fue aceptada con éxito, False en caso contrario.</returns>
        [OperationContract]
        bool AceptarSolicitudAmistad(int idRemitente, int idDestinatario);
        /// <summary>
        /// Rechaza una solicitud de amistad.
        /// </summary>
        /// <param name="idRemitente">El identificador del usuario que envió la solicitud.</param>
        /// <param name="idDestinatario">El identificador del usuario que recibe la solicitud.</param>
        /// <returns>True si la solicitud fue rechazada con éxito, False en caso contrario.</returns>
        [OperationContract]
        bool RechazarSolicitudAmistad(int idRemitente, int idDestinatario);
        /// <summary>
        /// Determina si dos usuarios son amigos.
        /// </summary>
        /// <param name="usuarioRemitente">El identificador de un usuario.</param>
        /// <param name="destinatario">El identificador del otro usuario.</param>
        /// <returns>True si son amigos, False en caso contrario.</returns>
        [OperationContract]
        bool SonAmigos(string usuarioRemitente, string destinatario);
    }
    [ServiceContract]
    public interface IAmistadCallBack
    {
        /// <summary>
        /// Notifica al cliente sobre un cambio en el estado de un amigo.
        /// </summary>
        /// <param name="amigo">El amigo cuyo estado ha cambiado.</param>
        [OperationContract]
        void CambiarEstadoAmigo(Amigo amigo);
        /// <summary>
        /// Recibe información actualizada de un amigo como parte de un callback.
        /// </summary>
        /// <param name="amigo">El amigo cuya información se está actualizando.</param>
        [OperationContract]
        void ObtenerAmigoCallback(Amigo amigo);
    }
}
