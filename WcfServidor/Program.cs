using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using UtilidadesLibreria;
using WcfServicioLibreria;

namespace WcfServidor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, Object> respuesta = InicializadorConfiguracion.IniciarConexion();
            respuesta.TryGetValue(Llaves.LLAVE_MENSAJE, out object valor);
            Console.WriteLine((string) valor);
            Program programa = new Program();
            programa.IniciarServidor();
        }
        private void IniciarServidor()
        {
            using (ServiceHost host = new ServiceHost(typeof(WcfServicioLibreria.Manejador.ManejadorPrincipal)))
            {
                host.Open();
                Console.WriteLine("Servidor corriedo");
                Console.WriteLine("Presiona cualquier tecla para atraparlo...");
                Console.ReadLine();
                host.Close();
            }
        }
    }
}
