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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCliente.Interfaz;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for EsperaRestoJugadoresUserControl.xaml
    /// </summary>
    public partial class EsperaRestoJugadoresUserControl : UserControl, IActualizacionUI
    {
        public EsperaRestoJugadoresUserControl()
        {
            InitializeComponent();
        }

        public void ActualizarUI()
        {
            labelEsperandoJugadores.Text = Properties.Idioma.labelEsperandoJugadores;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }
    }
}
