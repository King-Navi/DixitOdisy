using System;
using System.Collections.Generic;
using System.ServiceModel;
using UtilidadesLibreria;
using WcfServicioLibreria;

namespace WcfServidor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Dictionary<string, Object> respuesta = InicializadorConfiguracion.IniciarConexion();
            //respuesta.TryGetValue(Llaves.LLAVE_MENSAJE, out object valor);
            //Console.WriteLine((string)valor);
            Program programa = new Program();
            programa.IniciarServidor();
        }
        private void IniciarServidor()
        {
            using (ServiceHost host = new ServiceHost(typeof(WcfServicioLibreria.Manejador.ManejadorPrincipal)))
            {
                host.Open();
                Console.WriteLine("Servidor corriedo");
                Console.WriteLine("Presiona 9 tecla para atraparlo...");
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
    }
}
