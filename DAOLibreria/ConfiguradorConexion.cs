using DAOLibreria.ModeloBD;
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
        
        public static bool ConfigurarCadenaConexion(string servidor, string nombreBD, string usuario, string contrasena)
        {
            string nuevaCadenaConexion = $"metadata=res://*/{carpeta}.{nombreArchivo}.csdl|res://*/{carpeta}.{nombreArchivo}.ssdl|res://*/{carpeta}.{nombreArchivo}.msl;" +
                                         $"provider=System.Data.SqlClient;provider connection string=\"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};MultipleActiveResultSets=True;App=EntityFramework\";";
            ActualizarCadenaConexionEnAppConfig(nombreArchivoContext, nuevaCadenaConexion);
            return ProbarConexion(servidor, nombreBD, usuario, contrasena);
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
                string nuevaCadenaConexion = $"metadata=res://*/{carpeta}.{nombreArchivo}.csdl|res://*/{carpeta}.{nombreArchivo}.ssdl|res://*/{carpeta}.{nombreArchivo}.msl;" +
                                             $"provider=System.Data.SqlClient;provider connection string=\"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};MultipleActiveResultSets=True;App=EntityFramework\";";

                ActualizarCadenaConexionEnAppConfig(nombreArchivoContext, nuevaCadenaConexion);

                resultado = ProbarConexion(valoresLista[0], valoresLista[1], valoresLista[2], valoresLista[3]);
            }
            catch (ArgumentNullException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (Exception)
            {
            }
            return resultado;
        }
        
        public static bool ConfigurarCadenaConexionRuta() 
        {
            bool resultado = false;
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
            }
            catch (Exception)
            {
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

        private static bool ProbarConexion(string servidor, string nombreBD, string usuario, string contrasena)
        {
            bool resultado = false;
            string cadenaConexionSQL = $"Server={servidor};Database={nombreBD};User Id={usuario};Password={contrasena};";
            try
            {
                var primerIntento =Conexion.VerificarConexion();
                var segundoIntento = Conexion.VerificarConexion();
                var tercerIntento = Conexion.VerificarConexion();
                if (primerIntento && segundoIntento && tercerIntento)
                {
                    return true;
                }
            }
            catch (SqlException)
            {
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
