using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
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
    public partial class MenuWindow : Window, IActualizacionUI, IHabilitadorBotones
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
            SingletonInvitacionPartida.Instancia.InvitacionRecibida += RecibirInvitacion;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
            AbrirConexiones();
            ConfigurarTemporizadorNotificacion();
            InicializarEstadisticasAsync();

        }

        private async void InicializarEstadisticasAsync()
        {
            var resutlado = await Conexion.VerificarConexionAsync(null, null);
            if (!resutlado)
            {
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.Menu);
                SingletonGestorVentana.Instancia.AbrirNuevaVentana(Ventana.Reconectado, new ReconectandoWindow());
                return;
            }
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

        private void AbrirConexiones()
        {
            try
            {
                var resultadoUsuarioSesion = SingletonUsuarioSessionJugador.Instancia.AbrirConexion();
                if (!resultadoUsuarioSesion)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor,
                        Properties.Idioma.mensajeErrorServidor,
                        this);
                    SingletonGestorVentana.Instancia.CerrarVentana(Ventana.Menu);
                    return;
                }
                EvaluarAperturaDeCanales(resultadoUsuarioSesion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, this);
            }
        }

        private void EvaluarAperturaDeCanales(bool esNecesarioAbrir)
        {
            if (!esNecesarioAbrir)
                return;
            var manejadorServico = new ServicioManejador<ServicioUsuarioClient>();
            var YaInicioSesion = manejadorServico.EjecutarServicio(llamadaServidor=> 
                llamadaServidor.YaIniciadoSesion(SingletonCliente.Instance.NombreUsuario));
            if (YaInicioSesion)
            {
                return;
            }
            EvaluarAperturaDeCanalesAsync();
        }

        private async void EvaluarAperturaDeCanalesAsync()
        {
            Usuario user = new Usuario
            {
                IdUsuario = SingletonCliente.Instance.IdUsuario,
                Nombre = SingletonCliente.Instance.NombreUsuario
            };
            SingletonUsuarioSessionJugador.Instancia.UsuarioSesion.ObtenerSessionJugador(user);

            var resultadoAmigo = SingletonAmigos.Instancia.AbrirConexion();
            if (!resultadoAmigo)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor,
                    Properties.Idioma.mensajeErrorServidor,
                    this);
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.Menu);
                return;
            }
            var resultado = await SingletonAmigos.Instancia.Amigos.AbrirCanalParaAmigosAsync(user);
            if (!resultado)
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloCargarAmigosFalla, Idioma.mensajeCargarAmigosFalla, this);
                Application.Current.Shutdown();
            }

            var resultadoInvitacion = SingletonInvitacionPartida.Instancia.AbrirConexion();
            if (!resultadoInvitacion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor,
                    Properties.Idioma.mensajeErrorServidor,
                    this);
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.Menu);
                return;
            }
            await SingletonInvitacionPartida.Instancia.InvitacionPartida.AbrirCanalParaInvitacionesAsync(user);
        }

        private void ClicButtonCrearSala(object sender, RoutedEventArgs e)
        {
            AbrirVentanaSala(null);
        }

        private async void AbrirVentanaSala(string idSala)
        {
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }
            SalaEsperaWindow ventanaSala = new SalaEsperaWindow(idSala);
            if (!SingletonGestorVentana.Instancia.AbrirNuevaVentana(Ventana.SalaEspera, ventanaSala))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor,
                    Properties.Idioma.mensajeErrorServidor,
                    this);
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.Menu);
                return;
            }
            SingletonGestorVentana.Instancia.CerrarVentana(Ventana.Menu);
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
                codigoSala = VentanasEmergentes.AbrirVentanaModalSala(this);
            }
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, this);
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
            SingletonGestorVentana.Instancia.AbrirNuevaVentana(Ventana.Amigos, new AmigosWindow());
            SingletonGestorVentana.Instancia.CerrarVentana(Ventana.Menu);
        }

        [DebuggerStepThrough]
        private void ConfigurarTemporizadorNotificacion()
        {
            timerNotificacion = new DispatcherTimer();
            timerNotificacion.Interval = TimeSpan.FromMilliseconds(MAXIMO_TIEMPO_NOTIFICACION);
            timerNotificacion.Tick += ContadorNotificacion;
        }

        private void MostrarNotificacionGeneral(string mensaje, string imagenPath = "")
        {
            textBlockNotificacionGeneral.Text = mensaje;

            if (!string.IsNullOrEmpty(imagenPath))
            {
                imagenPerfil.Source = new BitmapImage(new Uri(imagenPath));
            }
            buttonUnirse.Visibility = Visibility.Visible;

            borderNotificacionGeneral.Visibility = Visibility.Visible;
            progressTimerGeneral.Value = 0;
            timerNotificacion.Start();
        }

        public void RecibirInvitacion(InvitacionPartida invitacion)
        {
            invitacionActual = invitacion;
            MostrarNotificacionGeneral(Properties.Idioma.mensajeInvitacionPartida + invitacion.GamertagEmisor);
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

        private async void ClicButtonRefrescarEstadisticas(object sender, RoutedEventArgs e)
        {
            if (contadorClics >= LIMITE_CLICS)
            {
                buttonRefrescar.Visibility = Visibility.Collapsed;
                return;
            }
            buttonRefrescar.IsEnabled = false;
            try
            {
                estadisticas.SolicitarEstadisiticas(SingletonCliente.Instance.IdUsuario);
                textBlockPartidasGanadas.Text = estadisticas.Estadistica.PartidasGanadas.ToString();
                textBlockPartidasJugadas.Text = estadisticas.Estadistica.PartidasJugadas.ToString();
                textBlockNombre.Text = SingletonCliente.Instance.NombreUsuario;
                contadorClics++;
            }
            catch (Exception excepcion)
            {
                gridEstadisticas.Visibility = Visibility.Collapsed;
                ManejadorExcepciones.ManejarErrorExcepcion(excepcion, this);
                bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, this);
                if (!conexionExitosa)
                {
                    return;
                }
            }
            buttonRefrescar.IsEnabled = true;

        }
    }
}
