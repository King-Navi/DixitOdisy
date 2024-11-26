using System;
using System.ServiceModel;
using WcfServicioLibreria;
using WcfServicioLibreria.Manejador;

namespace WcfServidor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var respuesta = InicializadorConfiguracion.IniciarConexion();
            //Console.WriteLine(respuesta.ToString());
            Program programa = new Program();
            programa.IniciarServidor();
        }
        private void IniciarServidor()
        {
            var manejadorPrincipal = new ManejadorPrincipal();

            using (ServiceHost host = new DescribeloServiceHost(manejadorPrincipal, typeof(WcfServicioLibreria.Manejador.ManejadorPrincipal)))
            {
                host.Open();
                Console.WriteLine("Servidor corriendo. Presiona una 9 para mostrar el atraparlo...");
                Menu(manejadorPrincipal);
                host.Close(); 
            }
        }
        private void Menu(ManejadorPrincipal manejadorPrincipal)
        {
            bool condicionSalida = true;

            while (condicionSalida)
            {
                Console.WriteLine("Digite un numero");
                Console.WriteLine("1.- Ver usuarios conectados");
                Console.WriteLine("9.- Salir");
                string respuesta = Console.ReadLine();
                switch (respuesta)
                {
                    case "1":
                        manejadorPrincipal.JugadoresConectado();
                        break;
                    case "9":
                        condicionSalida = false;
                        break;
                    default:
                        Console.WriteLine("No ingresaste un valor valido.");
                        break;
                }
            }
        }
        public static class Llaves
        {
            public const string LLAVE_ERROR = "error";
            public const string LLAVE_MENSAJE = "mensaje";
            public const string LLAVE_BOOLEANO = "bool";
        }
    }
}
