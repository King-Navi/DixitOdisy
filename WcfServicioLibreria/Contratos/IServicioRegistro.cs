using System.ServiceModel;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioRegistro
    {
        [OperationContract]
        bool RegistrarUsuario(Modelo.Usuario usuario);
    }
}
