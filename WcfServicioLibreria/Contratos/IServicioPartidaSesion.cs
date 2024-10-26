using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IPartidaCallback))]
    public interface IServicioPartidaSesion
    {
        [OperationContract]
        void UniserPartida(string gamertag, string idPartida);
        [OperationContract]
        void ConfirmarMovimiento();
        [OperationContract(IsOneWay = true)]
        void AvanzarRonda();
        [OperationContract(IsOneWay = true)]
        void FinalizarPartida();
        [OperationContract(IsOneWay = true)]
        void ExpulsarJugador();
        [OperationContract(IsOneWay = true)]
        void EnviarImagenCarta();
    }
    [ServiceContract]
    public interface IPartidaCallback
    {
        //Tal vez int porque regresa el numero de ronda???
        [OperationContract]
        void AvanzarRondaCallback();
    }
}
