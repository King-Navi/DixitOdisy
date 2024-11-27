using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DAOLibreria.DAO
{
    public class EstadisticasDAO
    {
        public static async Task<bool> AgregarEstadiscaPartidaAsync(int idEstadisticas, EstadisticasAcciones accion, int victoria)
        {
            if (victoria > 1)
            {
                throw new ActividadSospechosaExcepcion() { Identificador = idEstadisticas };
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
                    estadistica.partidasJugadas = (estadistica.partidasJugadas ?? 0) + 1;
                    estadistica.partidasGanadas = (estadistica.partidasGanadas ?? 0) + victoria;

                    var accionARealizar = ObtenerAccion(accion);
                    accionARealizar(estadistica);
                    await context.SaveChangesAsync();
                    resultado = true;

                }
            }
            catch (Exception)
            {
            }
            return resultado;
        }

        public static Estadisticas RecuperarEstadisticas(int idEstadisticas)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var estadistica = context.Estadisticas.SingleOrDefault(fila => fila.idEstadisticas == idEstadisticas);

                    return estadistica ?? throw new ArgumentException();
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        //Pregunar al profe: metodo con alta complejidad ciclomatica
        private static Action<Estadisticas> ObtenerAccion(EstadisticasAcciones accion)
        {
            switch (accion)
            {
                case EstadisticasAcciones.IncrementarPartidasMitologia:
                    return estadistica => { estadistica.vecesTematicaMitologia = (estadistica.vecesTematicaMitologia ?? 0) + 1; };
                case EstadisticasAcciones.IncrementarPartidaMixta:
                    return estadistica => { estadistica.vecesTematicaMixto = (estadistica.vecesTematicaMixto ?? 0) + 1; };
                case EstadisticasAcciones.IncrementarPartidaEspacio:
                    return estadistica => { estadistica.vecesTematicaEspacio = (estadistica.vecesTematicaEspacio ?? 0) + 1; };
                case EstadisticasAcciones.IncrementarPartidaAnimales:
                    return estadistica => { estadistica.vecesTematicaAnimales = (estadistica.vecesTematicaAnimales ?? 0) + 1; };
                case EstadisticasAcciones.IncrementarPartidaPaises:
                    return estadistica => { estadistica.vecesTematicaPaises = (estadistica.vecesTematicaPaises ?? 0) + 1; };
                default:
                    return estadistica => { estadistica.vecesTematicaMixto = (estadistica.vecesTematicaMixto ?? 0) + 1; };
            }
        }

        public static int ObtenerIdEstadisticaConIdUsuario(int idUsuario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var usuario = context.Estadisticas.SingleOrDefault(fila => fila.idUsuario == idUsuario);

                    return usuario == null ? throw new ArgumentException() : usuario.idUsuario;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
