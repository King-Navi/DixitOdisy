using System;
using System.Threading.Tasks;

namespace DAOLibreria.ModeloBD
{
    public static class Conexion
    {
        private const string CONSULTA_PING = "SELECT 1";
        public static async Task<bool> VerificarConexionAsync()
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    await context.Database.ExecuteSqlCommandAsync(CONSULTA_PING);
                    return true; 
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
