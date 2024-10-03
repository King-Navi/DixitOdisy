using System.ServiceModel;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioRegistro
    {
        [OperationContract]
        int RegistrarUsuario(string usuario, string contrasenia);
    }
}
