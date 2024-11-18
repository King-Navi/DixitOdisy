using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IAmistadCallBack))]
    public interface IServicioAmistad
    {
        [OperationContract(IsOneWay = true)]
        void AbrirCanalParaPeticiones(Usuario usuario);
        [OperationContract]
        [FaultContract(typeof(SolicitudAmistadFalla))]
        bool EnviarSolicitudAmistad (Usuario usuarioRemitente, string destinatario);
        [OperationContract]
        List<Usuario> ObtenerSolicitudesAmistad (Usuario usuario);
        [OperationContract]
        bool AceptarSolicitudAmistad(int idRemitente, int idDestinatario);
        [OperationContract]
        bool RechazarSolicitudAmistad(int idRemitente, int idDestinatario);
    }
    [ServiceContract]
    public interface IAmistadCallBack
    {
        [OperationContract]
        void CambiarEstadoAmigo(Amigo amigo);
        [OperationContract]
        void ObtenerAmigoCallback(Amigo amigo);
        [OperationContract]
        void ObtenerPeticionAmistadCallback(SolicitudAmistad nuevaSolicitudAmistad);
    }
}
