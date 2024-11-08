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
    /// Lógica de interacción para IngresarGamertagModalWindow.xaml
    /// </summary>
    public partial class IngresarGamertagModalWindow : Window, IActualizacionUI
    {
        public string ValorIngresado { get; private set; }
        public IngresarGamertagModalWindow()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelIngresarGamertag.Content = Properties.Idioma.labelIngresarUsuario;
            buttonAceptar.Content = Properties.Idioma.buttonAceptar;
            Title = Properties.Idioma.labelIngresarUsuario;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private void buttonAceptar_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCodigo())
            {
                ValorIngresado = textBoxCodigo.Text;
                DialogResult = true;
                this.Close();
            }
            else
            {
                labelGamertagInvalido.Visibility = Visibility.Visible;
                DialogResult = false;
            }

        }

        private bool ValidarCodigo()
        {
            return !string.IsNullOrWhiteSpace(textBoxCodigo.Text);
        }

    }
}
