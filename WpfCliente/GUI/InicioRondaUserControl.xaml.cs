using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCliente.Interfaz;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for InicioRondaUserControl.xaml
    /// </summary>
    public partial class InicioRondaUserControl : UserControl, IActualizacionUI
    {
        public InicioRondaUserControl()
        {
            InitializeComponent();
            Loaded += InicioRondaUserControlLoaded;
            //TODO: Liberar recurso
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
        }

        public void ActualizarUI()
        {
            //TODO:
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private void InicioRondaUserControlLoaded(object sender, RoutedEventArgs e)
        {
            StartRotationAnimation();
        }

        private void StartRotationAnimation()
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
