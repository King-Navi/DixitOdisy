using DAOLibreria.ModeloBD;
using DAOLibreria.Utilidades;
using System;
using System.Linq;

namespace DAOLibreria.DAO
{
    public class UsuarioCuentaDAO
    {
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
