using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioAmistad
    {
        [OperationContract]
        [FaultContract(typeof(AmistadFalla))]
        void AgregarAmigo(Usuario remitente, string destinatario);
        [OperationContract]
        int ActualizarAmigo(string nombreRemitente, string nombreDestinatario, string peticionEstado);
        [OperationContract]
        int BorrarAmigo(Amigo amigo);
        [OperationContract]
        Amigo[] ObtenerListaAmigos(string usuario);
    }
}
