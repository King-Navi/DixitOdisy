using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibreria.Interfaces
{
    public interface IEstadisticasDAO
    {
        /// <summary>
        /// Agrega una estadística de partida a la entidad asociada.
        /// </summary>
        /// <param name="idEstadisticas">El ID de las estadísticas a modificar.</param>
        /// <param name="accion">La acción a realizar sobre las estadísticas.</param>
        /// <param name="victoria">La cantidad de victorias a registrar (0 o 1).</param>
        /// <returns>Un valor booleano que indica si la operación fue exitosa.</returns>
        /// <exception cref="ActividadSospechosaExcepcion">Se lanza si el parámetro victoria tiene un valor mayor a 1.</exception>
        Task<bool> AgregarEstadiscaPartidaAsync(int idEstadisticas, EstadisticasAcciones accion, int victoria);

        /// <summary>
        /// Recupera una entidad de estadísticas por su ID.
        /// </summary>
        /// <param name="idEstadisticas">El ID de las estadísticas a recuperar.</param>
        /// <returns>La entidad `Estadisticas` correspondiente.</returns>
        Estadisticas RecuperarEstadisticas(int idEstadisticas);

        /// <summary>
        /// Obtiene el ID de estadísticas asociadas a un ID de usuario.
        /// </summary>
        /// <param name="idUsuario">El ID del usuario.</param>
        /// <returns>El ID de las estadísticas asociadas o -1 en caso de error.</returns>
        int ObtenerIdEstadisticaConIdUsuario(int idUsuario);
    }
}
