using System;
using System.Windows;
using WpfCliente.Interfaz;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class VerificarCorreoModalWindow : Window, IActualizacionUI
    {
        public string ValorIngresado { get; private set; }
        public const int MAXIMOS_CARACTERES = 50;
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
                MostrarIngresarCorreo();
            }
        }


        public void ActualizarUI()
        {
            this.Title = Properties.Idioma.tituloIngresarCodigoCorreo;
            labelInstruccionesVerificacionCorreo.Content = Properties.Idioma.labelInstruccionesVerificacionCorreo;
            labelIngresarCodigo.Content = Properties.Idioma.labelIngresarCodigoCorreo;
            labelCodigoInvalido.Content = Properties.Idioma.labelInvalido;
            buttonAceptar.Content = Properties.Idioma.buttonAceptar;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private void ClicButtonAceptar(object sender, RoutedEventArgs e)
        {
            if(ValidarCodigo()){
                ValorIngresado = textBoxCodigo.Text.ToUpper();
                DialogResult = true;
                TratarCerrarVentana();
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

        private void MostrarIngresarCorreo()
        {
            this.Title = Properties.Idioma.tituloIngresarCorreo;
            labelIngresarCodigo.Content = Properties.Idioma.labelIngresarCorreo;
            labelCodigoInvalido.Visibility = Visibility.Collapsed;
            labelInstruccionesVerificacionCorreo.Visibility = Visibility.Collapsed;
            textBoxCodigo.MaxLength = MAXIMOS_CARACTERES;
        }
        private void TratarCerrarVentana()
        {
            try
            {
                this.Close();
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
        }
    }
}
