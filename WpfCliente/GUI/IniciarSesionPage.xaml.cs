using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfCliente.Contexto;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class IniciarSesionPage : Page,  IActualizacionUI, IHabilitadorBotones
    {
        private const string RECURSOS_ESTILO_TEXTBOX_ERROR = "ErrorTextBoxStyle";
        public IniciarSesionPage()
        {
            KeepAlive = false;
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

        private void ClicButtonRegistrar(object sender, RoutedEventArgs e)
        {
            SingletonGestorVentana.Instancia.NavegarA(new RegistrarUsuarioPage());
        }

        private async void CliButtonIniciarSesionAsync(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos())
            {
                bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
                if (!conexionExitosa)
                {
                    return;
                }
                await IntentarIniciarSesionAsync();
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

        private async Task IntentarIniciarSesionAsync() {
            try
            {
                await EvaluarConexionAsync();
                ServidorDescribelo.IServicioUsuario servicio = new ServidorDescribelo.ServicioUsuarioClient();
                string contraseniaHash = Encriptacion.OcuparSHA256(passwordBoxContrasenia.Password);
                Usuario resultadoUsuario = servicio.ValidarCredenciales(textBoxUsuario.Text, contraseniaHash);
                bool yaInicioSesion = servicio.YaIniciadoSesion(textBoxUsuario.Text);
                if (yaInicioSesion)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloSesionIniciada, 
                        Properties.Idioma.mensajeSesionIniciada, 
                        Window.GetWindow(this));
                }
                else
                {
                    if (resultadoUsuario != null)
                    {
                        BitmapImage imagenUsuario = Imagen.ConvertirStreamABitmapImagen(resultadoUsuario.FotoUsuario);
                        if (imagenUsuario == null)
                        {
                            VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida, 
                                Properties.Idioma.mensajeImagenInvalida, 
                                Window.GetWindow(this));
                        }
                        else
                        {
                            ConfigurarSingletonConUsuario(resultadoUsuario, imagenUsuario);
                            SingletonGestorVentana.Instancia.NavegarA(new MenuPage());
                            return;
                        }
                    }
                    labelCredencialesIncorrectas.Visibility = Visibility.Visible;
                }
            }
            catch (FaultException<VetoFalla> veto)
            {
                EvaluarExcepcion(veto);
                return ;
            }
            catch (ArgumentException excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                                Properties.Idioma.mensajeImagenInvalida,
                                Window.GetWindow(this));
                return;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(excepcion, Window.GetWindow(this));
            }
        }

        private async Task EvaluarConexionAsync()
        {
            var resultado = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
            if (!resultado)
            {
                throw new InvalidOperationException(nameof(EvaluarConexionAsync));
            }
        }

        private void EvaluarExcepcion(FaultException<VetoFalla> veto)
        {
            if (veto.Detail.EsPermanete)
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloVetado, Idioma.mensajeVetoIndefinido, Window.GetWindow(this));
            }
            if (veto.Detail.EnProgreso)
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloVetado, Idioma.mensajeVetoTemporal, Window.GetWindow(this));
            }
        }

        private void ConfigurarSingletonConUsuario(Usuario usuario, BitmapImage imagenUsuario)
        {
            if (usuario == null || imagenUsuario == null)
            {
                throw new ArgumentNullException("El usuario o la imagen no pueden ser nulos.");
            }
            SingletonCliente.Instance.NombreUsuario = textBoxUsuario.Text;
            SingletonCliente.Instance.IdUsuario = usuario.IdUsuario;
            SingletonCliente.Instance.FotoJugador = imagenUsuario;
            SingletonCliente.Instance.NombreUsuario = usuario.Nombre;
            SingletonCliente.Instance.Correo = usuario.Correo;
            SingletonCliente.Instance.ContraniaHash = usuario.ContraseniaHASH;
        }

        private async void ClicButtonJugarComoInvitado(object sender, RoutedEventArgs e)
        {
            string codigoSala = VentanasEmergentes.AbrirVentanaModalSala(Window.GetWindow(this));
            if (codigoSala != null)
            {
                bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
                if (!conexionExitosa)
                {
                    return;
                }
                if (ValidacionExistenciaJuego.ExisteSala(codigoSala))
                {
                    SingletonCliente.Instance.NombreUsuario = Utilidades.GenerarGamertagInvitado();
                    await AbrirVentanaSalaAsync(codigoSala);
                    return;
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLobbyNoEncontrado, Properties.Idioma.mensajeLobbyNoEncontrado, Window.GetWindow(this));
                }

            }
            else
            {
                return;
            }
        }

        private async Task AbrirVentanaSalaAsync(string idSala)
        {
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                return;
            }
            try
            {
                SalaEsperaPage ventanaSala = new SalaEsperaPage(idSala);
                var resultado = SingletonGestorVentana.Instancia.NavegarA(ventanaSala);
                if (!resultado)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, Properties.Idioma.mensajeErrorServidor, Window.GetWindow(this));
                    SingletonGestorVentana.Instancia.NavegarA(new IniciarSesionPage());
                    return;
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(excepcion, Window.GetWindow(this));
            }
            
        }

        [DebuggerStepThrough]
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
            bool olvidoContrasenia = true;
            string correoIngresado = VentanasEmergentes.AbrirVentanaModalCorreo(Window.GetWindow(this), olvidoContrasenia);
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                return;
            }
            if (ValidacionesString.EsCorreoValido(correoIngresado) && Correo.VerificarCorreo(correoIngresado,Window.GetWindow(this)))
            {
                string gamertag = VentanasEmergentes.AbrirVentanaModalGamertag(Window.GetWindow(this));
                bool _conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
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
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCorreoYGamertagNoCoinciden, Properties.Idioma.mensajeCorreoYGamertagNoCoinciden, Window.GetWindow(this));
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCorreoInvalido, Properties.Idioma.mensajeCorreoInvalido, Window.GetWindow(this));
            }
            
        }

        private void AbrirVentanaCambiarContrasenia(string gamertag)
        {
            CambiarContraseniaPage cambiarContraseniaWindow = new CambiarContraseniaPage(gamertag);
            SingletonGestorVentana.Instancia.NavegarA(cambiarContraseniaWindow);
            SingletonGestorVentana.Instancia.Regresar();
        }
    }
}
