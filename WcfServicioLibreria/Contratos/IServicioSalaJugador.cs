using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(ISalaJugadorCallback))]
    public interface IServicioSalaJugador
    {
        [OperationContract(IsOneWay = true)]
        Task AgregarJugadorSala(string gamertag, string idSala);
        [OperationContract]
        bool ComenzarPartidaAnfrition(string nombre, string idSala, string idPartida);
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
