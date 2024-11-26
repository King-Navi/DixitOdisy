using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Security;

namespace DAOLibreria
{
    /// <summary>
    /// Clase estática que proporciona métodos para configurar y probar la cadena de conexión a la base de datos.
    /// </summary>
    public static class ConfiguradorConexion
    {
        private static string nombreArchivoContext = "DescribeloEntities";
        private static string carpeta = "ModeloBD";
        private static string nombreArchivo = "DescribeloBD";
        /// <summary>
        /// Configura la cadena de conexión solicitando al usuario los parámetros de conexión (servidor, base de datos, usuario y contraseña),
        /// actualiza la cadena de conexión en el archivo App.config y prueba la conexión.
        /// </summary>
        /// <returns>True or Fasle, indicando si  fue exitosa TRUE o si ocurrió un error FALSE.</returns>
        public static bool ConfigurarCadenaConexion(string servidor, string nombreBD, string usuario, string contrasena)
        {
            string nuevaCadenaConexion = $"metadata=res://*/{carpeta}.{nombreArchivo}.csdl|res://*/{carpeta}.{nombreArchivo}.ssdl|res://*/{carpeta}.{nombreArchivo}.msl;" +
                                         $"provider=System.Data.SqlClient;provider connection string=\"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};MultipleActiveResultSets=True;App=EntityFramework\";";
            ActualizarCadenaConexionEnAppConfig(nombreArchivoContext, nuevaCadenaConexion);
            return ProbarConexion(servidor, nombreBD, usuario, contrasena);
        }
        /// <summary>
        /// Configura la cadena de conexión utilizando una variable de entorno que contiene el servidor, nombre de la base de datos, nombre del usuario y contraseña del usuario;
        ///  despues actualiza la cadena de conexión en el archivo App.config y prueba la conexión.
        /// Solo funciona con SQL Autentication
        /// </summary>
        /// <param name="nombreVariableEntorno">Nombre de la variable de entorno que contiene la cadena de conexión.</param>
        /// <returns>True or Fasle, indicando si  fue exitosa TRUE o si ocurrió un error FALSE.</returns>
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
                string[] valoresLista = ExtraerValoresDeCadenaConexion(valoresVariableEntorno);
                if (valoresLista == null)
                {
                    throw new Exception("Es vacio al recuperar los valores de ExtraerValoresDeCadenaConexion");
                }
                string servidor = valoresLista[0];
                string nombreBD = valoresLista[1];
                string usuario = valoresLista[2];
                string contrasena = valoresLista[3];
                // Crear la nueva cadena de conexión utilizando los valores obtenidos
                string nuevaCadenaConexion = $"metadata=res://*/{carpeta}.{nombreArchivo}.csdl|res://*/{carpeta}.{nombreArchivo}.ssdl|res://*/{carpeta}.{nombreArchivo}.msl;" +
                                             $"provider=System.Data.SqlClient;provider connection string=\"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};MultipleActiveResultSets=True;App=EntityFramework\";";

                ActualizarCadenaConexionEnAppConfig(nombreArchivoContext, nuevaCadenaConexion);

                resultado = ProbarConexion(valoresLista[0], valoresLista[1], valoresLista[2], valoresLista[3]);
            }
            catch (ArgumentNullException)
            {
                resultado = false;
            }
            catch (SecurityException)
            {
                resultado = false;
            }
            catch (Exception)
            {
                resultado = false;   
            }
            return resultado;
        }
        public static bool ConfigurarCadenaConexionRuta()
        {
            bool resultado;
            try
            {
                Console.WriteLine("Ingrese la ruta absoluta del archivo:");
                string ruta = Console.ReadLine();
                if (File.Exists(ruta))
                {
                    string contenidoArchivo = File.ReadAllText(ruta);


                    ActualizarCadenaConexionEnAppConfig(nombreArchivoContext, contenidoArchivo);
                    resultado = ProbarConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-"); //FIXME
                }
                else
                {
                    resultado = true;
                }
            }
            catch (Exception)
            {
                resultado = false;
            }
            return resultado;

        }
        /// <summary>
        /// Actualiza la cadena de conexión en el archivo App.config.
        /// Si la cadena de conexión ya existe, la actualiza; si no existe, la crea.
        /// </summary>
        /// <param name="nombreCadena">El nombre de la cadena de conexión en el archivo de configuración.</param>
        /// <param name="nuevaCadena">La nueva cadena de conexión a ser guardada.</param>
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

        /// <summary>
        /// Prueba la conexión con la base de datos utilizando los parámetros proporcionados (servidor, base de datos, usuario y contraseña).
        /// </summary>
        /// <param name="servidor">Nombre del servidor de base de datos.</param>
        /// <param name="nombreBD">Nombre de la base de datos.</param>
        /// <param name="usuario">Nombre de usuario para la conexión.</param>
        /// <param name="contrasena">Contraseña para la conexión.</param>
        /// <returns>True or Fasle, indicando si la conexión fue exitosa TRUE o si ocurrió un error FALSE.</returns>
        private static bool ProbarConexion(string servidor, string nombreBD, string usuario, string contrasena)
        {
            // Construir la cadena de conexión
            string cadenaConexionSQL = $"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};";

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexionSQL))
                {
                    conexion.Open();
                    Console.WriteLine("Conexión exitosa.");
                    return true;
                }
            }
            catch (SqlException)
            {
                Console.WriteLine($"Error de conexión");
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Extrae los valores individuales de una cadena los retorna en un array.
        /// </summary>
        /// <param name="valoresVariableGlobal">La cadena debe estar separada por " ; " (Codigo ASCII 59)</param>
        /// <returns>
        /// Un array de strings conteniendo los componentes de la cadena de conexión: servidor, nombre de base de datos,
        /// usuario y contraseña terminando con " ; " (Codigo ASCII 59), en ese orden; o null si la cadena no cumple con el formato esperado.
        /// </returns>
        /// <remarks>
        /// La cadena de conexión esperada debe estar en el formato "servidor;nombreBD;usuario;contraseña;".
        /// Este método asume que la cadena de conexión contiene exactamente cinco componentes separados por punto y coma.
        /// Si la cantidad de componentes es diferente a cinco, el método retorna null.
        /// </remarks>
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
