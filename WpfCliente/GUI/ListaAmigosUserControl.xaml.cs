using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class ListaAmigosUserControl : UserControl, IActualizacionUI
    {
        private bool desechado = false;
        private DispatcherTimer timer;
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
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(VALOR_PARA_INTERVALO);
            timer.Tick += HoraActual;
            timer.Start();
        }

        [DebuggerStepThrough]
        private void HoraActual(object sender, EventArgs e)
        {
            DateTime horaActual = DateTime.Now;
            if (horaActual.Second != ultimaActualizacion.Second)
            {
                labelHora.Content = horaActual.ToString(FORMATO_HORA);
                ultimaActualizacion = horaActual;
            }
        }

        private void Desechar()
        {
            if (desechado) return;
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= HoraActual;
                timer = null;
            }
            desechado = true;
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
