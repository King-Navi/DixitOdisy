using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioEstadisticas
    {
        public Estadistica ObtenerEstadisitca(int idUsuario)
        {
            Estadistica estadistica = new Estadistica();
            int idEstadisica = estadisticasDAO.ObtenerIdEstadisticaConIdUsuario(idUsuario);
            if (idEstadisica > 0)
            {
                var estadisticaModeloBD = estadisticasDAO.RecuperarEstadisticas(idEstadisica);
                estadistica = new Estadistica(estadisticaModeloBD);
            }
            return estadistica;
        }
    }
}
