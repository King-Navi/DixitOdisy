using System;
using System.Threading.Tasks;

namespace DAOLibreria.ModeloBD
{
    public static class Conexion
    {
        public static async Task<bool> VerificarConexionAsync()
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    await context.Database.ExecuteSqlCommandAsync("SELECT 1");
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
