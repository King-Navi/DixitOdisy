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
                int idEstadistica = estadisticasDAO.ObtenerIdEstadisticaConIdUsuario(idUsuario);
                if (idEstadistica > ID_INVALIDO)
                {
                    var estadisticaModeloBD = estadisticasDAO.RecuperarEstadisticas(idEstadistica);
                    estadistica = new Estadistica(estadisticaModeloBD);
                    return estadistica;
                }

            }
            catch (ArgumentException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return new Estadistica();
        }
    }
}