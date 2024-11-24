using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioEstadisticas
    {
        /// <summary>
        /// Recupera las estadísticas asociadas a un usuario específico.
        /// </summary>
        /// <param name="idUsuario">El identificador del usuario cuyas estadísticas se desean obtener.</param>
        /// <returns>
        /// Un objeto <see cref="Estadistica"/> que contiene las estadísticas del usuario.
        /// Si no se encuentran estadísticas para el usuario, el método debería manejar adecuadamente esta situación,
        /// ya sea retornando un objeto nulo o lanzando una excepción específica, dependiendo de la implementación del servicio.
        /// </returns>
        [OperationContract]
        Estadistica ObtenerEstadisitca(int idUsuario);
    }
}
