using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Modelo.Excepciones;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IPartidaCallback))]
    public interface IServicioPartidaSesion
    {
        /// <summary>
        /// Permite que un usuario se una a una partida existente.
        /// </summary>
        /// <param name="nombreUsuario">El nombre del usuario que desea unirse a la partida.</param>
        /// <param name="idPartida">El identificador único de la partida.</param>
        [OperationContract]
        [FaultContract(typeof(PartidaFalla))]
        [FaultContract(typeof(FaultException<PartidaFalla>))]
        Task<bool> UnirsePartidaAsync(string nombreUsuario, string idPartida);

        /// <summary>
        /// Confirma el movimiento de un jugador en una partida.
        /// </summary>
        /// <param name="nombreJugador">El nombre del jugador que realiza el movimiento.</param>
        /// <param name="idPartida">El identificador único de la partida.</param>
        /// <param name="claveImagen">La clave de la imagen seleccionada por el jugador.</param>
        /// <param name="pista">(Opcional) La pista proporcionada por el narrador.</param>
        /// <remarks>
        /// Este método maneja movimientos realizados tanto por el narrador (incluyendo pistas) como por los jugadores al elegir imágenes.
        /// </remarks>
        [OperationContract(IsOneWay = true)]
        void ConfirmarMovimiento(string nombreJugador, string idPartida , string claveImagen, string pista = null);

        /// <summary>
        /// Permite a un jugador tratar de adivinar una imagen en la partida.
        /// </summary>
        /// <param name="nombreJugador">El nombre del jugador que intenta adivinar.</param>
        /// <param name="idPartida">El identificador único de la partida.</param>
        /// <param name="claveImagen">La clave de la imagen seleccionada como respuesta.</param>
        /// <remarks>
        /// Este método valida que el turno sea correcto y registra el intento de adivinar del jugador.
        /// </remarks>
        [OperationContract]
        void TratarAdivinar(string nombreJugador, string idPartida, string claveImagen);

        /// <summary>
        /// Expulsa a un jugador de una partida.
        /// </summary>
        /// <param name="nombreJugador">El nombre del jugador que será expulsado.</param>
        /// <param name="idPartida">El identificador único de la partida.</param>
        [OperationContract]
        Task<bool> ExpulsarJugadorPartida(string anfitrion, string jugadorAExpulsar, string idPartida);

        /// <summary>
        /// Inicia una partida después de validar su estado.
        /// </summary>
        /// <param name="nombreJugador">El nombre del jugador que solicita iniciar la partida.</param>
        /// <param name="idPartida">El identificador único de la partida.</param>
        /// <remarks>
        /// Este método notifica a todos los jugadores que la partida ha comenzado y ejecuta la lógica de inicio.
        /// </remarks>
        [OperationContract]
        Task EmpezarPartidaAsync( string idPartida);
    }
    [ServiceContract]
    public interface IPartidaCallback
    {
        /// <summary>
        /// Notifica al cliente si se unió correctamente a la partida.
        /// </summary>
        /// <param name="seUnio">True si el jugador se unió con éxito, false en caso contrario.</param>
        [OperationContract(IsOneWay =true)]
        void IniciarValoresPartidaCallback(bool seUnio);
        /// <summary>
        /// Notifica al cliente que perdió su turno.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void TurnoPerdidoCallback();
        /// <summary>
        /// Notifica al cliente que la partida ha finalizado.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void FinalizarPartidaCallback();
        /// <summary>
        /// Notifica al cliente que un nuevo jugador se ha unido a la sala.
        /// </summary>
        /// <param name="jugardoreNuevoEnSala">Información del jugador que se unió.</param>
        [OperationContract(IsOneWay = true)]
        void ObtenerJugadorPartidaCallback(Usuario jugardoreNuevoEnSala);
        /// <summary>
        /// Notifica al cliente que un jugador se ha retirado de la sala.
        /// </summary>
        /// <param name="jugardoreRetiradoDeSala">Información del jugador que se retiró.</param>
        [OperationContract(IsOneWay = true)]
        void EliminarJugadorPartidaCallback(Usuario jugardoreRetiradoDeSala);
        /// <summary>
        /// Notifica al cliente si ha sido asignado como narrador de la partida.
        /// </summary>
        /// <param name="esNarrador">True si el cliente es el narrador, false en caso contrario.</param>
        [OperationContract(IsOneWay = true)]
        void NotificarNarradorCallback(bool esNarrador);
        /// <summary>
        /// Envía una pista al cliente durante la partida.
        /// </summary>
        /// <param name="pista">Texto de la pista.</param>
        [OperationContract(IsOneWay = true)]
        void MostrarPistaCallback(string pista);
        /// <summary>
        /// Envía estadísticas actualizadas de la partida al cliente.
        /// </summary>
        /// <param name="estadisticas">Objeto que contiene las estadísticas de la partida.</param>
        [OperationContract(IsOneWay = true)]
        [ServiceKnownTypeAttribute(typeof(EstadisticasPartida))]
        [ServiceKnownTypeAttribute(typeof(List<JugadorPuntaje>))]
        [ServiceKnownTypeAttribute(typeof(JugadorPuntaje))]
        void EnviarEstadisticasCallback(EstadisticasPartida estadisticas, bool esAnfitrion);
        /// <summary>
        /// Cambia la pantalla actual del cliente a la especificada por el servidor.
        /// </summary>
        /// <param name="numeroPantalla">Número de la pantalla a la que se debe cambiar.</param>
        [OperationContract(IsOneWay = true)]
        void CambiarPantallaCallback(int numeroPantalla);
        /// <summary>
        /// Notifica al cliente que la partida no pudo comenzar.
        /// </summary>
        /// <remarks>
        /// Este método es un contrato de operación unidireccional, lo que significa que el servidor 
        /// envía la notificación al cliente sin esperar una respuesta.
        /// </remarks>
        [OperationContract(IsOneWay = true)]
        void NoSeEmpezoPartidaCallback();
        /// <summary>
        /// Muestra el tablero de cartas al cliente.
        /// </summary>
        /// <remarks>
        /// Este método permite al servidor notificar al cliente para que despliegue el tablero de cartas.
        /// Es un contrato unidireccional, por lo que no espera respuesta del cliente.
        /// </remarks>
        [OperationContract(IsOneWay = true)]
        void MostrarTableroCartasCallback();
    }
}
