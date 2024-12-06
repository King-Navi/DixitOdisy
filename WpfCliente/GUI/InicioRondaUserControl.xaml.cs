using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using WpfCliente.Interfaz;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class InicioRondaUserControl : UserControl, IActualizacionUI
    {
        private const int ANGULO_INICIO = 0;
        private const int ANGULO_FIN = 360;
        private const int DURACION_SEGUNDO = 1;
        public InicioRondaUserControl()
        {
            InitializeComponent();
            Loaded += InicioRondaUserControlCargado;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            textBlockEscogiendoNarrador.Text = Properties.Idioma.mensajeEscogiendoNarrador;
            textBlockEmpezandoRonda.Text = Properties.Idioma.mensajeEmpezandoRonda;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private void InicioRondaUserControlCargado(object sender, RoutedEventArgs e)
        {
            ComenzarAnimacionIncioRonda();
        }

        private void ComenzarAnimacionIncioRonda()
        {
            _ = new DoubleAnimation
            {
                From = ANGULO_INICIO,
                To = ANGULO_FIN,
                Duration = new Duration(TimeSpan.FromSeconds(DURACION_SEGUNDO)),
                RepeatBehavior = RepeatBehavior.Forever
            };
        }
    }
}
