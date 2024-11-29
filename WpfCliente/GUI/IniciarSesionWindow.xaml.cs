using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class IniciarSesion : Window, IActualizacionUI, IHabilitadorBotones
    {
        private const string RECURSOS_ESTILO_TEXTBOX_ERROR = "TextBoxEstiloError";
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

        private async void ClicIniciarSesionAsync(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos())
            {
                bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
                if (!conexionExitosa)
                {
                    return;
                }
                _ = IntentarIniciarSesionAsync();
            }
            else
            {
                labelCredencialesIncorrectas.Visibility = Visibility.Visible;
            }
        }

        private bool ValidarCampos()
        {
            bool camposValidos = true;

            if (!ValidacionesString.EsGamertagValido(textBoxUsuario.Text))
            {
                textBoxUsuario.Style = (Style)FindResource(RECURSOS_ESTILO_TEXTBOX_ERROR);
                camposValidos = false;
            }

            if (string.IsNullOrWhiteSpace(passwordBoxContrasenia.Password) || passwordBoxContrasenia.Password.Contains(" "))
            {
                textBoxContrasenia.Style = (Style)FindResource(RECURSOS_ESTILO_TEXTBOX_ERROR); 
                camposValidos = false;
            }
            return camposValidos;
        }

        private async Task<bool> IntentarIniciarSesionAsync()
        {
            if (!await VerificarConexionAsync())
            {
                return false;
            }

            Usuario usuario = ValidarCredenciales();
            if (usuario == null)
            {
                MostrarMensajeCredencialesIncorrectas();
                return false;
            }

            if (VerificarSesionIniciada(usuario.Nombre))
            {
                VentanasEmergentes.CrearVentanaEmergente(
                    Properties.Idioma.tituloSesionIniciada,
                    Properties.Idioma.mensajeSesionIniciada,
                    this);
                return false;
            }

            return ConfigurarSesionYMostrarMenu(usuario);
        }

        private async Task<bool> VerificarConexionAsync()
        {
            return await Conexion.VerificarConexion(HabilitarBotones, this);
        }

        private Usuario ValidarCredenciales()
        {
            try
            {
                ServidorDescribelo.IServicioUsuario servicio = new ServidorDescribelo.ServicioUsuarioClient();
                string contraseniaHash = Encriptacion.OcuparSHA256(passwordBoxContrasenia.Password);
                return servicio.ValidarCredenciales(textBoxUsuario.Text, contraseniaHash);
            }
            catch (FaultException<VetoFalla> veto)
            {
                EvaluarExcepcion(veto);
                return null;
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex, this);
                return null;
            }
        }

        private bool VerificarSesionIniciada(string nombreUsuario)
        {
            ServidorDescribelo.IServicioUsuario servicio = new ServidorDescribelo.ServicioUsuarioClient();
            return servicio.YaIniciadoSesion(nombreUsuario);
        }

        private void MostrarMensajeCredencialesIncorrectas()
        {
            labelCredencialesIncorrectas.Visibility = Visibility.Visible;
        }

        private bool ConfigurarSesionYMostrarMenu(Usuario resultadoUsuario)
        {
            BitmapImage imagenUsuario = Imagen.ConvertirStreamABitmapImagen(resultadoUsuario.FotoUsuario);
            if (imagenUsuario == null)
            {
                VentanasEmergentes.CrearVentanaEmergente(
                    Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeImagenInvalida,
                    this);
                this.Close();
                return false;
            }

            ConfigurarSingletonConUsuario(resultadoUsuario, imagenUsuario);
            AbrirVentanaMenu();
            return true;
        }


        private void EvaluarExcepcion(FaultException<VetoFalla> veto)
        {
            if (veto.Detail.EsPermanete)
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloVetado, Idioma.mensajeVetoIndefinido, this);
            }
            if (veto.Detail.EnProgreso)
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloVetado, Idioma.mensajeVetoTemporal, this);
            }
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

        private async void ClicJugarComoInvitadoAsync(object sender, RoutedEventArgs e)
        {
            string codigoSala = VentanasEmergentes.AbrirVentanaModalSala(this);
            if (codigoSala != null)
            {
                bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
                if (!conexionExitosa)
                {
                    return;
                }
                if (ValidacionExistenciaJuego.ExisteSala(codigoSala))
                {
                    SingletonCliente.Instance.NombreUsuario = Utilidades.GenerarGamertagInvitado();
                    await AbrirVentanaSalaAsync(codigoSala);
                    this.Hide();
                    return;
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLobbyNoEncontrado, Properties.Idioma.mensajeLobbyNoEncontrado, this);
                }

            }
            else
            {
                return;
            }
        }

        private async Task AbrirVentanaSalaAsync(string idSala)
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
                ManejadorExcepciones.ManejarErrorExcepcion(ex,this);   
            }
        }

        private void TeclaPresionadaEnter(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonIniciarSesion.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private async void ClicButtonOlvidarContrasenia(object sender, RoutedEventArgs e)
        {
            await OlvidarContraseniaAsync();
        }

        private async Task OlvidarContraseniaAsync()
        {
            string correoIngresado = SolicitarCorreo();
            if (correoIngresado == null) return;

            if (!await VerificarConexionAsync()) return;

            if (!EsCorreoValido(correoIngresado))
            {
                MostrarMensajeCorreoInvalido();
                return;
            }

            string gamertag = SolicitarGamertag();
            if (gamertag == null) return;

            if (!await VerificarConexionAsync()) return;

            if (!EsGamertagYCorreoValidos(gamertag, correoIngresado))
            {
                MostrarMensajeCorreoYGamertagNoCoinciden();
                return;
            }

            AbrirVentanaCambiarContrasenia(gamertag);
        }

        private string SolicitarCorreo()
        {
            bool olvidoContrasenia = true;
            return VentanasEmergentes.AbrirVentanaModalCorreo(this, olvidoContrasenia);
        }

        private bool EsCorreoValido(string correoIngresado)
        {
            return ValidacionesString.EsCorreoValido(correoIngresado) && Correo.VerificarCorreo(correoIngresado, this);
        }

        private void MostrarMensajeCorreoInvalido()
        {
            VentanasEmergentes.CrearVentanaEmergente(
                Properties.Idioma.tituloCorreoInvalido,
                Properties.Idioma.mensajeCorreoInvalido,
                this);
        }

        private string SolicitarGamertag()
        {
            return VentanasEmergentes.AbrirVentanaModalGamertag(this);
        }

        private bool EsGamertagYCorreoValidos(string gamertag, string correoIngresado)
        {
            return ValidacionesString.EsGamertagValido(gamertag) &&
                   Correo.VerificarCorreoConGamertag(gamertag, correoIngresado);
        }

        private void MostrarMensajeCorreoYGamertagNoCoinciden()
        {
            VentanasEmergentes.CrearVentanaEmergente(
                Properties.Idioma.tituloCorreoYGamertagNoCoinciden,
                Properties.Idioma.mensajeCorreoYGamertagNoCoinciden,
                this);
        }

        private void AbrirVentanaCambiarContrasenia(string gamertag)
        {
            CambiarContraseniaWindow cambiarContraseniaWindow = new CambiarContraseniaWindow(gamertag);
            cambiarContraseniaWindow.Show();
        }
    }
}
