using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioEstadisticas
    {
        [OperationContract]
        Estadistica ObtenerEstadisitca(int idUsuario);
    }
}
