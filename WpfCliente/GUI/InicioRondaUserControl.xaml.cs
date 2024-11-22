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
        public InicioRondaUserControl()
        {
            InitializeComponent();
            Loaded += InicioRondaUserControlCargado;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
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
            var rotationAnimation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                RepeatBehavior = RepeatBehavior.Forever
            };
        }
    }
}
