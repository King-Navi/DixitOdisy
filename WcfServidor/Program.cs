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
            var respuesta = InicializadorConfiguracion.IniciarConexion();
            if (!respuesta)
            {
                return;
            }
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
                Menu();
                host.Close(); 
            }
        }
        private void Menu()
        {
            bool condicionSalida = true;

            while (condicionSalida)
            {
                Console.WriteLine("Digite un numero");
                Console.WriteLine("9.- Salir");
                string respuesta = Console.ReadLine();
                switch (respuesta)
                {
                    case "9":
                        condicionSalida = false;
                        break;
                    default:
                        Console.WriteLine("No ingresaste un valor valido.");
                        break;
                }
            }
        }
    }
}
