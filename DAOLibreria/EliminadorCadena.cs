using DAOLibreria.Utilidades;
using System;
using System.Configuration;

namespace DAOLibreria
{
    public static class EliminadorCadena
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
                }
            }
            catch (ConfigurationErrorsException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
        }
    }
}
