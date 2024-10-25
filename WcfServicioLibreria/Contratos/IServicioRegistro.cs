using System.ServiceModel;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Modelo.Excepciones;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioRegistro
    {
        [OperationContract]
        [FaultContract(typeof(BaseDatosFalla))]
        bool RegistrarUsuario(Modelo.Usuario usuario );
    }
}
