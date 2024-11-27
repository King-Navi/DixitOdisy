using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Security;

namespace DAOLibreria
{
    public static class ConfiguradorConexion
    {
        private static string nombreArchivoContext = "DescribeloEntities";
        private static string carpeta = "ModeloBD";
        private static string nombreArchivo = "DescribeloBD";

        public static Dictionary<string, Object> ConfigurarCadenaConexion(string servidor, string nombreBD, string usuario, string contrasena)
        {
            string nuevaCadenaConexion = $"metadata=res://*/{carpeta}.{nombreArchivo}.csdl|res://*/{carpeta}.{nombreArchivo}.ssdl|res://*/{carpeta}.{nombreArchivo}.msl;" +
                                         $"provider=System.Data.SqlClient;provider connection string=\"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};MultipleActiveResultSets=True;App=EntityFramework\";";
            ActualizarCadenaConexionEnAppConfig(nombreArchivoContext, nuevaCadenaConexion);
            return ProbarConexion(servidor, nombreBD, usuario, contrasena);
        }

        public static Dictionary<string, Object> ConfigurarCadenaConexion(string nombreVariableEntorno)
        {
            Dictionary<string, Object> resultado = new Dictionary<string, Object>();
            try
            {
                string valoresVariableEntorno = Environment.GetEnvironmentVariable(nombreVariableEntorno, EnvironmentVariableTarget.Machine);
                if (string.IsNullOrEmpty(valoresVariableEntorno))
                {
                    resultado.Add(Llaves.LLAVE_ERROR, true);
                    resultado.Add(Llaves.LLAVE_MENSAJE, $"Error: La variable de entorno '{nombreVariableEntorno}' no está configurada o está vacía.");
                    return resultado;
                }
                string[] valores = ExtraerYValidarValoresDeVariableEntorno(valoresVariableEntorno);
                if (valores == null)
                {
                    throw new Exception("Es vacio al recuperar los valores de ExtraerValoresDeCadenaConexion");
                }
                string servidor = valores[0];
                string nombreBD = valores[1];
                string usuario = valores[2];
                string contrasena = valores[3];
                string nuevaCadenaConexion = $"metadata=res://*/{carpeta}.{nombreArchivo}.csdl|res://*/{carpeta}.{nombreArchivo}.ssdl|res://*/{carpeta}.{nombreArchivo}.msl;" +
                                             $"provider=System.Data.SqlClient;provider connection string=\"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};" +
                                             $"MultipleActiveResultSets=True;App=EntityFramework\";";

                ActualizarCadenaConexionEnAppConfig(nombreArchivoContext, nuevaCadenaConexion);

                resultado = ProbarConexion(servidor, nombreBD, usuario, contrasena);
            }
            catch (ArgumentNullException excepcion)
            {
                resultado.Add(Llaves.LLAVE_ERROR, true);
                resultado.Add(Llaves.LLAVE_MENSAJE, $"Error al conectar con la base de datos(ConfigurarCadenaConexion(1)) [ArgumentNullException].  {excepcion.Message}");
            }
            catch (SecurityException excepcion)
            {
                resultado.Add(Llaves.LLAVE_ERROR, true);
                resultado.Add(Llaves.LLAVE_MENSAJE, $"Error al conectar con la base de datos (ConfigurarCadenaConexion(1)) [SecurityException].  {excepcion.Message}");
            }
            catch (Exception excepcion)
            {
                resultado.Add(Llaves.LLAVE_ERROR, true);
                resultado.Add(Llaves.LLAVE_MENSAJE, $"Error al conectar con la base de datos(ConfigurarCadenaConexion(1)) [Exception].  {excepcion.Message}");
            }
            return resultado;
        }

        public static Dictionary<string, Object> ConfigurarCadenaConexionRuta() 
        {
            Dictionary<string, Object> resultado = new Dictionary<string, Object>();
            try
            {
                Console.WriteLine("Ingrese la ruta absoluta del archivo:");
                string ruta = Console.ReadLine();

                string[] valores = ExtraerYValidarValoresDeArchivo(ruta);

                if (valores != null)
                {
                    string servidor = valores[0];
                    string nombreBD = valores[1];
                    string usuario = valores[2];
                    string contrasena = valores[3];

                    string nuevaCadenaConexion = $"metadata=res://*/{carpeta}.{nombreArchivo}.csdl|res://*/{carpeta}.{nombreArchivo}.ssdl|res://*/{carpeta}.{nombreArchivo}.msl;" +
                                             $"provider=System.Data.SqlClient;provider connection string=\"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};MultipleActiveResultSets=True;App=EntityFramework\";";

                    ActualizarCadenaConexionEnAppConfig(nombreArchivoContext, nuevaCadenaConexion);

                    resultado = ProbarConexion(servidor, nombreBD, usuario, contrasena);
                }
                else
                {
                    resultado.Add(Llaves.LLAVE_ERROR, true);
                    resultado.Add(Llaves.LLAVE_MENSAJE, $"El archivo no tiene el formato correcto: {ruta}");
                }
            }
            catch (Exception excepcion)
            {
                resultado.Add(Llaves.LLAVE_ERROR, true);
                resultado.Add(Llaves.LLAVE_MENSAJE, $"Error al conectar con la base de datos(ConfigurarCadenaConexionRuta()) [Exception]. {excepcion.Message}");
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

        private static Dictionary<String, Object> ProbarConexion(string servidor, string nombreBD, string usuario, string contrasena)
        {
            Dictionary<String, Object> resultado = new Dictionary<string, object>();
            string cadenaConexionSQL = $"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};";
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexionSQL))
                {
                    conexion.Open();
                    resultado.Add(Llaves.LLAVE_ERROR, false);
                    resultado.Add(Llaves.LLAVE_MENSAJE, "La conexión a la base de datos fue configurada y establecida exitosamente.");
                }
            }
            catch (SqlException excepcion)
            {
                resultado.Add(Llaves.LLAVE_ERROR, true);
                resultado.Add(Llaves.LLAVE_MENSAJE, $"Error al conectar con la base de datos. (ProbarConexion(4))  {excepcion.Message}");
            }
            return resultado;

        }

        private static string[] ExtraerYValidarValoresDeVariableEntorno(string valoresVariableGlobal)
        {
            string[] partes = valoresVariableGlobal.Split(';');
            try
            {
                if (partes.Length == 4)
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
                Console.WriteLine($"Error al procesar la variable de entorno: {excepcion.Message}");
            }

            return null;
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
                if (partes.Length == 4)
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
                Console.WriteLine($"Error al procesar el archivo: {excepcion.Message}");
            }

            return null;
        }
    }
    public static class Llaves
    {
        public const string LLAVE_ERROR = "error";
        public const string LLAVE_MENSAJE = "mensaje";
        public const string LLAVE_BOOLEANO = "bool";
    }
}
