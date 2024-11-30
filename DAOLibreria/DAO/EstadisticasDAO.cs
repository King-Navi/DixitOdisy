using DAOLibreria.Excepciones;
using DAOLibreria.Interfaces;
using DAOLibreria.ModeloBD;
using DAOLibreria.Utilidades;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace DAOLibreria.DAO
{
    public class EstadisticasDAO : IEstadisticasDAO
    {
        private const int AUMENTO_MAXIMO_PARTIDAS_JUGADAS = 1;
        private const int AUMENTO_MAXIMO_VICTORIAS = 1;
        private const int SIN_PARTIDAS_JUGADAS = 0;
        private const int SIN_VICTORIA = 0;
        public async Task<bool> AgregarEstadiscaPartidaAsync(int idEstadisticas, EstadisticasAcciones accion, int victoria)
        {
            if (victoria > AUMENTO_MAXIMO_VICTORIAS)
            {
                throw new ActividadSospechosaExcepcion()
                {
                    Identificador = idEstadisticas
                };
            }
            bool resultado = false;
            try
            {
                using (var context = new DescribeloEntities())
                {

                    var estadistica = context.Estadisticas.Single(fila => fila.idEstadisticas == idEstadisticas);
                    if (estadistica == null)
                    {
                        return false;
                    }
                    if (estadistica != null)
                    {
                        context.Entry(estadistica).Reload();
                    }
                    estadistica.partidasJugadas = (estadistica.partidasJugadas ?? SIN_PARTIDAS_JUGADAS) + AUMENTO_MAXIMO_PARTIDAS_JUGADAS;
                    estadistica.partidasGanadas = (estadistica.partidasGanadas ?? SIN_VICTORIA) + victoria;

                    var accionARealizar = ObtenerAccion(accion);
                    accionARealizar(estadistica);
                    await context.SaveChangesAsync();
                    resultado = true;

                }
            }
            catch (DbUpdateException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return resultado;
        }

        public Estadisticas RecuperarEstadisticas(int idEstadisticas)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var estadistica = context.Estadisticas.SingleOrDefault(fila => fila.idEstadisticas == idEstadisticas);

                    return estadistica ?? throw new ArgumentException();
                }
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }

            return null;
        }

        private Action<Estadisticas> ObtenerAccion(EstadisticasAcciones accion)
        {
            switch (accion)
            {
                case EstadisticasAcciones.IncrementarPartidasMitologia:
                    return estadistica => { estadistica.vecesTematicaMitologia = (estadistica.vecesTematicaMitologia ?? SIN_PARTIDAS_JUGADAS) + AUMENTO_MAXIMO_PARTIDAS_JUGADAS; };
                case EstadisticasAcciones.IncrementarPartidaMixta:
                    return estadistica => { estadistica.vecesTematicaMixto = (estadistica.vecesTematicaMixto ?? SIN_PARTIDAS_JUGADAS) + AUMENTO_MAXIMO_PARTIDAS_JUGADAS; };
                case EstadisticasAcciones.IncrementarPartidaEspacio:
                    return estadistica => { estadistica.vecesTematicaEspacio = (estadistica.vecesTematicaEspacio ?? SIN_PARTIDAS_JUGADAS) + AUMENTO_MAXIMO_PARTIDAS_JUGADAS; };
                case EstadisticasAcciones.IncrementarPartidaAnimales:
                    return estadistica => { estadistica.vecesTematicaAnimales = (estadistica.vecesTematicaAnimales ?? SIN_PARTIDAS_JUGADAS) + AUMENTO_MAXIMO_PARTIDAS_JUGADAS; };
                case EstadisticasAcciones.IncrementarPartidaPaises:
                    return estadistica => { estadistica.vecesTematicaPaises = (estadistica.vecesTematicaPaises ?? SIN_PARTIDAS_JUGADAS) + AUMENTO_MAXIMO_PARTIDAS_JUGADAS; };
                default:
                    return estadistica => { estadistica.vecesTematicaMixto = (estadistica.vecesTematicaMixto ?? SIN_PARTIDAS_JUGADAS) + AUMENTO_MAXIMO_PARTIDAS_JUGADAS; };
            }
        }

        public int ObtenerIdEstadisticaConIdUsuario(int idUsuario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var usuario = context.Estadisticas.SingleOrDefault(fila => fila.idUsuario == idUsuario);

                    return usuario == null ? throw new ArgumentException() : usuario.idUsuario;
                }
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return -1;
        }
    }
}
