using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Modelo.Excepciones;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IAmistadCallBack))]
    public interface IServicioAmistad
    {
        /// <summary>
        /// Abre un canal para recibir peticiones de amistad.
        /// </summary>
        /// <param name="usuario">El usuario que abre el canal.</param>
        [OperationContract]
        [ServiceKnownType(typeof(MemoryStream))]
        [ServiceKnownType(typeof(Stream))]
        [ServiceKnownType(typeof(Amigo))]
        bool AbrirCanalParaAmigos(Usuario usuario);
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
        /// <exception cref="ServidorFalla">Se lanza si tiene veto.</exception>
        [OperationContract]
        [FaultContract(typeof(ServidorFalla))]
        [ServiceKnownType(typeof(MemoryStream))]
        [ServiceKnownType(typeof(Stream))]
        [ServiceKnownType(typeof(Amigo))]
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
        [ServiceKnownType(typeof(MemoryStream))]
        [ServiceKnownType(typeof(Stream))]
        [ServiceKnownType(typeof(Amigo))]
        bool SonAmigos(string usuarioRemitente, string destinatario);
        /// <summary>
        /// Elimina un amigo de la lista de contactos del usuario remitente.
        /// </summary>
        /// <param name="usuarioRemitente">El identificador o nombre del usuario que solicita la eliminación de un amigo.</param>
        /// <param name="destinatario">El identificador o nombre del amigo a eliminar.</param>
        /// <returns>
        /// True si el amigo fue eliminado exitosamente; False en caso contrario, incluyendo situaciones donde el amigo no existe
        /// en la lista del usuario remitente o si hay un problema de acceso a la base de datos.
        /// </returns>
        /// <remarks>
        /// Este método procesa la solicitud de eliminación verificando primero que el destinatario exista en la lista de amigos
        /// del remitente. Posteriormente, procede con la eliminación y retorna un valor booleano para indicar el éxito o fallo de la operación.
        /// Es importante manejar adecuadamente los errores durante la ejecución para evitar inconsistencias en los datos de usuario.
        /// </remarks>
        /// <exception cref="ServidorFalla">Se lanza si tiene veto.</exception>
        [OperationContract]
        [FaultContract(typeof(ServidorFalla))]
        [ServiceKnownType(typeof(MemoryStream))]
        [ServiceKnownType(typeof(Stream))]
        [ServiceKnownType(typeof(Amigo))]
        bool EliminarAmigo(string usuarioRemitente, string destinatario);
    }
    [ServiceContract]
    public interface IAmistadCallBack
    {
        /// <summary>
        /// Notifica al cliente sobre un cambio en el estado de un amigo.
        /// </summary>
        /// <param name="amigo">El amigo cuyo estado ha cambiado.</param>
        [OperationContract]
        [ServiceKnownType(typeof(MemoryStream))]
        [ServiceKnownType(typeof(Stream))]
        [ServiceKnownType(typeof(Amigo))]
        void CambiarEstadoAmigoCallback(Amigo amigo);
        /// <summary>
        /// Recibe información actualizada de un amigo como parte de un callback.
        /// </summary>
        /// <param name="amigo">El amigo cuya información se está actualizando.</param>
        [OperationContract]
        [ServiceKnownType(typeof(MemoryStream))]
        [ServiceKnownType(typeof(Stream))]
        [ServiceKnownType(typeof(Amigo))]
        void ObtenerAmigoCallback(Amigo amigo);
        /// <summary>
        /// Notifica al cliente que un amigo ha sido eliminado de la lista de amigos del usuario.
        /// </summary>
        /// <param name="amigo">El objeto 'Amigo' que representa al amigo eliminado, conteniendo todos los detalles relevantes como el identificador, nombre, etc.</param>
        /// <remarks>
        /// Este método es utilizado como un callback para informar al cliente de la eliminación exitosa de un amigo.
        /// Debe ser implementado por el cliente para manejar la actualización de su interfaz de usuario o lógica de negocio
        /// conforme a la eliminación del amigo.
        /// Es importante asegurar que los datos del objeto 'Amigo' se manejen de manera segura y conforme a las políticas de privacidad.
        /// </remarks>
        [OperationContract]
        [ServiceKnownType(typeof(MemoryStream))]
        [ServiceKnownType(typeof(Stream))]
        [ServiceKnownType(typeof(Amigo))]
        void  EliminarAmigoCallback(Amigo amigo);
    }
}
