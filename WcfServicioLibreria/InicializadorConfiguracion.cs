using System;
using DAOLibreria;
namespace WcfServicioLibreria
{
    public static class InicializadorConfiguracion
    {
        public static bool IniciarConexion()
        {
            bool salir = false;

            while (!salir)
            {
                Console.WriteLine("---- Bienvenido al iniciador de conexión ----");
                Console.WriteLine("¿Qué desea hacer?");
                Console.WriteLine("1.- Comenzar configuración sin variable de entorno");
                Console.WriteLine("2.- Comenzar configuración con variable de entorno");
                Console.WriteLine("3.- Con archivo");
                Console.WriteLine("4.- Salir del iniciador");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        salir = ConfigurarSinVariable();
                        break;

                    case "2":
                        salir = ConfigurarConVariableEntorno();
                        break;

                    case "3":
                        salir = ConfigurarConArchivo();
                        break;

                    case "4":
                        Console.WriteLine("Saliendo del programa...");
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Inténtelo de nuevo.");
                        break;
                }
            }

            return salir;
        }

        private static bool ConfigurarSinVariable()
        {
            Console.WriteLine("Ingrese el nombre del servidor:");
            string servidor = Console.ReadLine();

            Console.WriteLine("Ingrese el nombre de la base de datos:");
            string nombreBD = Console.ReadLine();

            Console.WriteLine("Ingrese el nombre de usuario:");
            string usuario = Console.ReadLine();

            Console.WriteLine("Ingrese la contraseña:");
            string contrasena = Console.ReadLine();

            var resultado = ConfiguradorConexion.ConfigurarCadenaConexion(servidor, nombreBD, usuario, contrasena);
            return resultado;
        }

        private static bool ConfigurarConVariableEntorno()
        {
            Console.WriteLine("Ingrese el nombre de la variable de entorno para la conexión:");
            string nombreVariableEntorno = Console.ReadLine();

            var resultado = ConfiguradorConexion.ConfigurarCadenaConexion(nombreVariableEntorno);
            return resultado;
        }
        public static bool ConfigurarConVariableEntorno(string nombreVariableEntorno)
        {
            var resultado = ConfiguradorConexion.ConfigurarCadenaConexion(nombreVariableEntorno);
            return resultado;
        }

        private static bool ConfigurarConArchivo()
        {
            Console.WriteLine("Buscando archivo...");
            var resultado = ConfiguradorConexion.ConfigurarCadenaConexionRuta();
            return resultado;
        }

    }
}
