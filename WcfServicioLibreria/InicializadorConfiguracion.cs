using System;
using System.Collections.Generic;
using DAOLibreria;
using DAOLibreria.ModeloBD;
using UtilidadesLibreria;
namespace WcfServicioLibreria
{
    public static class InicializadorConfiguracion
    {

        public static Dictionary<String, Object> IniciarConexion()
        {
            bool salir = false;
            Dictionary<String, Object> resultado = new Dictionary<String, Object>();
            object fueExitoso;
            while (!salir)
            {
                Console.WriteLine("---- Bienvenido al iniciador de conexión ----");
                Console.WriteLine("¿Qué desea hacer?");
                Console.WriteLine("1.- Comenzar configuración sin variable de entorno");
                Console.WriteLine("2.- Comenzar configuración con variable de entorno");
                Console.WriteLine("3.- Salir del iniciador");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        resultado = ConfiguradorConexion.ConfigurarCadenaConexion();
                        resultado.TryGetValue(Llaves.LLAVE_MENSAJE, out object mensaje);
                        Console.WriteLine((string)mensaje);
                        resultado.TryGetValue(Llaves.LLAVE_ERROR, out fueExitoso);

                        if (!(bool)fueExitoso)
                        {
                            salir = true;
                        }
                        break;

                    case "2":
                        Console.WriteLine("Ingrese el nombre de la variable de entorno para la conexión:");
                        string nombreVariableEntorno = Console.ReadLine();
                        resultado = ConfiguradorConexion.ConfigurarCadenaConexion(nombreVariableEntorno);
                        resultado.TryGetValue(Llaves.LLAVE_ERROR, out  fueExitoso);
                        resultado.TryGetValue(Llaves.LLAVE_MENSAJE, out mensaje);
                        Console.WriteLine((string)mensaje);
                        if (!(bool)fueExitoso)
                        {
                            salir = true;
                        }
                        break;
                    case "3":
                        Console.WriteLine("Saliendo del programa...");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Inténtelo de nuevo.");
                        break;
                        
                }
            }
            return resultado;

        }

    }
}
