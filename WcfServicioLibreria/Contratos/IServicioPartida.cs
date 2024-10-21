using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IPartidaCallabck))]
    public interface IServicioPartida
    {
        [OperationContract]
        [ServiceKnownType(typeof(CondicionVictoriaPartida))]
        string ComenzarPartidaAnfrition(string nombre, ConfiguracionPartida configuracion);
        [OperationContract]
        void UniserPartida(string idPartida);
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
    public interface IPartidaCallabck
    {
        //Tal vez int porque regresa el numero de ronda???

    }
}
