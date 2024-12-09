using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            KeepAlive = true;
            InitializeComponent();
            SingletonCanal.Instancia.InvitacionRecibida += RecibirInvitacion;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
            AbrirConexiones();
            ConfigurarTemporizadorNotificacion();
            _ = InicializarEstadisticasAsync();

        }

        private async Task InicializarEstadisticasAsync()
        {
            var resutlado = await Conexion.VerificarConexionConBaseDatosSinCierreAsync(null, null);
            if (!resutlado)
            {
                buttonAbrirEstadisticas.IsEnabled = false;
                return;
            }
            try
            {
                buttonAbrirEstadisticas.IsEnabled = true;
                estadisticas = new EstadisticaUsuario(SingletonCliente.Instance.IdUsuario);
                textBlockPartidasGanadas.Text = estadisticas.Estadistica.PartidasGanadas.ToString();
                textBlockPartidasJugadas.Text = estadisticas.Estadistica.PartidasJugadas.ToString();
                textBlockNombre.Text = SingletonCliente.Instance.NombreUsuario;
            }
            catch (NullReferenceException excepcion)
            {
                gridEstadisticas.Visibility = Visibility.Collapsed;
                ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
            }
            catch (Exception excepcion)
            {
                gridEstadisticas.Visibility = Visibility.Collapsed;
                ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
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
                    SingletonGestorVentana.Instancia.NavegarA(new IniciarSesionPage());
                    SingletonGestorVentana.Instancia.LimpiarHistorial();
                    return;
                }
                _ = EvaluarAperturaDeCanalesAsync(resultadoUsuarioSesion);
            }
            catch(CommunicationException excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor,Properties.Idioma.mensajeErrorServidor, Window.GetWindow(this));
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion, Window.GetWindow(this));
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion, Window.GetWindow(this));
            }
        }

        private async Task EvaluarAperturaDeCanalesAsync(bool esNecesarioAbrir)
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
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }

        private async void ClicButtonCrearSalaAsync(object sender, RoutedEventArgs e)
        {
            await AbrirVentanaSalaAsync(null);
        }

        private async Task AbrirVentanaSalaAsync(string idSala)
        {
            try
            {
                bool conexionExitosa = await Conexion.VerificarConexionSinBaseDatosAsync(HabilitarBotones, Window.GetWindow(this));
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
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }

        private async void ClicButtonUnirseSalaAsync(object sender, RoutedEventArgs e)
        {
            bool esInvitacion = false;
            string codigoSala = null;
            await UnirseASalaAsync(esInvitacion, codigoSala);
        }

        private async Task UnirseASalaAsync(bool esInvitacion, string codigoSalaParametro)
        {
            string codigoSala = "";
            if (esInvitacion)
            {
                codigoSala = codigoSalaParametro;
            }
            else
            {
                codigoSala = VentanasEmergentes.AbrirVentanaModalSala(Window.GetWindow(this));
            }
            try
            {
                bool conexionExitosa = await Conexion.VerificarConexionSinBaseDatosAsync(HabilitarBotones, Window.GetWindow(this));
                if (!conexionExitosa)
                {
                    return;
                }
                if (codigoSala != null)
                {
                    if (ValidacionExistenciaJuego.ExisteSala(codigoSala))
                    {
                        await AbrirVentanaSalaAsync(codigoSala);
                    }
                    else
                    {
                        VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLobbyNoEncontrado,
                            Properties.Idioma.mensajeLobbyNoEncontrado,
                            Window.GetWindow(this));
                    }
                }
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            buttonCrearSala.IsEnabled = esHabilitado;
            buttonUniserSala.IsEnabled = esHabilitado;
            perfilMenuDesplegable.IsEnabled = esHabilitado;
            listaAmigosUserControl.IsEnabled = esHabilitado;
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
            borderNotificacion.Visibility = Visibility.Visible;
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
            borderNotificacion.Visibility = Visibility.Collapsed;
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
        private async void ClicButtonUnirseInvitacionAsync(object sender, RoutedEventArgs e)
        {
            bool esInvitacion = true;
            await UnirseASalaAsync(esInvitacion, invitacionActual.CodigoSala);
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
                await estadisticas.SolicitarEstadisticasAsync(SingletonCliente.Instance.IdUsuario);
                textBlockPartidasGanadas.Text = estadisticas.Estadistica.PartidasGanadas.ToString();
                textBlockPartidasJugadas.Text = estadisticas.Estadistica.PartidasJugadas.ToString();
                textBlockNombre.Text = SingletonCliente.Instance.NombreUsuario;
            }
            catch (Exception excepcion)
            {
                gridEstadisticas.Visibility = Visibility.Collapsed;
                ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
                bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
                if (!conexionExitosa)
                {
                    return;
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA_CLIC_SEGUNDOS)); 
            buttonRefrescar.IsEnabled = true;

        }

        private void CerrandoPage(object sender, RoutedEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
        }

    }
}
