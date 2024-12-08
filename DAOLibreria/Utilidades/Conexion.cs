using DAOLibreria.Interfaces;
using DAOLibreria.Utilidades;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DAOLibreria.ModeloBD
{
    public class Conexion : IConexion
    {
        private const string CONSULTA_DE_PRUEBA = "SELECT 1";
        public async Task<bool> VerificarConexionAsync()
        {
            try
            {
                using (var contexto = new DescribeloEntities())
                {
                    await contexto.Database.ExecuteSqlCommandAsync(CONSULTA_DE_PRUEBA);
                    return true;
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
                return false;
            }
        }
        public bool VerificarConexion()
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
