using DAOLibreria.Utilidades;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DAOLibreria.ModeloBD
{
    public static class Conexion
    {
        private const string CONSULTA_DE_PRUEBA = "SELECT 1";
        public static async Task<bool> VerificarConexionAsync()
        {
            try
            {
                using (var contexto = new DescribeloEntities())
                {
                    await contexto.Database.ExecuteSqlCommandAsync(CONSULTA_DE_PRUEBA);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool VerificarConexion()
        {
            try
            {
                using (var contexto = new DescribeloEntities())
                {
                    contexto.Database.ExecuteSqlCommand(CONSULTA_DE_PRUEBA);
                    return true;
                }
            }
            catch (SqlException excepcion)
            {
                ManejadorExcepciones.ManejarFatalException(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarFatalException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }

            return false;
        }
    }
}
