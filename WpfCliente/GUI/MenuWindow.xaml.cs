using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window, IServicioUsuarioSesionCallback, IActualizacionUI
    {
        public MenuWindow()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            AbrirConexiones();
        }

        private void AbrirConexiones()
        {
            try
            {
                Conexion.AbrirConexionUsuarioSesionCallback(this);
                Conexion.AbrirConexionAmigosCallback(amigosUserControl);

                ///TODO: Este debe ser el id de usuario (esto esta hecho para pruebas cambiar a una variable en despliegue)
                Usuario user = new Usuario();
                user.IdUsuario = Singleton.Instance.IdUsuario;
                user.Nombre = Singleton.Instance.NombreUsuario;
                Conexion.UsuarioSesion.ObtenerSessionJugador(user);
                Conexion.Amigos.AbrirCanalParaPeticiones(user);
            }
            catch (Exception excepcion)
            {
                this.Close();
            };
        }

        private void ClicBotonCrearSala(object sender, RoutedEventArgs e)
        {
            //TODO: Hacer la logica para la peticion al servidor de la sala y la respuesta, este es el caso en el que el solicitante es el anfitrion
            AbrirVentanaSala(null);
        }
        private void AbrirVentanaSala(string idSala)
        {
            SalaEspera ventanaSala = new SalaEspera(idSala);
            if(ventanaSala == null || !ventanaSala.IsLoaded)
            {
                VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
                this.Close();
                return;
            }
            ventanaSala.Show();
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
            //TODO: No imitar el 418
            Console.WriteLine("Im a teapot");
        }

        private async void ClicButtonUnirseSala(object sender, RoutedEventArgs e)
        {
            Task<bool> verificarConexion = Validacion.ValidarConexion();
            HabilitarBotones(false);
            if (!await verificarConexion)
            {
                VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
                this.Close();
                
                return;
            }
            HabilitarBotones(true);
            string codigoSala = AbrirVentanaModal();
            if (Validacion.ExisteSala(codigoSala))
            {
                AbrirVentanaSala(codigoSala);
                return;
            }
            else
            {
                //TODO: I18N
                VentanasEmergentes.CrearVentanaEmergenteLobbyNoEncontrado(this);
            }


        }

        private void HabilitarBotones(bool v)
        {
            //TODO
        }

        private string AbrirVentanaModal()
        {
            string valorObtenido = null;
            UnirseSalaModalWindow ventanaModal = new UnirseSalaModalWindow();
            ventanaModal.Owner = this;
            bool? resultado = ventanaModal.ShowDialog();

            if (resultado == true)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }
            else
            {
                //TODO: I18N
                MessageBox.Show("No se ingresó ningún valor.");
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
                //TODO Manejar el error
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
            //TODO: Pedirle a unaay los .resx
        }
    }
}
