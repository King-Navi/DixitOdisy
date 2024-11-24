using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class CambiarContraseniaWindow : Window, IActualizacionUI, IHabilitadorBotones
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
            windowCambiarContrasenia.Title = Properties.Idioma.tituloCambiarContrasenia; 
        }

        private void ClicButtonAceptar(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    EncriptarContrasenia(passwordBoxContrasenia.Password);
                    GuardarCambiosUsuario(usuarioEditado);
                }
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex, this);
            }
        }

        private bool ValidarCampos()
        {
            bool isValid = true;
            ColocarEstilos();

            if (!ValidarCaracteristicasContrasenia())
            {
                isValid = false;
            }

            return isValid;
        }

        private void ColocarEstilos()
        {
            labelContraseniasNoCoinciden.Visibility = Visibility.Collapsed;

            labelContraseniaMinimo.Foreground = Brushes.Red;
            labelContraseniaMaximo.Foreground = Brushes.Red;
            labelContraseniaSimbolos.Foreground = Brushes.Red;
        }

        private bool ValidarCaracteristicasContrasenia()
        {
            bool isValid = true;

            if (passwordBoxContrasenia.Password.Trim().Length >= 5)
            {
                labelContraseniaMinimo.Foreground = Brushes.Green;
            }
            else
            {
                isValid = false;
            }

            if (passwordBoxContrasenia.Password.Trim().Length <= 20)
            {
                labelContraseniaMaximo.Foreground = Brushes.Green;
            }
            else
            {
                isValid = false;
            }

            if (ValidacionesString.EsSimboloValido(passwordBoxContrasenia.Password))
            {
                labelContraseniaSimbolos.Foreground = Brushes.Green;
            }
            else
            {
                isValid = false;
            }

            if (passwordBoxContrasenia.Password != passwordBoxRepetirContrasenia.Password)
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

        private async void GuardarCambiosUsuario(Usuario usuarioEditado)
        {
            var resultadoConexion = await Conexion.VerificarConexion(HabilitarBotones,this);
            if (!resultadoConexion)
            {
                return;
            }

            var manejadorServicio = new ServicioManejador<ServicioUsuarioClient>();
            bool resultado = manejadorServicio.EjecutarServicio(proxy =>
            {
                return proxy.EditarContraseniaUsuario(usuarioEditado.Nombre,usuarioEditado.ContraseniaHASH);
            });

            if (resultado)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloEditarUsuario, Properties.Idioma.mensajeUsuarioEditadoConExito, this);
                this.Close();
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloEditarUsuario, Idioma.mensajeUsuarioEditadoFallo, this);
            }

        }
        private void ClicButtonCancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClicButtonFlechaAtras(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            buttonEditarContrasenia.IsEnabled = esHabilitado;
            buttonCancelarCambio.IsEnabled = esHabilitado;
            windowCambiarContrasenia.IsEnabled = esHabilitado;
            buttonEditarContrasenia.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA : Utilidades.OPACIDAD_MINIMA;
            buttonCancelarCambio.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA : Utilidades.OPACIDAD_MINIMA;
        }
    }

}
