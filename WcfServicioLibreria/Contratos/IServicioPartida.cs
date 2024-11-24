using System;
using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioPartida
    {
        /// <summary>
        /// Crea una nueva partida basada en la configuración proporcionada por el anfitrión.
        /// </summary>
        /// <param name="anfitrion">El identificador del anfitrión que crea la partida.</param>
        /// <param name="configuracion">La configuración detallada de la partida, incluyendo parámetros como duración, número de jugadores, etc.</param>
        /// <returns>El identificador único de la partida creada.</returns>
        [OperationContract]
        string CrearPartida(string anfitrion, ConfiguracionPartida configuracion);

        /// <summary>
        /// Borra una partida existente. Este método se llama en respuesta a un evento, como por ejemplo, cuando todos los jugadores se han desconectado.
        /// </summary>
        /// <param name="sender">El objeto que envía la solicitud de borrado, generalmente la partida que se va a eliminar.</param>
        /// <param name="e">Argumentos del evento relacionados con la solicitud de borrado.</param>
        /// <remarks>
        /// Este método no está decorado con OperationContract porque está destinado a ser invocado internamente o en respuesta a eventos específicos.
        /// </remarks>
        void BorrarPartida(object sender, EventArgs e);

        /// <summary>
        /// Valida si una partida con un identificador específico existe y está en un estado válido para ser jugada o accedida.
        /// </summary>
        /// <param name="idPartida">El identificador de la partida a validar.</param>
        /// <returns>True si la partida existe y es válida; False en caso contrario.</returns>
        [OperationContract]
        bool ValidarPartida(string idPartida);

        /// <summary>
        /// Verifica si una partida ya ha comenzado.
        /// </summary>
        /// <param name="idPartida">El identificador de la partida a verificar.</param>
        /// <returns>True si la partida ha empezado; False si aún no comienza o si el identificador no corresponde a una partida válida.</returns>
        [OperationContract]
        bool EsPartidaEmpezada(string idPartida);
    }

}
