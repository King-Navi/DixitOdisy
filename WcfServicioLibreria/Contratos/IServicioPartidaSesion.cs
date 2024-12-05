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
        [OperationContract(IsOneWay = true)]
        void ExpulsarJugadorPartida(string nombreJugador, string idPartida);

        /// <summary>
        /// Inicia una partida después de validar su estado.
        /// </summary>
        /// <param name="nombreJugador">El nombre del jugador que solicita iniciar la partida.</param>
        /// <param name="idPartida">El identificador único de la partida.</param>
        /// <remarks>
        /// Este método notifica a todos los jugadores que la partida ha comenzado y ejecuta la lógica de inicio.
        /// </remarks>
        [OperationContract(IsOneWay = true)]
        Task EmpezarPartidaAsync(string nombreJugador, string idPartida);
    }
    [ServiceContract]
    public interface IPartidaCallback
    {
        [OperationContract(IsOneWay =true)]
        void IniciarValoresPartidaCallback(bool seUnio);

        [OperationContract(IsOneWay = true)]
        void TurnoPerdidoCallback();

        [OperationContract(IsOneWay = true)]
        void FinalizarPartidaCallback();

        [OperationContract(IsOneWay = true)]
        void ObtenerJugadorPartidaCallback(Usuario jugardoreNuevoEnSala);

        [OperationContract(IsOneWay = true)]
        void EliminarJugadorPartidaCallback(Usuario jugardoreRetiradoDeSala);

        [OperationContract(IsOneWay = true)]
        void NotificarNarradorCallback(bool esNarrador);

        [OperationContract(IsOneWay = true)]
        void MostrarPistaCallback(string pista);

        [OperationContract(IsOneWay = true)]
        [ServiceKnownTypeAttribute(typeof(EstadisticasPartida))]
        [ServiceKnownTypeAttribute(typeof(List<JugadorPuntaje>))]
        [ServiceKnownTypeAttribute(typeof(JugadorPuntaje))]
        void EnviarEstadisticasCallback(EstadisticasPartida estadisticas);

        [OperationContract(IsOneWay = true)]
        void CambiarPantallaCallback(int numeroPantalla);
    }
}
