using System;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioEstadisticas
    {
        public Estadistica ObtenerEstadisticas(int idUsuario)
        {
            try
            {
                Estadistica estadistica = new Estadistica();

                int idEstadistica = DAOLibreria.DAO.EstadisticasDAO.ObtenerIdEstadisticaConIdUsuario(idUsuario);
                if (idEstadistica > 0)
                {
                    var estadisticaModeloBD = DAOLibreria.DAO.EstadisticasDAO.RecuperarEstadisticas(idEstadistica);
                    estadistica = new Estadistica(estadisticaModeloBD);
                }

                return estadistica;
            }
            catch (ArgumentException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
                return new Estadistica(); 
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
                return new Estadistica(); 
            }
        }
    }
}
