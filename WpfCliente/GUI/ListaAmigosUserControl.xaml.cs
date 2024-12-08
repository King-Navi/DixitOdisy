using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.Utilidad;
using System.Timers;
using Timer = System.Timers.Timer;


namespace WpfCliente.GUI
{
    public partial class ListaAmigosUserControl : UserControl, IActualizacionUI
    {
        private Timer timer;
        private DateTime ultimaActualizacion;
        private const string FORMATO_HORA = "HH:mm:ss";
        private const int VALOR_PARA_INTERVALO = 500;
        public ListaAmigosUserControl()
        {
            InitializeComponent();
            IniciarHora();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
            try
            {
                DataContext = SingletonCanal.Instancia;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
        }

        private void IniciarHora()
        {
            timer = new Timer(VALOR_PARA_INTERVALO);
            timer.Elapsed += HoraActual;
            timer.AutoReset = true;
            timer.Start();
        }

        [DebuggerHidden]
        private void HoraActual(object sender, EventArgs e)
        {
            DateTime horaActual = DateTime.Now;

            if (horaActual.Second != ultimaActualizacion.Second)
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        labelHora.Content = horaActual.ToString(FORMATO_HORA);
                    }
                    catch (Exception excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                    }
                });
                ultimaActualizacion = horaActual;
            }
        }
        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelListaAmigos.Content = Idioma.labelListaAmigos;
        }

        private void CerrandoUserControl(object sender, RoutedEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
        }

    }
}
