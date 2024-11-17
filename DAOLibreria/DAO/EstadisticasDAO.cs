using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibreria.DAO
{
    public class EstadisticasDAO
    {
        public static async Task<bool> AgregarEstadiscaPartidaAsync(int idEstadisticas, EstadisticasAcciones accion, int victoria)
        {
            if (victoria > 1)
            {
                throw new ActividadSospechosaExcepcion() { id = idEstadisticas };
            }
            bool resultado = false;
            using (var context = new DescribeloEntities())
            {

                var estadistica = context.Estadisticas.Single(fila => fila.idEstadisticas == idEstadisticas);
                if (estadistica == null)
                {
                    return false;
                }
                context.Entry(estadistica).Reload();
                estadistica.partidasJugadas = (estadistica.partidasJugadas ?? 0) + 1;
                estadistica.partidasGanadas = (estadistica.partidasGanadas ?? 0) + victoria;

                var accionARealizar = ObtenerAccion(accion);
                accionARealizar(estadistica);
                await context.SaveChangesAsync();

                Console.WriteLine("Estadísticas agregadas exitosamente.");
                resultado = true;

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

                    if (estadistica == null)
                    {
                        throw new ArgumentException();
                    }

                    return estadistica;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }


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
                    throw new ArgumentOutOfRangeException(nameof(accion), "Acción no permitida.");
            }
        }
        public static int ObtenerIdEstadisticaConIdUsuario(int idUsuario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var usuario = context.Estadisticas.SingleOrDefault(fila => fila.idUsuario == idUsuario);

                    if (usuario == null)
                    {
                        throw new ArgumentException();
                    }

                    return usuario.idUsuario;
                }
            }
            catch (Exception)
            {
            }
            return -1;
        }
    }
}
