using System;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class MenuWindow : Window, IServicioUsuarioSesionCallback, IServicioInvitacionPartidaCallback, IActualizacionUI
    {
        private DispatcherTimer timerNotificacion;
        private InvitacionPartida invitacionActual;
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
                    VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
                    this.Close();
                    return;
                }

                var resultadoAmigo = await Conexion.AbrirConexionAmigosCallbackAsync(amigosUserControl);
                if (!resultadoAmigo)
                {
                    VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
                    this.Close();
                    return;
                }

                var resultadoInvitacion = await Conexion.AbrirConexionInvitacionPartidaCallbackAsync(this);
                if (!resultadoInvitacion)
                {
                    VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
                    this.Close();
                    return;
                }
                Usuario user = new Usuario
                {
                    IdUsuario = Singleton.Instance.IdUsuario,
                    Nombre = Singleton.Instance.NombreUsuario
                };
                //TODO: Hay que verificar que no haya iniciado sesion antes
                Conexion.UsuarioSesion.ObtenerSessionJugador(user);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarFatalException(excepcion,this);
            };
        }

        private void ClicBotonCrearSala(object sender, RoutedEventArgs e)
        {
            //TODO: Hacer la logica para la peticion al servidor de la sala y la respuesta, este es el caso en el que el solicitante es el anfitrion
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
                VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
                this.Close();
                return;
            }
            this.Hide();
            ventanaSala.Closed += (s, args) => {
                if (!Conexion.CerrarConexionesSalaConChat())
                {
                    VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
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
                if (Validacion.ExisteSala(codigoSala))
                {
                    AbrirVentanaSala(codigoSala);
                    return;
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergenteLobbyNoEncontrado(this);
                }

            }
        }

        private void HabilitarBotones(bool habilitar)
        {
            buttonCrearSala.IsEnabled = habilitar;
            buttonUniserSala.IsEnabled = habilitar;
            perfilMenuDesplegable.IsEnabled = habilitar;
            amigosUserControl.IsEnabled = habilitar;
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
            NotificacionesWindow notificaciones = new NotificacionesWindow(this);
            notificaciones.Show();
        }

        private void ConfigurarTimerNotificacion()
        {
            timerNotificacion = new DispatcherTimer();
            timerNotificacion.Interval = TimeSpan.FromMilliseconds(50); // Actualización del progreso
            timerNotificacion.Tick += TimerNotificacion_Tick;
        }

        public void RecibirInvitacion(InvitacionPartida invitacion)
        {
            invitacionActual = invitacion;
            MostrarNotificacion($"Invitación de {invitacion.GamertagEmisor} para unirse a la sala {invitacion.CodigoSala}");
        }

        private void MostrarNotificacion(string mensaje)
        {
            textNotificacionInvitacion.Text = mensaje;
            borderNotificacionInvitacion.Visibility = Visibility.Visible;
            progressTimer.Value = 0; 
            timerNotificacion.Start();
        }

        private void OcultarNotificacion()
        {
            borderNotificacionInvitacion.Visibility = Visibility.Collapsed;
            timerNotificacion.Stop();
        }

        private void TimerNotificacion_Tick(object sender, EventArgs e)
        {
            progressTimer.Value += 2; 

            if (progressTimer.Value >= 100)
            {
                OcultarNotificacion();
            }
        }

        private void UnirseSala_Click(object sender, RoutedEventArgs e)
        {
            bool esInvitacion = true;
            UnirseASala(esInvitacion, invitacionActual.CodigoSala);
            OcultarNotificacion();
        }


    }
}
