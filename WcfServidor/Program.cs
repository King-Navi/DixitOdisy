using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfServidor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Program programa = new Program();
            programa.IniciarServidor();
        }
        private void IniciarServidor()
        {
            using (ServiceHost host = new ServiceHost(typeof(WcfServicioLibreria.Manegador.Managador)))
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
