using System;
using System.Configuration;

namespace DAOLibreria
{
    public class EliminadorCadena
    {
        private const string NOMBRE = "DescribeloEntities";
        public static void EliminarConnectionStringDelArchivo()
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.ConnectionStrings.ConnectionStrings[NOMBRE] != null)
                {
                    config.ConnectionStrings.ConnectionStrings.Remove(NOMBRE);
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("connectionStrings");
                    Console.WriteLine($"Connection string '{NOMBRE}' eliminado del archivo de configuración.");
                }
                else
                {
                    Console.WriteLine($"Connection string '{NOMBRE}' no encontrado en el archivo.");
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
