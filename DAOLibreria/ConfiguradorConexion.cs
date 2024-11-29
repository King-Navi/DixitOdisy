using DAOLibreria.Utilidades;
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
        /// <summary>
        /// Configura la cadena de conexión solicitando al usuario los parámetros de conexión (servidor, base de datos, usuario y contraseña),
        /// actualiza la cadena de conexión en el archivo App.config y prueba la conexión.
        /// </summary>
        /// <returns>Un diccionario con los resultados de la operación, incluyendo un mensaje y un indicador de éxito o error.</returns>
        public static Dictionary<string, Object> ConfigurarCadenaConexion(string servidor, string nombreBD, string usuario, string contrasena)
        {
            string nuevaCadenaConexion = $"metadata=res://*/{carpeta}.{nombreArchivo}.csdl|res://*/{carpeta}.{nombreArchivo}.ssdl|res://*/{carpeta}.{nombreArchivo}.msl;" +
                                         $"provider=System.Data.SqlClient;provider connection string=\"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};MultipleActiveResultSets=True;App=EntityFramework\";";
            ActualizarCadenaConexionEnAppConfig(nombreArchivoContext, nuevaCadenaConexion);
            return ProbarConexion(servidor, nombreBD, usuario, contrasena);
        /// <summary>
        /// Configura la cadena de conexión utilizando una variable de entorno que contiene el servidor, nombre de la base de datos, nombre del usuario y contraseña del usuario;
        ///  despues actualiza la cadena de conexión en el archivo App.config y prueba la conexión.
        /// Solo funciona con SQL Autentication
        /// </summary>
        /// <param name="nombreVariableEntorno">Nombre de la variable de entorno que contiene la cadena de conexión.</param>
        /// <returns>Un diccionario con los resultados de la operación, incluyendo un mensaje y un indicador de éxito o error.</returns>
        public static Dictionary<string, Object> ConfigurarCadenaConexion(string nombreVariableEntorno)
        {
            bool resultado = false;
            try
            {
                string valoresVariableEntorno = Environment.GetEnvironmentVariable(nombreVariableEntorno, EnvironmentVariableTarget.Machine);
                if (string.IsNullOrEmpty(valoresVariableEntorno))
                {
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
                ManejadorExcepciones.ManejarFatalException(excepcion);
            }
            catch (SecurityException excepcion)
            {
                resultado.Add(Llaves.LLAVE_ERROR, true);
                resultado.Add(Llaves.LLAVE_MENSAJE, $"Error al conectar con la base de datos (ConfigurarCadenaConexion(1)) [SecurityException].  {excepcion.Message}");
                ManejadorExcepciones.ManejarFatalException(excepcion);
            }
            catch (Exception excepcion)
            {
                resultado.Add(Llaves.LLAVE_ERROR, true);
                resultado.Add(Llaves.LLAVE_MENSAJE, $"Error al conectar con la base de datos(ConfigurarCadenaConexion(1)) [Exception].  {excepcion.Message}");
                ManejadorExcepciones.ManejarFatalException(excepcion);
            }
            return resultado;
        }

        public static Dictionary<string, Object> ConfigurarCadenaConexionRuta() 

        {
            bool resultado;

                string[] valores = ExtraerYValidarValoresDeArchivo(ruta);

                if (valores != null)
                    Console.WriteLine("Ingrese la ruta absoluta del archivo:");
            string ruta = Console.ReadLine();
            if (File.Exists(ruta))
            {
                string servidor = valores[0];
                string nombreBD = valores[1];
                string usuario = valores[2];
                string contrasena = valores[3];

                string nuevaCadenaConexion = $"metadata=res://*/{carpeta}.{nombreArchivo}.csdl|res://*/{carpeta}.{nombreArchivo}.ssdl|res://*/{carpeta}.{nombreArchivo}.msl;" +
                                         $"provider=System.Data.SqlClient;provider connection string=\"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};MultipleActiveResultSets=True;App=EntityFramework\";";

                ActualizarCadenaConexionEnAppConfig(nombreArchivoContext, nuevaCadenaConexion);

                    resultado.Add(Llaves.LLAVE_ERROR, true);
                    resultado.Add(Llaves.LLAVE_MENSAJE, $"El archivo no tiene el formato correcto: {ruta}");
                resultado.Add(Llaves.LLAVE_ERROR, true);
                resultado.Add(Llaves.LLAVE_MENSAJE, $"Error al conectar con la base de datos(ConfigurarCadenaConexionRuta()) [Exception]. {excepcion.Message}");
                ManejadorExcepciones.ManejarFatalException(excepcion);
            }
            catch (Exception excepcion)
            {
                resultado.Add(Llaves.LLAVE_ERROR, true);
                resultado.Add(Llaves.LLAVE_MENSAJE, $"Error al conectar con la base de datos(ConfigurarCadenaConexionRuta()) [Exception]. {excepcion.Message}");
                ManejadorExcepciones.ManejarFatalException(excepcion);
            catch (FileNotFoundException excepcion)
            {
                            ManejadorExcepciones.ManejarFatalException(excepcion);

            }
    catch(Exception excepcion)
        {
                        ManejadorExcepciones.ManejarFatalException(excepcion);

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
        private static Dictionary<String, Object> ProbarConexion(string servidor, string nombreBD, string usuario, string contrasena)
        private static bool ProbarConexion(string servidor, string nombreBD, string usuario, string contrasena)
    {
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
            catch (SqlException excepcion)
                Console.WriteLine($"Error de conexión");
                resultado.Add(Llaves.LLAVE_ERROR, true);
                resultado.Add(Llaves.LLAVE_MENSAJE, $"Error al conectar con la base de datos. (ProbarConexion(4))  {excepcion.Message}");
            }
            return resultado;
                //TODO Manejar el error
        }

        private static string[] ExtraerYValidarValoresDeVariableEntorno(string valoresVariableGlobal)
        /// <remarks>
        /// La cadena de conexión esperada debe estar en el formato "servidor;nombreBD;usuario;contraseña;".
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
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }

            return null;
    return partes;
}
            else
{
    return null;
}
        }
    }
}
