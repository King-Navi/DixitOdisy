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
    public partial class MenuWindow : Window, IServicioUsuarioSesionCallback, IServicioAmistadCallback, IServicioInvitacionPartidaCallback, IActualizacionUI, IHabilitadorBotones
    {
        private DispatcherTimer timerNotificacion;
        private InvitacionPartida invitacionActual;
        private SolicitudAmistad solicitudAmistadActual;
        public MenuWindow()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
            AbrirConexiones();
            ConfigurarTimerNotificacion();
        }

        private async void AbrirConexiones()
        {
            try
            {

                var resultadoUsuarioSesion = await Conexion.AbrirConexionUsuarioSesionCallbackAsync(this);
                if (!resultadoUsuarioSesion)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, Properties.Idioma.mensajeErrorServidor, this);
                    this.Close();
                    return;
                }

                var resultadoAmigo = await Conexion.AbrirConexionAmigosCallbackAsync(amigosUserControl);
                if (!resultadoAmigo)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, Properties.Idioma.mensajeErrorServidor, this);
                    this.Close();
                    return;
                }

                var resultadoInvitacion = await Conexion.AbrirConexionInvitacionPartidaCallbackAsync(this);
                if (!resultadoInvitacion)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, Properties.Idioma.mensajeErrorServidor, this);
                    this.Close();
                    return;
                }
                Usuario user = new Usuario
                {
                    IdUsuario = Singleton.Instance.IdUsuario,
                    Nombre = Singleton.Instance.NombreUsuario
                };
                Conexion.UsuarioSesion.ObtenerSessionJugador(user);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarFatalException(excepcion,this);
            };
        }

        private void ClicBotonCrearSala(object sender, RoutedEventArgs e)
        {
            AbrirVentanaSala(null);
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
            this.Hide();
            ventanaSala.Closed += (s, args) => {
                if (!Conexion.CerrarConexionesSalaConChat())
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, Properties.Idioma.mensajeErrorServidor, this);
                    this.Close();   
                }
                this.Show(); 
            };
        }

        public void ObtenerSessionJugadorCallback(bool esSesionAbierta)
        {
            Usuario user = new Usuario
            {
                IdUsuario = Singleton.Instance.IdUsuario,
                Nombre = Singleton.Instance.NombreUsuario
            };
            Conexion.Amigos.AbrirCanalParaPeticiones(user);
            Conexion.InvitacionPartida.AbrirCanalParaInvitaciones(user);
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
                codigoSala = AbrirVentanaModal();
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
                    AbrirVentanaSala(codigoSala);
                    return;
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLobbyNoEncontrado, Properties.Idioma.mensajeLobbyNoEncontrado, this);
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

        private string AbrirVentanaModal()
        {
            string valorObtenido = null;
            UnirseSalaModalWindow ventanaModal = new UnirseSalaModalWindow();
            try
            {
                ventanaModal.Owner = this;

            }
            catch (Exception)
            {
                
            }            
            bool? resultado = ventanaModal.ShowDialog();

            if (resultado == true)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }
            

            return valorObtenido;
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
                ManejadorExcepciones.ManejarComponentErrorException(excepcion);
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
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AmigosWindow amigos = new AmigosWindow(this);
            amigos.Show();
        }

        private void ConfigurarTimerNotificacion()
        {
            timerNotificacion = new DispatcherTimer();
            timerNotificacion.Interval = TimeSpan.FromMilliseconds(50); 
            timerNotificacion.Tick += TimerNotificacion_Tick;
        }

        private void MostrarNotificacionGeneral(string mensaje, bool esSolicitudAmistad = false, string imagenPath = "")
        {
            textNotificacionGeneral.Text = mensaje;

            if (!string.IsNullOrEmpty(imagenPath))
            {
                imagenPerfil.Source = new BitmapImage(new Uri(imagenPath));
            }

            if (esSolicitudAmistad)
            {
                btnAceptar.Visibility = Visibility.Visible;
                btnRechazar.Visibility = Visibility.Visible;
                btnUnirse.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnAceptar.Visibility = Visibility.Collapsed;
                btnRechazar.Visibility = Visibility.Collapsed;
                btnUnirse.Visibility = Visibility.Visible;
            }

            borderNotificacionGeneral.Visibility = Visibility.Visible;
            progressTimerGeneral.Value = 0;
            timerNotificacion.Start();
        }

        public void RecibirInvitacion(InvitacionPartida invitacion)
        {
            invitacionActual = invitacion;
            MostrarNotificacionGeneral($"Invitación de {invitacion.GamertagEmisor} para unirse a la sala {invitacion.CodigoSala}",
                esSolicitudAmistad: false);
        }

        public void ObtenerPeticionAmistadCallback(SolicitudAmistad nuevaSolicitudAmistad)
        {
            solicitudAmistadActual = nuevaSolicitudAmistad;
            MostrarNotificacionGeneral($"Solicitud de {nuevaSolicitudAmistad.Remitente.Nombre}",
                esSolicitudAmistad: true);
        }

        private void OcultarNotificacion()
        {
            borderNotificacionGeneral.Visibility = Visibility.Collapsed;
            timerNotificacion.Stop();
        }

        private void TimerNotificacion_Tick(object sender, EventArgs e)
        {
            progressTimerGeneral.Value += 2;

            if (progressTimerGeneral.Value >= 100)
            {
                OcultarNotificacion();
            }
        }
        private void Unirse_Click(object sender, RoutedEventArgs e)
        {
            bool esInvitacion = true;
            UnirseASala(esInvitacion, invitacionActual.CodigoSala);
            OcultarNotificacion();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            _ = AceptarSolicitud(solicitudAmistadActual);
        }

        private async Task<bool> AceptarSolicitud(SolicitudAmistad solicitud)
        {
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return false;
            }

            try
            {
                var resultado = Conexion.Amigos.AceptarSolicitudAmistad(solicitudAmistadActual.Remitente.IdUsuario, Singleton.Instance.IdUsuario);
                return resultado;
            }
            catch (Exception excepcion)
            {
                //TODO manejar excepcion
                VentanasEmergentes.CrearVentanaEmergente(excepcion.InnerException.ToString(), excepcion.StackTrace, this);
                return false;
            }
        }

        private void Rechazar_Click(object sender, RoutedEventArgs e)
        {
            _ = RechazarSolicitud(solicitudAmistadActual);
        }

        private async Task<bool> RechazarSolicitud(SolicitudAmistad solicitud)
        {
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return false;
            }

            try
            {
                var resultado = Conexion.Amigos.RechazarSolicitudAmistad(solicitudAmistadActual.Remitente.IdUsuario, Singleton.Instance.IdUsuario);
                return resultado;
            }
            catch (Exception excepcion)
            {
                //TODO manejar excepcion
                VentanasEmergentes.CrearVentanaEmergente(excepcion.InnerException.ToString(), excepcion.StackTrace, this);
                return false;
            }
        }

        //TODO duda sobre interfases
        public void CambiarEstadoAmigo(Amigo amigo)
        {
            throw new NotImplementedException();
        }

        public void ObtenerAmigoCallback(Amigo amigo)
        {
            throw new NotImplementedException();
        }
    }
}
