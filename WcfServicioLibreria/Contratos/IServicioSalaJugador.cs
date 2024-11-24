using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(ISalaJugadorCallback))]
    public interface IServicioSalaJugador
    {
        /// <summary>
        /// Agrega un jugador a una sala especificada de manera asíncrona.
        /// </summary>
        /// <param name="usuario">El identificador del usuario que se agregará a la sala.</param>
        /// <param name="idSala">El identificador de la sala a la que el jugador será agregado.</param>
        /// <remarks>
        /// Este método es unidireccional y no devuelve una respuesta directa al llamador.
        /// </remarks>
        [OperationContract(IsOneWay = true)]
        Task AgregarJugadorSala(string usuario, string idSala);

        /// <summary>
        /// Permite al anfitrión de una sala comenzar una partida.
        /// </summary>
        /// <param name="nombre">El nombre del anfitrión que inicia la partida.</param>
        /// <param name="idSala">El identificador de la sala donde se inicia la partida.</param>
        /// <param name="idPartida">El identificador de la partida que se va a iniciar.</param>
        [OperationContract(IsOneWay = true)]
        void ComenzarPartidaAnfrition(string nombre, string idSala, string idPartida);

        /// <summary>
        /// Expulsa a un jugador de una sala.
        /// </summary>
        /// <param name="anfitrion">El anfitrión que realiza la expulsión.</param>
        /// <param name="jugadorAExpulsar">El identificador del jugador a expulsar de la sala.</param>
        /// <param name="idSala">El identificador de la sala de donde se expulsará al jugador.</param>
        [OperationContract(IsOneWay = true)]
        void ExpulsarJugadorSala(string anfitrion, string jugadorAExpulsar, string idSala);
    }
    [ServiceContract]
    public interface ISalaJugadorCallback
    {
        /// <summary>
        /// Notifica a los clientes cuando un nuevo jugador se ha unido a la sala.
        /// </summary>
        /// <param name="jugardoreNuevoEnSala">El usuario que se ha unido a la sala.</param>
        [OperationContract]
        void ObtenerJugadorSalaCallback(Usuario jugardoreNuevoEnSala);

        /// <summary>
        /// Notifica a los clientes cuando un jugador ha sido retirado de la sala.
        /// </summary>
        /// <param name="jugardoreRetiradoDeSala">El usuario que ha sido retirado de la sala.</param>
        [OperationContract]
        void EliminarJugadorSalaCallback(Usuario jugardoreRetiradoDeSala);

        /// <summary>
        /// Notifica a todos los jugadores en la sala que la partida ha comenzado.
        /// </summary>
        /// <param name="idPartida">El identificador de la partida que ha comenzado.</param>
        [OperationContract(IsOneWay = true)]
        void EmpezarPartidaCallBack(string idPartida);

        /// <summary>
        /// Notifica a un jugador si ha sido asignado como anfitrión de la sala.
        /// </summary>
        /// <param name="esAnfitrion">Indica si el jugador es el nuevo anfitrión.</param>
        [OperationContract(IsOneWay = true)]
        void DelegacionRolCallback(bool esAnfitrion);

    }
}
