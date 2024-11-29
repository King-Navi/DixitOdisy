using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfCliente.Contexto;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class MenuPage : Page, IActualizacionUI, IHabilitadorBotones
    {
        private const int INCREMENTO_PROGRESO_BARRA = 2;
        private const int MAXIMO_TIEMPO_NOTIFICACION = 100;
        private const int TIEMPO_ESPERA_CLIC_SEGUNDOS = 10;
        private DispatcherTimer timerNotificacion;
        private InvitacionPartida invitacionActual;
        private EstadisticaUsuario estadisticas;
        public MenuPage()
        {
            SingletonHilo.Instancia.Iniciar();
            KeepAlive = true;
            this.Loaded += CargarNuevoContexto;
            InitializeComponent();
            SingletonCanal.Instancia.InvitacionRecibida += RecibirInvitacion;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
            AbrirConexiones();
            ConfigurarTemporizadorNotificacion();
            InicializarEstadisticasAsync();

        }
        private void CargarNuevoContexto(object sender, RoutedEventArgs e)
        {
            if (this.DataContext == null)
            {
                this.DataContext = this;
            }
        }


        private async void InicializarEstadisticasAsync()
        {
            var resutlado = await Conexion.VerificarConexionAsync(null, null);
            if (!resutlado)
            {
                SingletonGestorVentana.Instancia.Regresar();
                SingletonGestorVentana.Instancia.AbrirNuevaVentanaPrincipal(new ReconectandoWindow());
                return;
            }
            try
            {
                estadisticas = new EstadisticaUsuario(SingletonCliente.Instance.IdUsuario);
                textBlockPartidasGanadas.Text = estadisticas.Estadistica.PartidasGanadas.ToString();
                textBlockPartidasJugadas.Text = estadisticas.Estadistica.PartidasJugadas.ToString();
                textBlockNombre.Text = SingletonCliente.Instance.NombreUsuario;
            }
            catch (NullReferenceException excepcion)
            {
                gridEstadisticas.Visibility = Visibility.Collapsed;
                ManejadorExcepciones.ManejarErrorExcepcion(excepcion, Window.GetWindow(this));
            }
            catch (Exception excepcion)
            {
                gridEstadisticas.Visibility = Visibility.Collapsed;
                ManejadorExcepciones.ManejarErrorExcepcion(excepcion, Window.GetWindow(this));
            }
        }

        private void AbrirConexiones()
        {
            try
            {
                var resultadoUsuarioSesion = SingletonCanal.Instancia.AbrirTodaConexion();
                if (!resultadoUsuarioSesion)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor,
                        Properties.Idioma.mensajeErrorServidor,
                        Window.GetWindow(this));
                    SingletonGestorVentana.Instancia.Regresar();
                    return;
                }
                EvaluarAperturaDeCanalesAsync(resultadoUsuarioSesion);
            }
            catch(CommunicationException excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor,Properties.Idioma.mensajeErrorServidor, Window.GetWindow(this));
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, Window.GetWindow(this));
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, Window.GetWindow(this));
            }
        }

        private async void EvaluarAperturaDeCanalesAsync(bool esNecesarioAbrir)
        {
            if (!esNecesarioAbrir)
                return;
            var manejadorServico = new ServicioManejador<ServicioUsuarioClient>();
            var YaInicioSesion = manejadorServico.EjecutarServicio(llamadaServidor => 
                llamadaServidor.YaIniciadoSesion(SingletonCliente.Instance.NombreUsuario));
            if (YaInicioSesion)
            {
                return;
            }
            await EvaluarAperturaDeCanalesAsync();
        }

        private async Task EvaluarAperturaDeCanalesAsync()
        {
            Usuario user = new Usuario
            {
                IdUsuario = SingletonCliente.Instance.IdUsuario,
                Nombre = SingletonCliente.Instance.NombreUsuario
            };
            try
            {
                var resultadoSesion = await SingletonCanal.Instancia.UsuarioSesion.ObtenerSesionJugadorAsync(user);
                if (!resultadoSesion)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloErrorServidor, 
                        Idioma.mensajeErrorServidor, 
                        Window.GetWindow(this));
                    Application.Current.Shutdown();
                }
                var resultado = await SingletonCanal.Instancia.Amigos.ConectarYBuscarAmigosAsync(user);
                if (!resultado)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloCargarAmigosFalla,
                        Idioma.mensajeCargarAmigosFalla, 
                        Window.GetWindow(this));
                    Application.Current.Shutdown();
                }
            }
            catch(CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
        }

        private void ClicButtonCrearSala(object sender, RoutedEventArgs e)
        {
            AbrirVentanaSala(null);
        }

        private async void AbrirVentanaSala(string idSala)
        {
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                return;
            }
            SalaEsperaPage ventanaSala = new SalaEsperaPage(idSala);
            if (!SingletonGestorVentana.Instancia.NavegarA(ventanaSala))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor,
                    Properties.Idioma.mensajeErrorServidor,
                    Window.GetWindow(this));
                SingletonGestorVentana.Instancia.Regresar();
                return;
            }
        }

        private void ClicButtonUnirseSala(object sender, RoutedEventArgs e)
        {
            bool esInvitacion = false;
            string _codigoSala = null;
            UnirseASala(esInvitacion, _codigoSala);
        }

        private async void UnirseASala(bool esInvitacion, string _codigoSala)
        {
            string codigoSala = "";
            if (esInvitacion)
            {
                codigoSala = _codigoSala;
            }
            else
            {
                codigoSala = VentanasEmergentes.AbrirVentanaModalSala(Window.GetWindow(this));
            }
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                return;
            }
            if (codigoSala != null)
            {
                if (ValidacionExistenciaJuego.ExisteSala(codigoSala))
                {
                    AbrirVentanaSala(codigoSala);
                    return;
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLobbyNoEncontrado,
                        Properties.Idioma.mensajeLobbyNoEncontrado,
                        Window.GetWindow(this));
                }

            }
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            buttonCrearSala.IsEnabled = esHabilitado;
            buttonUniserSala.IsEnabled = esHabilitado;
            perfilMenuDesplegable.IsEnabled = esHabilitado;
            amigosUserControl.IsEnabled = esHabilitado;
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

        private void ClicButtonImagenAmigos(object sender, RoutedEventArgs e)
        {
            SingletonGestorVentana.Instancia.NavegarA(new AmigosPage());
        }

        [DebuggerStepThrough]
        private void ConfigurarTemporizadorNotificacion()
        {
            timerNotificacion = new DispatcherTimer();
            timerNotificacion.Interval = TimeSpan.FromMilliseconds(MAXIMO_TIEMPO_NOTIFICACION);
            timerNotificacion.Tick += ContadorNotificacion;
        }

        private void MostrarNotificacionGeneral(string mensaje)
        {
            textBlockNotificacionGeneral.Text = mensaje;
            buttonUnirse.Visibility = Visibility.Visible;
            borderNotificacionGeneral.Visibility = Visibility.Visible;
            progressTimerGeneral.Value = 0;
            timerNotificacion.Start();
        }

        public void RecibirInvitacion(InvitacionPartida invitacion)
        {
            invitacionActual = invitacion;
            MostrarNotificacionGeneral(Properties.Idioma.mensajeInvitacionPartida + invitacion.NombreEmisor);
        }

        private void OcultarNotificacion()
        {
            borderNotificacionGeneral.Visibility = Visibility.Collapsed;
            timerNotificacion.Stop();
        }
        [DebuggerStepThrough]
        private void ContadorNotificacion(object sender, EventArgs e)
        {
            progressTimerGeneral.Value += INCREMENTO_PROGRESO_BARRA;

            if (progressTimerGeneral.Value >= MAXIMO_TIEMPO_NOTIFICACION)
            {
                OcultarNotificacion();
            }
        }
        private void ClicButtonUnirseInvitacion(object sender, RoutedEventArgs e)
        {
            bool esInvitacion = true;
            UnirseASala(esInvitacion, invitacionActual.CodigoSala);
            OcultarNotificacion();
        }

        private void ClicButtonAbrirEstadisticas(object sender, RoutedEventArgs e)
        {
            if (gridEstadisticas.Visibility == Visibility.Visible)
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
            buttonRefrescar.IsEnabled = false;
            try
            {
                estadisticas.SolicitarEstadisiticas(SingletonCliente.Instance.IdUsuario);
                textBlockPartidasGanadas.Text = estadisticas.Estadistica.PartidasGanadas.ToString();
                textBlockPartidasJugadas.Text = estadisticas.Estadistica.PartidasJugadas.ToString();
                textBlockNombre.Text = SingletonCliente.Instance.NombreUsuario;
            }
            catch (Exception excepcion)
            {
                gridEstadisticas.Visibility = Visibility.Collapsed;
                ManejadorExcepciones.ManejarErrorExcepcion(excepcion, Window.GetWindow(this));
                bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
                if (!conexionExitosa)
                {
                    return;
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA_CLIC_SEGUNDOS)); 
            buttonRefrescar.IsEnabled = true;

        }
    }
}
