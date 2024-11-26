using System;
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
    public partial class MenuWindow : Window, IServicioInvitacionPartidaCallback, IActualizacionUI, IHabilitadorBotones
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
            InicializarHiloConexion();
            InitializeComponent();
            //CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            //ActualizarUI();
            ////AbrirConexiones();
            //ConfigurarTemporizadorNotificacion();
            //InicializarEstadisticasAsync();

        }

        private void InicializarHiloConexion()
        {
            SingletonHilo.Instancia.ServidorCaido += EnServidorCaido;
            bool hiloIniciado = SingletonHilo.Instancia.Iniciar();
            if (!hiloIniciado)
            {
                Console.WriteLine("El hilo ya estaba iniciado.");
            }
        }
        private void EnServidorCaido()
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    this.IsEnabled = false;
                    SingletonGestorVentana.Instancia.CerrarVentana(Ventana.Menu);
                    SingletonGestorVentana.Instancia.AbrirNuevaVentana(Ventana.Reconectado, new ReconectandoWindow());
                }
                catch (Exception excepcion)
                {
                    Application.Current.Shutdown();
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                }
            });
        }

        private async void InicializarEstadisticasAsync()
        {
            var resutlado = await Conexion.VerificarConexionAsync(null, null);
            if (resutlado)
            {
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.Menu);
                SingletonGestorVentana.Instancia.AbrirNuevaVentana(Ventana.Reconectado, new IniciarSesion());
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

        private async void AbrirConexiones()
        {
            try
            {
                var resultadoUsuarioSesion = SingletonUsuarioSessionJugador.Instancia.AbrirConexionUsuarioSesionCallback();
                if (!resultadoUsuarioSesion)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor,
                        Properties.Idioma.mensajeErrorServidor,
                        this);
                    SingletonGestorVentana.Instancia.CerrarVentana(Ventana.Menu);
                    return;
                }
                Usuario user = new Usuario
                {
                    IdUsuario = SingletonCliente.Instance.IdUsuario,
                    Nombre = SingletonCliente.Instance.NombreUsuario
                };
                SingletonUsuarioSessionJugador.Instancia.UsuarioSesion.ObtenerSessionJugador(user);

                var resultadoAmigo = await Conexion.AbrirConexionAmigosCallbackAsync(amigosUserControl);
                if (!resultadoAmigo)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor,
                        Properties.Idioma.mensajeErrorServidor,
                        this);
                    SingletonGestorVentana.Instancia.CerrarVentana(Ventana.Menu);
                    return;
                }

                var resultado = await Conexion.Amigos?.AbrirCanalParaAmigosAsync(user);
                if (!resultado)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloCargarAmigosFalla, Idioma.mensajeCargarAmigosFalla, this);
                    Application.Current.Shutdown();
                }

                var resultadoInvitacion = await Conexion.AbrirConexionInvitacionPartidaCallbackAsync(this);
                if (!resultadoInvitacion)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor,
                        Properties.Idioma.mensajeErrorServidor,
                        this);
                    SingletonGestorVentana.Instancia.CerrarVentana(Ventana.Menu);
                    return;
                }
                await Conexion.InvitacionPartida.AbrirCanalParaInvitacionesAsync(user);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, this);
            }
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
            try
            {
                ventanaSala.Show();

            }
            catch (InvalidOperationException)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor,
                    Properties.Idioma.mensajeErrorServidor,
                    this);
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.Menu);
                return;
            }
            SingletonGestorVentana.Instancia.OcultarVentana(Ventana.Menu);
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
            try
            {
                try
                {
                    SingletonUsuarioSessionJugador.Instancia.CerrarUsuarioSesion();
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
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
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
            SingletonGestorVentana.Instancia.AbrirNuevaVentanaConVuelta(Ventana.Amigos, new AmigosWindow(), Ventana.Menu);
        }

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
        //private void TratarCerrarVentana()
        //{
        //    try
        //    {
        //        this.Close();
        //    }
        //    catch (Exception excepcion)
        //    {
        //        ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
        //    }
        //}
        //private void TratarEsconderVentana()
        //{
        //    try
        //    {
        //        this.Hide();
        //    }
        //    catch (Exception excepcion)
        //    {
        //        ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
        //    }
        //}

        //private void TratarMostrarVentana()
        //{
        //    try
        //    {
        //        this.Show();
        //    }
        //    catch (Exception excepcion)
        //    {
        //        ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
        //    }
        //}
    }
}
