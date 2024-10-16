using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IServicioAmistadCallBack))]
    public interface IServicioAmistad
    {
        [OperationContract(IsOneWay = true)]
        void AbrirCanalParaPeticiones(Usuario usuario);
    }
    [ServiceContract]
    public interface IServicioAmistadCallBack
    {
        [OperationContract]
        [ServiceKnownType(typeof(List<Amigo>))]
        void ObtenerListaAmigoCallback(List<Amigo> amigos);
        [OperationContract]
        void ObtenerPeticionAmistadCallback(SolicitudAmistad nuevaSolicitudAmistad);
    }
}
