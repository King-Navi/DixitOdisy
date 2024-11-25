using DAOLibreria.ModeloBD;
using System;
using System.Linq;

namespace DAOLibreria.DAO
{
    public class UsuarioCuentaDAO
    {
        /// <summary>
        /// Obtiene el identificador de cuenta asociado a un identificador de usuario específico.
        /// </summary>
        /// <param name="idUsuario">El identificador del usuario para el cual se desea obtener el identificador de cuenta asociado.</param>
        /// <returns>
        /// El identificador de la cuenta del usuario si existe; null si el usuario no tiene una cuenta asociada
        /// o si el identificador de cuenta es 0. Retorna -1 en caso de cualquier error durante la operación de recuperación.
        /// </returns>
        /// <remarks>
        /// Este método realiza una consulta a la base de datos utilizando Entity Framework para obtener el identificador de cuenta.
        /// Utiliza manejo de excepciones para asegurar que cualquier error en la consulta no interrumpa el flujo de la aplicación
        /// y retorna -1 como indicador de error. Este valor de retorno debería ser manejado adecuadamente por el código que invoca este método
        /// para distinguir entre diferentes condiciones de fallo o ausencia de datos.
        /// </remarks>
        public static int? ObtenerIdUsuarioCuentaPorIdUsuario(int idUsuario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var idUsuarioCuenta = context.Usuario
                                                  .Where(u => u.idUsuario == idUsuario)
                                                  .Select(u => u.idUsuarioCuenta)
                                                  .FirstOrDefault();

                    return idUsuarioCuenta == 0 ? (int?)null : idUsuarioCuenta;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
