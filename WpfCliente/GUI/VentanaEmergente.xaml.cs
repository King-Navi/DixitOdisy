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
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Se utiliza para reutilizar esta ventana emergente para distintos casos en los que algo falla
    /// </summary>
    public partial class VentanaEmergente : Window, IActualizacionUI
    {

        public VentanaEmergente(string titulo, string descripcion)
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();

            labelTituloVentanaEmergente.Content = titulo;
            textBlockDescripcionVentanaEmergente.Text = descripcion;

        }

        public VentanaEmergente()
        {
            InitializeComponent();
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            buttonAceptar.Content = Properties.Idioma.buttonAceptar;
        }


        private void buttonAceptar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
