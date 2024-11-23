using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class IniciarSesion : Window, IActualizacionUI, IHabilitadorBotones
    {
        private const string RECURSOS_ESTILO_TEXTBOX_ERROR = "ErrorTextBoxStyle";
        public IniciarSesion()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }


        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelTitulo.Content = Properties.Idioma.tituloJuego;
            labelUsuario.Content = Properties.Idioma.labelUsuario;
            labelContrasenia.Content = Properties.Idioma.labelContrasenia;
            buttonOlvidarContrasenia.Content = Properties.Idioma.buttonOlvidarContrasenia;
            buttonIniciarSesion.Content = Properties.Idioma.buttonIniciarSesion;
            buttonRegistrar.Content = Properties.Idioma.buttonRegistrarse;
            buttonJugarComoInvitado.Content = Properties.Idioma.buttonJugarComoInvitado;
            this.Title = Properties.Idioma.tituloInicioSesion;
        }

        private void CerrandoVentana(object sender, EventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
        }

        public void HabilitarBotones(bool esValido)
        {
            textBoxUsuario.IsEnabled = esValido;
            passwordBoxContrasenia.IsEnabled = esValido;
            buttonIniciarSesion.IsEnabled = esValido;
            buttonRegistrar.IsEnabled = esValido;
            cambiarIdiomaUserControl.IsEnabled = esValido;
            buttonJugarComoInvitado.IsEnabled = esValido;
            buttonOlvidarContrasenia.IsEnabled= esValido;
        }

        private void ClicRegistrar(object sender, RoutedEventArgs e)
        {
            RegistrarUsuarioWindow registrarWindow = new RegistrarUsuarioWindow();
            registrarWindow.Show();
            this.Close();
        }

        private async void ClicIniciarSesion(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos())
            {
                bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
                if (!conexionExitosa)
                {
                    return;
                }
                IntentarIniciarSesion();
            }
            else
            {
                labelCredencialesIncorrectas.Visibility = Visibility.Visible;
            }
        }

        private bool ValidarCampos()
        {
            bool camposValidos = true;

            if (string.IsNullOrWhiteSpace(textBoxUsuario.Text) && textBoxUsuario.Text.Contains(" "))
            {
                textBoxUsuario.Style = (Style)FindResource(RECURSOS_ESTILO_TEXTBOX_ERROR);
                camposValidos = false;
            }

            if (string.IsNullOrWhiteSpace(passwordBoxContrasenia.Password) && passwordBoxContrasenia.Password.Contains(" "))
            {
                pwBxPasswordMask.Style = (Style)FindResource(RECURSOS_ESTILO_TEXTBOX_ERROR); 
                camposValidos = false;
            }
            return camposValidos;
        }

        private bool IntentarIniciarSesion() {
            bool exito = false;
            ServidorDescribelo.IServicioUsuario servicio = new ServidorDescribelo.ServicioUsuarioClient();
            string contraseniaHash = Encriptacion.OcuparSHA256(passwordBoxContrasenia.Password);
            Usuario resultadoUsuario = servicio.ValidarCredenciales(textBoxUsuario.Text, contraseniaHash);
            bool yaInicioSesion = servicio.YaIniciadoSesion(textBoxUsuario.Text);
            if (yaInicioSesion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloSesionIniciada, Properties.Idioma.mensajeSesionIniciada, this);
            }
            else
            {
                if (resultadoUsuario != null)
                {
                    exito = true;
                    BitmapImage imagenUsuario = Imagen.ConvertirStreamABitmapImagen(resultadoUsuario.FotoUsuario);
                    if (imagenUsuario == null)
                    {
                        VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida, Properties.Idioma.mensajeImagenInvalida, this);
                        this.Close();
                    }
                    else
                    {
                        ConfigurarSingletonConUsuario(resultadoUsuario, imagenUsuario);
                        AbrirVentanaMenu();
                    }
                }
                labelCredencialesIncorrectas.Visibility = Visibility.Visible;
            }
            return exito;
        }

        private void ConfigurarSingletonConUsuario(Usuario usuario, BitmapImage imagenUsuario)
        {
            SingletonCliente.Instance.NombreUsuario = textBoxUsuario.Text;
            SingletonCliente.Instance.IdUsuario = usuario.IdUsuario;
            SingletonCliente.Instance.FotoJugador = imagenUsuario;
            SingletonCliente.Instance.NombreUsuario = usuario.Nombre;
            SingletonCliente.Instance.Correo = usuario.Correo;
            SingletonCliente.Instance.ContraniaHash = usuario.ContraseniaHASH;
        }

        private async void ClicJugarComoInvitado(object sender, RoutedEventArgs e)
        {
            string codigoSala = VentanaModal.AbrirVentanaModalSala(this);
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }
            if (codigoSala != null)
            {
                if (ValidacionExistenciaJuego.ExisteSala(codigoSala))
                {
                    SingletonCliente.Instance.NombreUsuario = Utilidades.GenerarGamertagInvitado();
                    AbrirVentanaSala(codigoSala);
                    return;
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLobbyNoEncontrado, Properties.Idioma.mensajeLobbyNoEncontrado, this);
                }

            }
        }

        private async void AbrirVentanaSala(string idSala)
        {
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }
            SalaEsperaWindow ventanaSala = new SalaEsperaWindow(idSala);
            try
            {
                ventanaSala.Show();

            }
            catch (InvalidOperationException)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, Properties.Idioma.mensajeErrorServidor, this);
                this.Close();
                return;
            }
            ventanaSala.Closed += (s, args) => {
                if (!Conexion.CerrarConexionesSalaConChat())
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, Properties.Idioma.mensajeErrorServidor, this);
                    this.Close();
                }
                this.Show();
            };
        }

        private void AbrirVentanaMenu()
        {
            try
            {
                MenuWindow nuevaVentana = new MenuWindow();
                nuevaVentana.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarErrorException(ex,this);   
            }
        }

        private void TeclaPresionadaEnter(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonIniciarSesion.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void ClicButtonOlvidarContrasenia(object sender, RoutedEventArgs e)
        {
            OlvidarContrasenia();
        }

        private async void OlvidarContrasenia()
        {
            bool olvidoContrasenia = true;
            string correoIngresado = VentanaModal.AbrirVentanaModalCorreo(this, olvidoContrasenia);
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }
            if (ValidacionesString.EsCorreoValido(correoIngresado) && Correo.VerificarCorreo(correoIngresado,this))
            {
                string gamertag = VentanaModal.AbrirVentanaModalGamertag(this);
                bool _conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
                if (!_conexionExitosa)
                {
                    return;
                }
                if (ValidacionesString.EsGamertagValido(gamertag) && Correo.VerificarCorreoConGamertag(gamertag, correoIngresado))
                {
                    AbrirVentanaCambiarContrasenia(gamertag);
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCorreoYGamertagNoCoinciden, Properties.Idioma.mensajeCorreoYGamertagNoCoinciden, this);
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCorreoInvalido, Properties.Idioma.mensajeCorreoInvalido, this);
            }
            
        }

        private void AbrirVentanaCambiarContrasenia(string gamertag)
        {
            CambiarContraseniaWindow cambiarContraseniaWindow = new CambiarContraseniaWindow(gamertag);
            cambiarContraseniaWindow.Show();
        }
    }
}
