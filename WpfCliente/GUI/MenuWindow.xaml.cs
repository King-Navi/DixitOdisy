using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class MenuWindow : Window, IServicioUsuarioSesionCallback, IServicioInvitacionPartidaCallback, IActualizacionUI, IHabilitadorBotones
    {
        private const int INCREMENTO_PROGRESO_BARRA = 2;
        private const int MAXIMO_TIEMPO_NOTIFICACION = 100;
        private const int LIMITE_CLICS = 2;
        private int contadorClics = 0;
        private DispatcherTimer timerNotificacion;
        private InvitacionPartida invitacionActual;
        private EstadisticaUsuario estadisticas;
        public MenuWindow()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
            _ = AbrirConexionesAsync();
            ConfigurarTemporizadorNotificacion();
            InicializarEstadisticas();
        }

        private void InicializarEstadisticas()
        {
            try
            {
                estadisticas = new EstadisticaUsuario(SingletonCliente.Instance.IdUsuario);
                textBlockPartidasGanadas.Text = estadisticas.Estadistica.PartidasGanadas.ToString();
                textBlockPartidasJugadas.Text = estadisticas.Estadistica.PartidasJugadas.ToString();
                textBlockNombre.Text = SingletonCliente.Instance.NombreUsuario;
            }
            catch (Exception excepcion) 
            {
                gridEstadisticas.Visibility = Visibility.Collapsed;
                ManejadorExcepciones.ManejarErrorExcepcion(excepcion, this);
            }
        }

        private async Task AbrirConexionesAsync()
        {
            try
            {

                var resultadoUsuarioSesion = await Conexion.AbrirConexionUsuarioSesionCallbackAsync(this);
                if (!resultadoUsuarioSesion)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, 
                        Properties.Idioma.mensajeErrorServidor, 
                        this);
                    this.Close();
                    return;
                }

                var resultadoAmigo = await Conexion.AbrirConexionAmigosCallbackAsync(amigosUserControl);
                if (!resultadoAmigo)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, 
                        Properties.Idioma.mensajeErrorServidor, 
                        this);
                    this.Close();
                    return;
                }

                var resultadoInvitacion = await Conexion.AbrirConexionInvitacionPartidaCallbackAsync(this);
                if (!resultadoInvitacion)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, 
                        Properties.Idioma.mensajeErrorServidor, 
                        this);
                    this.Close();
                    return;
                }
                Usuario user = new Usuario
                {
                    IdUsuario = SingletonCliente.Instance.IdUsuario,
                    Nombre = SingletonCliente.Instance.NombreUsuario
                };
                await Conexion.UsuarioSesion.ObtenerSessionJugadorAsync(user);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion,this);
            }
        }

        private void ClicButtonCrearSala(object sender, RoutedEventArgs e)
        {
            _ = AbrirVentanaSalaAsync(null);
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
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, 
                    Properties.Idioma.mensajeErrorServidor, 
                    this);
                this.Close();
                return;
            }
            this.Hide();
            ventanaSala.Closed += (s, args) => {
                if (!Conexion.CerrarConexionesSalaConChat())
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, 
                        Properties.Idioma.mensajeErrorServidor, 
                        this);
                    this.Close();   
                }
                this.Show(); 
            };
        }

        public async void ObtenerSessionJugadorCallback(bool esSesionAbierta)
        {
            Usuario user = new Usuario
            {
                IdUsuario = SingletonCliente.Instance.IdUsuario,
                Nombre = SingletonCliente.Instance.NombreUsuario
            };
            var resultado = await Conexion.Amigos.AbrirCanalParaPeticionesAsync(user);
            if (!resultado)
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloCargarAmigosFalla, Idioma.mensajeCargarAmigosFalla, this);
                Application.Current.Shutdown();
            }
            await Conexion.InvitacionPartida.AbrirCanalParaInvitacionesAsync(user);
        }

        private void ClicButtonUnirseSala(object sender, RoutedEventArgs e)
        {
            bool esInvitacion = false;
            string _codigoSala = null;
            _ = UnirseASalaAsync(esInvitacion, _codigoSala);
        }

        private async Task UnirseASalaAsync(bool esInvitacion, string _codigoSala)
        {
            string codigoSala = "";
            if (esInvitacion)
            {
                codigoSala = _codigoSala;
            }
            else
            {
                codigoSala = VentanasEmergentes.AbrirVentanaModalSala(this);
            }
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }
            if (codigoSala != null)
            {
                if (ValidacionExistenciaJuego.ExisteSala(codigoSala))
                {
                    await AbrirVentanaSalaAsync(codigoSala);
                    return;
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLobbyNoEncontrado, 
                        Properties.Idioma.mensajeLobbyNoEncontrado, 
                        this);
                }

            }
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            buttonCrearSala.IsEnabled = esHabilitado;
            buttonUniserSala.IsEnabled = esHabilitado;
            perfilMenuDesplegable.IsEnabled = esHabilitado;
            amigosUserControl.IsEnabled = esHabilitado;
            windowMenu.IsEnabled = esHabilitado;
        }

        private void CerrandoVentana(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
            try
            {
                Conexion.CerrarUsuarioSesion();
                Conexion.CerrarConexionesSalaConChat();
                Conexion.CerrarConexionInvitacionesPartida();
                Conexion.CerrarAmigos();
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            IniciarSesion iniciarSesion = new IniciarSesion();
            iniciarSesion.Show();
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            buttonCrearSala.Content = Idioma.buttonCrearSalaEspera;
            buttonUniserSala.Content = Idioma.buttonUnirseSalaDeEspera;
            labelEstadisitcasJugador.Content = Idioma.labelEstadisticajugador;
            labelNombreUsuario.Content = Idioma.labelNombre;
            labelJugadas.Content = Idioma.labelPartidasJugadas;
            labelGanadas.Content = Idioma.labelPartidasGanadas;
            buttonRefrescar.Content = Idioma.buttonRefrescar;
            this.Title = Properties.Idioma.tituloMenu;
        }

        private void ClicImagenAmigos(object sender, RoutedEventArgs e)
        {
            AmigosWindow amigos = new AmigosWindow();
            amigos.Closed += (s, args) =>
            {
                try
                {
                    this.Visibility = Visibility.Visible;
                }
                catch (Exception excepcion)
                {       
                    Application.Current.Shutdown();
                    ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
                }
            };
            this.Visibility = Visibility.Hidden;
            amigos.Show();
        }

        private void ConfigurarTemporizadorNotificacion()
        {
            timerNotificacion = new DispatcherTimer();
            timerNotificacion.Interval = TimeSpan.FromMilliseconds(MAXIMO_TIEMPO_NOTIFICACION); 
            timerNotificacion.Tick += ContadorNotificacion;
        }

        private void MostrarNotificacion(string mensaje)
        {
            textBlockNotificacion.Text = mensaje;
            buttonUnirse.Visibility = Visibility.Visible;

            borderNotificacion.Visibility = Visibility.Visible;
            progressTimerGeneral.Value = 0;
            timerNotificacion.Start();
        }

        public void RecibirInvitacion(InvitacionPartida invitacion)
        {
            invitacionActual = invitacion;
            MostrarNotificacion(Properties.Idioma.mensajeInvitacionPartida + " " + invitacion.GamertagEmisor);
        }

        private void OcultarNotificacion()
        {
            borderNotificacion.Visibility = Visibility.Collapsed;
            timerNotificacion.Stop();
        }

        private void ContadorNotificacion(object sender, EventArgs e)
        {
            progressTimerGeneral.Value += INCREMENTO_PROGRESO_BARRA;

            if (progressTimerGeneral.Value >= MAXIMO_TIEMPO_NOTIFICACION)
            {
                OcultarNotificacion();
            }
        }
        private async void ClicButtonUnirseInvitacionAsync(object sender, RoutedEventArgs e)
        {
            bool esInvitacion = true;
            await UnirseASalaAsync(esInvitacion, invitacionActual.CodigoSala);
            OcultarNotificacion();
        }

        private void ClicButtonAbrirEstadisticas(object sender, RoutedEventArgs e)
        {
            if(gridEstadisticas.Visibility == Visibility.Visible)
            {
                gridEstadisticas.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridEstadisticas.Visibility = Visibility.Visible;
            }
        }

        private void ClicButtonCerrarEstadisticas(object sender, RoutedEventArgs e)
        {
            gridEstadisticas.Visibility = Visibility.Collapsed;
        }

        private async void ClicButtonRefrescarEstadisticasAsync(object sender, RoutedEventArgs e)
        {
            if (contadorClics >= LIMITE_CLICS)
            {
                buttonRefrescar.Visibility = Visibility.Collapsed;
                return;
            }
            buttonRefrescar.IsEnabled = false;
            try
            {
                estadisticas.SolicitarEstadisticas(SingletonCliente.Instance.IdUsuario);
                textBlockPartidasGanadas.Text = estadisticas.Estadistica.PartidasGanadas.ToString();
                textBlockPartidasJugadas.Text = estadisticas.Estadistica.PartidasJugadas.ToString();
                textBlockNombre.Text = SingletonCliente.Instance.NombreUsuario;
                contadorClics++;
            }
            catch (Exception excepcion) 
            {
                gridEstadisticas.Visibility = Visibility.Collapsed;
                ManejadorExcepciones.ManejarErrorExcepcion(excepcion, this);
                bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
                if (!conexionExitosa)
                {
                    return;
                }
            }
            buttonRefrescar.IsEnabled = true;

        }
    }
}
