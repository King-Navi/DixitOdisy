using System.ServiceModel;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioChat
    {
        [OperationContract]
        bool CrearChat(string idChat);
        [OperationContract]
        bool EliminarChat();

    }
}
