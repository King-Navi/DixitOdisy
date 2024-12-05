using DAOLibreria.ModeloBD;
using DAOLibreria.Utilidades;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Security;

namespace DAOLibreria
{
    public static class ConfiguradorConexion
    {
        private const string NOMBRE_CADENA_CONTEXTO = "DescribeloEntities";
        private const string CARPETA = "ModeloBD";
        private const string NOMBRE_MODELO_ENTIDAD = "DescribeloBD";

        public static bool ConfigurarCadenaConexion(string servidor, string nombreBD, string usuario, string contrasena)
        {
            string nuevaCadenaConexion = $"metadata=res://*/{CARPETA}.{NOMBRE_MODELO_ENTIDAD}.csdl|res://*/{CARPETA}.{NOMBRE_MODELO_ENTIDAD}.ssdl|res://*/{CARPETA}.{NOMBRE_MODELO_ENTIDAD}.msl;" +
                                         $"provider=System.Data.SqlClient;provider connection string=\"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};MultipleActiveResultSets=True;App=EntityFramework\";";
            ActualizarCadenaConexionEnAppConfig(NOMBRE_CADENA_CONTEXTO, nuevaCadenaConexion);
            return ProbarConexion();

        }

        public static bool ConfigurarCadenaConexion(string nombreVariableEntorno)
        {
            bool resultado = false;
            try
            {
                string valoresVariableEntorno = Environment.GetEnvironmentVariable(nombreVariableEntorno, EnvironmentVariableTarget.Machine);
                if (string.IsNullOrEmpty(valoresVariableEntorno))
                {
                    return resultado;
                }
                string[] valoresLista = ExtraerValoresDeCadenaConexion(valoresVariableEntorno) ?? throw new Exception("Es vacio al recuperar los valores de ExtraerValoresDeCadenaConexion");
                string servidor = valoresLista[0];
                string nombreBD = valoresLista[1];
                string usuario = valoresLista[2];
                string contrasena = valoresLista[3];
                string nuevaCadenaConexion = $"metadata=res://*/{CARPETA}.{NOMBRE_MODELO_ENTIDAD}.csdl|res://*/{CARPETA}.{NOMBRE_MODELO_ENTIDAD}.ssdl|res://*/{CARPETA}.{NOMBRE_MODELO_ENTIDAD}.msl;" +
                                             $"provider=System.Data.SqlClient;provider connection string=\"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};MultipleActiveResultSets=True;App=EntityFramework\";";

                ActualizarCadenaConexionEnAppConfig(NOMBRE_CADENA_CONTEXTO, nuevaCadenaConexion);

                resultado = ProbarConexion();
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarFatalException(excepcion);

            }
            catch (SecurityException excepcion)
            {
                ManejadorExcepciones.ManejarFatalException(excepcion);
            }
            catch (Exception exepcion)
            {
                ManejadorExcepciones.ManejarFatalException(exepcion);
            }
            return resultado;
        }

        private static void ActualizarCadenaConexionEnAppConfig(string nombreCadena, string nuevaCadena)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection seccion = (ConnectionStringsSection)config.GetSection("connectionStrings");
            if (seccion.ConnectionStrings[nombreCadena] != null)
            {
                seccion.ConnectionStrings[nombreCadena].ConnectionString = nuevaCadena;
                seccion.ConnectionStrings[nombreCadena].ProviderName = "System.Data.EntityClient";
            }
            else
            {
                seccion.ConnectionStrings.Add(new ConnectionStringSettings(nombreCadena, nuevaCadena, "System.Data.EntityClient"));
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");
        }
        public static bool ConfigurarCadenaConexionRuta()
        {
            bool resultado = false;
            try
            {
                string ruta = Console.ReadLine();

                string[] valores = ExtraerYValidarValoresDeArchivo(ruta);

                if (valores != null)
                {
                    string servidor = valores[0];
                    string nombreBD = valores[1];
                    string usuario = valores[2];
                    string contrasena = valores[3];

                    string nuevaCadenaConexion = $"metadata=res://*/{CARPETA}.{NOMBRE_MODELO_ENTIDAD}.csdl|res://*/{CARPETA}.{NOMBRE_MODELO_ENTIDAD}.ssdl|res://*/{CARPETA}.{NOMBRE_MODELO_ENTIDAD}.msl;" +
                                             $"provider=System.Data.SqlClient;provider connection string=\"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};MultipleActiveResultSets=True;App=EntityFramework\";";

                    ActualizarCadenaConexionEnAppConfig(NOMBRE_CADENA_CONTEXTO, nuevaCadenaConexion);

                    resultado = ProbarConexion();
                }
            }
            catch (FileNotFoundException exepcion)
            {
                ManejadorExcepciones.ManejarFatalException(exepcion);
            }
            catch (Exception exepcion)
            {
                ManejadorExcepciones.ManejarFatalException(exepcion);
            }
            return resultado;

        }

        private static string[] ExtraerYValidarValoresDeArchivo(string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
            {
                throw new FileNotFoundException($"El archivo no existe: {rutaArchivo}");
            }

            try
            {
                string contenidoArchivo = File.ReadAllText(rutaArchivo);
                string[] partes = contenidoArchivo.Split(';');
                if (partes.Length == 5)
                {
                    foreach (string parte in partes)
                    {
                        if (string.IsNullOrWhiteSpace(parte))
                        {
                            return null;
                        }
                    }
                    return partes;
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }

            return null;
        }

        private static bool ProbarConexion()
        {
            bool resultado = false;
            try
            {
                var primerIntento = (new Conexion()).VerificarConexion();
                var segundoIntento = (new Conexion()).VerificarConexion();
                var tercerIntento = (new Conexion()).VerificarConexion();
                if (primerIntento && segundoIntento && tercerIntento)
                {
                    return true;
                }
            }
            catch (SqlException exepcion)
            {
                ManejadorExcepciones.ManejarFatalException(exepcion);
            }
            catch (Exception exepcion)
            {
                ManejadorExcepciones.ManejarFatalException(exepcion);
            }
            return resultado;

        }

        private static string[] ExtraerValoresDeCadenaConexion(string valoresVariableGlobal)
        {
            string[] partes = valoresVariableGlobal.Split(';');
            if (partes.Length == 5)
            {
                return partes;
            }
            else
            {
                return null;
            }
        }
    }
}