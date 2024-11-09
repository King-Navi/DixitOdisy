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
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para CambiarContraseniaWindow.xaml
    /// </summary>
    public partial class CambiarContraseniaWindow : Window, IActualizacionUI
    {
        Usuario usuarioEditado = new Usuario();
        public CambiarContraseniaWindow(string gamertag)
        {
            InitializeComponent();
            usuarioEditado.Nombre = gamertag;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            
            labelRepetirContrasenia.Content = Properties.Idioma.labelRepitaContraseña;
            labelContrasenia.Content = Properties.Idioma.labelContrasenia;
            labelContraseniaInstruccion.Content = Properties.Idioma.labelContraseniaInstruccion;
            labelContraseniaMinimo.Content = Properties.Idioma.labelContraseniaMinimo;
            labelContraseniaMaximo.Content = Properties.Idioma.labelContraseniaMaximo;
            labelContraseniaSimbolos.Content = Properties.Idioma.labelContraseniaSimbolos;
            buttonEditarContrasenia.Content = Properties.Idioma.buttonCambiarContrasenia;
            buttonCancelarCambio.Content = Properties.Idioma.buttonCancelar;
        }

        private void clicButtonAceptar(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos()){
                EncriptarContrasenia(textBoxContrasenia.Password);
                GuardarCambiosUsuario(usuarioEditado);
            }
        }

        private bool ValidarCampos()
        {
            bool isValid = true;
            SetDefaultStyles();

            if (!ValidarCaracteristicasContrasenia())
            {
                isValid = false;
            }

            return isValid;
        }

        private void SetDefaultStyles()
        {
            labelContraseniasNoCoinciden.Visibility = Visibility.Collapsed;

            labelContraseniaMinimo.Foreground = Brushes.Red;
            labelContraseniaMaximo.Foreground = Brushes.Red;
            labelContraseniaSimbolos.Foreground = Brushes.Red;
        }

        private bool ValidarCaracteristicasContrasenia()
        {
            bool isValid = true;

            if (textBoxContrasenia.Password.Trim().Length >= 5)
            {
                labelContraseniaMinimo.Foreground = Brushes.Green;
            }
            else
            {
                isValid = false;
            }

            if (textBoxContrasenia.Password.Trim().Length <= 20)
            {
                labelContraseniaMaximo.Foreground = Brushes.Green;
            }
            else
            {
                isValid = false;
            }

            if (ValidacionesString.IsValidSymbol(textBoxContrasenia.Password))
            {
                labelContraseniaSimbolos.Foreground = Brushes.Green;
            }
            else
            {
                isValid = false;
            }

            if (textBoxContrasenia.Password != textBoxRepetirContrasenia.Password)
            {
                labelContraseniasNoCoinciden.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                labelContraseniasNoCoinciden.Visibility = Visibility.Collapsed;
            }

            return isValid;
        }

        private void EncriptarContrasenia(string contrasenia)
        {
            usuarioEditado.ContraseniaHASH = Encriptacion.OcuparSHA256(contrasenia);
        }

        private void GuardarCambiosUsuario(Usuario usuarioEditado)
        {

            var manejadorServicio = new ServicioManejador<ServicioUsuarioClient>();
            bool resultado = manejadorServicio.EjecutarServicio(proxy =>
            {
                return proxy.EditarContraseniaUsuario(usuarioEditado.Nombre,usuarioEditado.ContraseniaHASH);
            });

            if (resultado)
            {
                VentanasEmergentes.CrearVentanaEmergenteDatosEditadosExito(this);
                this.Close();
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloEditarUsuario, Idioma.mensajeUsuarioEditadoFallo, this);
            }

        }
        private void clicButtonCancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        
    }

}
