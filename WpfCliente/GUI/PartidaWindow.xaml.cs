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
using System.Windows.Shapes;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for PartidaWindow.xaml
    /// </summary>
    public partial class PartidaWindow : Window , IActualizacionUI , IHabilitadorBotones, IServicioPartidaSesionCallback
    {
        public PartidaWindow(string Partida)
        {
            InitializeComponent();
        }

        public void ActualizarUI()
        {
            //TODO: recursos .resx
        }

        public void AvanzarRondaCallback()
        {
            throw new NotImplementedException();
        }

        public void HabilitarBotones(bool esValido)
        {
            throw new NotImplementedException();
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }
    }
}
