using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCliente.Utilidad
{
    using System;
    using System.Threading;
    using System.Windows;
    using WpfCliente.GUI;

    public class SingletonHilo
    {
        private static readonly Lazy<SingletonHilo> instancia = new Lazy<SingletonHilo>(() => new SingletonHilo());
        private Thread hilo;
        private bool ejecutando = false;
        private readonly ManualResetEvent detenerEvento = new ManualResetEvent(false);
        private const int INTERVALO_EJECUCION_SEGUNDOS = 5;
        public event Action ServidorCaido;
        public static SingletonHilo Instancia => instancia.Value;
        private SingletonHilo() { }
        public bool Iniciar()
        {
            Detener();
            ejecutando = true;
            detenerEvento.Reset();
            hilo = new Thread(Ejecutar);
            hilo.IsBackground = true;
            hilo.Start();
            return true;
        }

        private void Detener()
        {
            if (!ejecutando)
            {
                return;
            }
            ejecutando = false;
            detenerEvento.Set(); 
            hilo.Join();
        }

        private async void Ejecutar()
        {
            Console.WriteLine("Hilo en ejecución...");
            try
            {
                while (ejecutando)
                {
                    if (detenerEvento.WaitOne(TimeSpan.FromSeconds(INTERVALO_EJECUCION_SEGUNDOS)))
                    {
                        break;
                    }
                    bool conexionExitosa = await Conexion.VerificarConexionSinBaseDatosAsync();
                    if (!conexionExitosa)
                    {
                        if (ServidorCaido != null)
                        {
                            ServidorCaido.Invoke();
                        }
                        else
                        {
                            Console.WriteLine("No hay suscriptores para ServidorCaido.");
                        }
                        Detener();
                        return;
                    }
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            Console.WriteLine("Hilo finalizado.");
        }
    }

}
