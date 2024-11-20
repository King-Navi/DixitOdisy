using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioEstadisticas
    {
        public Estadistica ObtenerEstadisitca(int idUsuario)
        {
            Estadistica estadistica = new Estadistica();
            int idEstadisica = DAOLibreria.DAO.EstadisticasDAO.ObtenerIdEstadisticaConIdUsuario(idUsuario);
            if (idEstadisica > 0)
            {
                var estadisticaModeloBD = DAOLibreria.DAO.EstadisticasDAO.RecuperarEstadisticas(idEstadisica);
                estadistica = new Estadistica(estadisticaModeloBD);
            }
            return estadistica;
        }
    }
}
