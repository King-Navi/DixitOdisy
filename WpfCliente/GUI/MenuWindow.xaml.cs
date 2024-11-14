using System;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class MenuWindow : Window, IServicioUsuarioSesionCallback, IServicioInvitacionPartidaCallback, IActualizacionUI
    {
        public event Action<InvitacionPartida> InvitacionRecibida;
        public MenuWindow()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
            AbrirConexiones();
        }

        private async void AbrirConexiones()
        {
            try
            {
                //var resultadoUsuarioSesion = await Conexion.AbrirConexionUsuarioSesionCallbackAsync(this);
                //var resultadoAmigo = await Conexion.AbrirConexionAmigosCallbackAsync(amigosUserControl);
                //var resultadoInvitacion = await Conexion.AbrirConexionInvitacionPartidaCallbackAsync(this);
                //if (!resultadoAmigo || !resultadoUsuarioSesion || !resultadoUsuarioSesion)
                //{
                //    VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
                //    this.Close();
                //}

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
        }

        private async void ClicButtonUnirseSala(object sender, RoutedEventArgs e)
        {
            string codigoSala = AbrirVentanaModal();
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
            buttonSalir.IsEnabled = habilitar;
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
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponentErrorException(excepcion);
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
            buttonSalir.Content = Idioma.buttonCerrarSesion;
        }

        private void buttonSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NotificacionesWindow notificaciones = new NotificacionesWindow(this);
            notificaciones.Show();
        }

        public void RecibirInvitacion(InvitacionPartida invitacion)
        {
            InvitacionRecibida?.Invoke(invitacion);
        }
    }
}
