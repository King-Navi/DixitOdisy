using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Modelo.Excepciones;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IPartidaCallback))]
    public interface IServicioPartidaSesion
    {
        [OperationContract(IsOneWay = true)]
        Task UnirsePartida(string gamertag, string idPartida);
        [OperationContract(IsOneWay = true)]
        void ConfirmarMovimiento(string nombreJugador, string idPartida , string claveImagen, string pista = null);
        [OperationContract(IsOneWay = true)]
        void ExpulsarJugador(string nombreJugador, string idPartida);
        [OperationContract]
        Task<bool> SolicitarImagenCartaAsync(string nombreJugador, string idPartida);
        [OperationContract(IsOneWay = true)]
        Task EmpezarPartida(string nombreJugador, string idPartida);
    }
    [ServiceContract]
    public interface IPartidaCallback
    {
        [OperationContract]
        void IniciarValoresPartidaCallback(bool seUnio); //FIXME
        [OperationContract]
        void AvanzarRondaCallback(int RondaActual);
        [OperationContract(IsOneWay = true)]
        void TurnoPerdidoCallback();
        [OperationContract(IsOneWay = true)]
        void RecibirImagenCallback(ImagenCarta imagen);
        [OperationContract(IsOneWay = true)]
        void RecibirGrupoImagenCallback(ImagenCarta imagen);
        [OperationContract(IsOneWay = true)]
        void FinalizarPartida();
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
        [ServiceKnownTypeAttribute(typeof(List<JugadorEstadisticas>))]
        [ServiceKnownTypeAttribute(typeof(JugadorEstadisticas))]
        void EnviarEstadisticas(EstadisticasPartida estadisticas);
        [OperationContract]
        void CambiarPantallaCallback(int numeroPantalla);
    }
}
