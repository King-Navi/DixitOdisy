using System.Collections.Generic;
using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(ISalaJugadorCallback))]
    public interface IServicioSalaJugador
    {
        [OperationContract]
        [FaultContract(typeof(UsuarioFalla))]
        bool AgregarJugadorSala(string gamertag, string idSala);
        [OperationContract]
        void RemoverJugadorSala(string gamertag, string ididSalaRoom);
        [OperationContract(IsOneWay = true)]
        void ObtenerJugadoresSala(string gamertag, string idSala);
        [OperationContract(IsOneWay = true)]
        void EmpezarPartida(string idSala);
        [OperationContract(IsOneWay = true)]
        void AsignarColor(string idSala);
    }
    [ServiceContract]
    public interface ISalaJugadorCallback
    {
        [OperationContract]
        void ObtenerJugadoresSalaCallback(Usuario[] jugardoresEnSala);
        [OperationContract]
        void EmpezarPartidaCallBack();
        [OperationContract]
        void AsignarColorCallback(Dictionary<string, char> jugadoresColores);
    }
}
