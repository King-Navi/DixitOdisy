using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibreria
{
    public class EliminadorCadena
    {
        private const string nombre = "DescribeloEntities";
        public static void EliminarConnectionStringDelArchivo()
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.ConnectionStrings.ConnectionStrings[nombre] != null)
                {
                    config.ConnectionStrings.ConnectionStrings.Remove(nombre);
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("connectionStrings");
                    Console.WriteLine($"Connection string '{nombre}' eliminado del archivo de configuración.");
                }
                else
                {
                    Console.WriteLine($"Connection string '{nombre}' no encontrado en el archivo.");
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
