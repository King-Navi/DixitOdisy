using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Security;
using UtilidadesLibreria;

namespace DAOLibreria
{
    /// <summary>
    /// Clase estática que proporciona métodos para configurar y probar la cadena de conexión a la base de datos.
    /// </summary>
    public static class ConfiguradorConexion
    {
        /// <summary>
        /// Este es el nombre de la carpeta que contiene la base de datos
        /// </summary>
        private static string carpeta = "ModeloBD";
        /// <summary>
        /// Esta es el nombre del archivo de la base de datos (el que se genera con .edmx)
        /// </summary>
        private static string nombreArchivo = "ModeloBaseDatos";
        /// <summary>
        /// Configura la cadena de conexión solicitando al usuario los parámetros de conexión (servidor, base de datos, usuario y contraseña),
        /// actualiza la cadena de conexión en el archivo App.config y prueba la conexión.
        /// </summary>
        /// <returns>Un diccionario con los resultados de la operación, incluyendo un mensaje y un indicador de éxito o error.</returns>
        public static Dictionary<string, Object> ConfigurarCadenaConexion()
        {
            Dictionary<string, Object> resultado = new Dictionary<string, Object>();
            Console.WriteLine("Ingrese el nombre del servidor:");
            string servidor = Console.ReadLine();
            Console.WriteLine("Ingrese el nombre de la base de datos:");
            string nombreBD = Console.ReadLine();
            Console.WriteLine("Ingrese el nombre de usuario:");
            string usuario = Console.ReadLine();
            Console.WriteLine("Ingrese la contraseña:");
            string contrasena = Console.ReadLine();
            string nuevaCadenaConexion = $"metadata=res://*/{carpeta}.{nombreArchivo}.csdl|res://*/{carpeta}.{nombreArchivo}.ssdl|res://*/{carpeta}.{nombreArchivo}.msl;" +
                                         $"provider=System.Data.SqlClient;provider connection string=\"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};MultipleActiveResultSets=True;App=EntityFramework\";";
            ActualizarCadenaConexionEnAppConfig("bd1Entities", nuevaCadenaConexion);
            resultado = ProbarConexion(servidor, nombreBD, usuario, contrasena);
            return resultado;
        }
        /// <summary>
        /// Configura la cadena de conexión utilizando una variable de entorno que contiene el servidor, nombre de la base de datos, nombre del usuario y contraseña del usuario;
        ///  despues actualiza la cadena de conexión en el archivo App.config y prueba la conexión.
        /// Solo funciona con SQL Autentication
        /// </summary>
        /// <param name="nombreVariableEntorno">Nombre de la variable de entorno que contiene la cadena de conexión.</param>
        /// <returns>Un diccionario con los resultados de la operación, incluyendo un mensaje y un indicador de éxito o error.</returns>
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

                ActualizarCadenaConexionEnAppConfig("bd1Entities", nuevaCadenaConexion);

                resultado = ProbarConexion(valoresLista[0], valoresLista[1], valoresLista[2], valoresLista[3]);
            }
            catch (ArgumentNullException ex)
            {
                // TODO Manejar la excepción si el nombre de la variable de entorno es nulo
                resultado.Add(Llaves.LLAVE_ERROR, true);
                resultado.Add(Llaves.LLAVE_MENSAJE, $"Error al conectar con la base de datos(ConfigurarCadenaConexion(1)) [ArgumentNullException].  {ex.Message}");
            }
            catch (SecurityException ex)
            {
                //TODO Manejar la excepción si no se tiene permiso para acceder a la variable de entorno
                resultado.Add(Llaves.LLAVE_ERROR, true);
                resultado.Add(Llaves.LLAVE_MENSAJE, $"Error al conectar con la base de datos (ConfigurarCadenaConexion(1)) [SecurityException].  {ex.Message}");
            }
            catch (Exception ex)
            {
                //TODO Manejar cualquier otra excepción 
                resultado.Add(Llaves.LLAVE_ERROR, true);
                resultado.Add(Llaves.LLAVE_MENSAJE, $"Error al conectar con la base de datos(ConfigurarCadenaConexion(1)) [Exception].  {ex.Message}");
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
        /// <returns>Un diccionario con los resultados de la operación, indicando si la conexión fue exitosa o si ocurrió un error.</returns>
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
            catch (SqlException ex)
            {
                //TODO Manejar el error
                resultado.Add(Llaves.LLAVE_ERROR, true);
                resultado.Add(Llaves.LLAVE_MENSAJE, $"Error al conectar con la base de datos. (ProbarConexion(4))  {ex.Message}");
            }
            return resultado;

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
                string servidor = partes[0];  
                string nombreBD = partes[1];  
                string usuario = partes[2];    
                string contrasena = partes[3]; 
                return partes;
            }
            else
            {
                return null;
            }
        }
    }
}
