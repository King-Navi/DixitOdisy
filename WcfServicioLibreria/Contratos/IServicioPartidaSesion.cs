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
        void UnirsePartida(string gamertag, string idPartida);
        [OperationContract]
        [FaultContract(typeof(PartidaFalla))]
        void ConfirmarMovimiento(string nombreJugador, string idPartida , string claveImagen);
        [OperationContract(IsOneWay = true)]
        void ExpulsarJugador(string nombreJugador, string idPartida);
        [OperationContract(IsOneWay = true)]
        Task SolicitarImagenCartaAsync(string nombreJugador, string idPartida);
        [OperationContract(IsOneWay = true)]
        void EmpezarPartida(string nombreJugador, string idPartida);
    }
    [ServiceContract]
    public interface IPartidaCallback
    {
        [OperationContract(IsOneWay = true)]
        void AvanzarRondaCallback(int RondaActual);
        [OperationContract(IsOneWay = true)]
        void TurnoPerdidoCallback();
        [OperationContract(IsOneWay = true)]
        void RecibirImagenCallback(ImagenCarta imagen);
        [OperationContract(IsOneWay = true)]
        void FinalizarPartida();
        [OperationContract(IsOneWay = true)]
        void ObtenerJugadorPartidaCallback(Usuario jugardoreNuevoEnSala);
        [OperationContract(IsOneWay = true)]
        void EliminarJugadorPartidaCallback(Usuario jugardoreRetiradoDeSala);
    }
}
