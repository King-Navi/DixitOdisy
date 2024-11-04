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
    /// Lógica de interacción para VerificarCorreoModalWindow.xaml
    /// </summary>
    public partial class VerificarCorreoModalWindow : Window, IActualizacionUI
    {
        public string ValorIngresado { get; private set; }
        public VerificarCorreoModalWindow()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelIngresarCodigo.Content = Properties.Idioma.labelIngresarCodigoCorreo;
            buttonEnviarCodigoNuevamente.Content = Properties.Idioma.buttonenviarCodigoNuevamente;
            buttonAceptar.Content = Properties.Idioma.buttonAceptar;
            Title = Properties.Idioma.tituloIngresarCodigoCorreo;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private void buttonAceptar_Click(object sender, RoutedEventArgs e)
        {
            if(ValidarCodigo()){
                ValorIngresado = textBoxCodigo.Text.ToUpper();
                DialogResult = true;
                this.Close();
            }
            else
            {
                labelCodigoInvalido.Visibility = Visibility.Visible;
                DialogResult = false;
            }

        }

        private bool ValidarCodigo()
        {
            return !string.IsNullOrWhiteSpace(textBoxCodigo.Text);
        }
        private void buttonEnviarCodigoNuevamente_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
