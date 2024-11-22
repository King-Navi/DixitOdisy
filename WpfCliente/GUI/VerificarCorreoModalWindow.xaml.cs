using System;
using System.Windows;
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

        public VerificarCorreoModalWindow(bool olvidoContrasenia)
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
            if (olvidoContrasenia) {
                labelIngresarCodigo.Content = Properties.Idioma.labelIngresarCorreo;
                textBoxCodigo.MaxLength = 100;
            }
        }


        public void ActualizarUI()
        {
            labelIngresarCodigo.Content = Properties.Idioma.labelIngresarCodigoCorreo;
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
    }
}
