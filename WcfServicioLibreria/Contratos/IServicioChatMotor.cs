using System.Collections.Generic;
using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    /// <summary>
    /// Define un contrato de servicio para un sistema de chat, permitiendo la gestión de usuarios y el envío de mensajes.
    /// Permite sesiones, aunque no es obligatorio mantener una.
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed, CallbackContract = typeof(IChatCallback))]
    public interface IServicioChatMotor
    {
        /// <summary>
        /// Intenta agregar un nuevo usuario al chat.
        /// </summary>
        /// <param name="nombreUsuario">Datos del usuario a agregar.</param>
        /// <returns>Verdadero si el usuario es agregado exitosamente; de lo contrario, falso.</returns>
        /// <exception cref="UsuarioDuplicadoFalla">Se lanza si el usuario ya existe en el sistema.</exception>
        [OperationContract]
        [FaultContract(typeof(UsuarioDuplicadoFalla))]
        bool AgregarUsuarioChat(string idChat, string nombreUsuario);
        /// <summary>
        /// Desconecta a un usuario del chat.
        /// </summary>
        /// <param name="usuario">El usuario a desconectar.</param>
        /// <returns>Verdadero si la desconexión fue exitosa; de lo contrario, falso.</returns>
        [OperationContract]
        bool DesconectarUsuarioChat(string nombreUsuario);
        /// <summary>
        /// Obtiene el estado actual del jugador.
        /// </summary>
        /// <param name="usuario">El usuario cuyo estado se está consultando.</param>
        /// <returns>Verdadero si el jugador está activo; de lo contrario, falso.</returns>
        [OperationContract]
        bool EstadoJugador(string nombreUsuario);
        /// <summary>
        /// Envia un mensaje de chat desde un usuario.
        /// </summary>
        /// <param name="usuario">El usuario que envía el mensaje.</param>
        /// <param name="mensaje">El contenido del mensaje a enviar.</param>

        [OperationContract(IsOneWay = false)]
        void EnviarMensaje(string idChat, ChatMensaje mensaje);
        /// <summary>
        /// Obtiene el historial de mensajes del chat.
        /// </summary>
        /// <returns>Una lista de mensajes de chat.</returns>
        [OperationContract]
        List<ChatMensaje> GetMessageHistory();

    }
    /// <summary>
    /// Define los métodos de callback para los clientes del servicio de chat, permitiendo recibir mensajes.
    /// </summary>
    [ServiceContract]
    public interface IChatCallback
    {
        /// <summary>
        /// Envia un mensaje reciente a un cliente.
        /// Este método es unidireccional, significando que el servicio no espera una respuesta del cliente.
        /// </summary>
        /// <param name="mensaje">El mensaje de chat a enviar al cliente.</param>
        [OperationContract(IsOneWay = true)]
        void RecibirMensajeCliente(ChatMensaje mensaje);

    }
}
