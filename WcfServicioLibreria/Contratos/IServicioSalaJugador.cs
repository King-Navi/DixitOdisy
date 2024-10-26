using System.Collections.Generic;
using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(ISalaJugadorCallback))]
    public interface IServicioSalaJugador
    {
        [OperationContract(IsOneWay = true)]
        void AgregarJugadorSala(string gamertag, string idSala);
        [OperationContract(IsOneWay = true)]
        void ComenzarPartidaAnfrition(string nombre, string idSala);
        [OperationContract(IsOneWay = true)]
        void AsignarColor(string idSala);
    }
    [ServiceContract]
    public interface ISalaJugadorCallback
    {
        [OperationContract]
        void ObtenerJugadorSalaCallback(Usuario jugardoreNuevoEnSala);
        [OperationContract]
        void EliminarJugadorSalaCallback(Usuario jugardoreRetiradoDeSala);
        [OperationContract]
        void EmpezarPartidaCallBack(string idPartida);
        [OperationContract]
        void AsignarColorCallback(Dictionary<string, char> jugadoresColores);
    }
}
