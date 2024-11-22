using System.ServiceModel;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioSala
    {
        [OperationContract]
        string CrearSala(string nombreUsuarioAnfitrion);
        [OperationContract]
        bool ValidarSala(string idSala);
    }
}
