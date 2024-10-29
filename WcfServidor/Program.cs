using System;
using System.Collections.Generic;
using System.ServiceModel;
using WcfServicioLibreria;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Utilidades;

namespace WcfServidor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, Object> respuesta = InicializadorConfiguracion.IniciarConexion();
            respuesta.TryGetValue(Llaves.LLAVE_MENSAJE, out object valor);
            Console.WriteLine((string)valor);
            Program programa = new Program();
            programa.IniciarServidor();
        }
        private void IniciarServidor()
        {
            var manejadorPrincipal = new ManejadorPrincipal(); // Instancia de ManejadorPrincipal

            using (ServiceHost host = new DescribeloServiceHost(manejadorPrincipal, typeof(WcfServicioLibreria.Manejador.ManejadorPrincipal)))
            {
                host.Open();
                Console.WriteLine("Servidor corriendo. Presiona una tecla para mostrar el menú...");
                Menu();
                host.Close(); // Se llama a OnClosing y luego a OnClosed automáticamente creo
            }
            //using (ServiceHost host = new ServiceHost(typeof(WcfServicioLibreria.Manejador.ManejadorPrincipal)))
            //{
            //    host.Open();
            //    Console.WriteLine("Servidor corriedo");
            //    Console.WriteLine("Presiona 9 tecla para atraparlo...");
            //    Menu();
            //    host.Close();

            //}

        }
        private void Menu()
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
