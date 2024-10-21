using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IServicioAmistadCallBack))]
    public interface IServicioAmistad
    {
        [OperationContract(IsOneWay = true)]
        [ServiceKnownType(typeof(EstadoAmigo))]
        [ServiceKnownType(typeof(List<Amigo>))]
        void AbrirCanalParaPeticiones(Usuario usuario);
    }
    [ServiceContract]
    public interface IServicioAmistadCallBack
    {
        [OperationContract]
        [ServiceKnownType(typeof(EstadoAmigo))]
        [ServiceKnownType(typeof(List<Amigo>))]
        void ObtenerListaAmigoCallback(List<Amigo> amigos);
        [OperationContract]
        void ObtenerPeticionAmistadCallback(SolicitudAmistad nuevaSolicitudAmistad);
    }
}
