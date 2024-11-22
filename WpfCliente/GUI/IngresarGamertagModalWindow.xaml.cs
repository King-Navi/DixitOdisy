using System;
using System.Windows;
using WpfCliente.Interfaz;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
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

        private void ClicButtonAceptar(object sender, RoutedEventArgs e)
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
