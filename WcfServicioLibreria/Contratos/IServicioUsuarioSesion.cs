using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IUsuarioSesionCallback))]
    public interface IServicioUsuarioSesion
    {
        [OperationContract(IsOneWay = true)]
        void ObtenerSessionJugador(Usuario usuario);
    }
    [ServiceContract]
    public interface IUsuarioSesionCallback
    {
        [OperationContract]
        void ObtenerSessionJugadorCallback(bool esSesionAbierta);
    }

}
