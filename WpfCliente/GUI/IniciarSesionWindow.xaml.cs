using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class IniciarSesion : Window, IActualizacionUI
    {

        public IniciarSesion()
        {
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

        
        private void EnCierre(object sender, EventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
        }

        private void HabilitarBotones(bool esValido)
        {
            textBoxUsuario.IsEnabled = esValido;
            textBoxContrasenia.IsEnabled = esValido;
            buttonIniciarSesion.IsEnabled = esValido;
            buttonRegistrar.IsEnabled = esValido;
        }

        private void buttonRegistrar_Click(object sender, RoutedEventArgs e)
        {
            RegistrarUsuarioWindow registrarWindow = new RegistrarUsuarioWindow();
            registrarWindow.Show();
            this.Close();
        }

        private async void buttonIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }
            if (ValidarCampos())
            {
                TryIniciarSesion();

            }
            else
            {
                labelCredencialesIncorrectas.Visibility = Visibility.Visible;
            }
        }

        private bool ValidarCampos()
        {
            bool camposValidos = true;

            if (string.IsNullOrWhiteSpace(textBoxUsuario.Text) && textBoxUsuario.Text.Contains(" "))
            {
                textBoxUsuario.Style = (Style)FindResource("ErrorTextBoxStyle");
                camposValidos = false;
            }

            if (string.IsNullOrWhiteSpace(textBoxContrasenia.Password) && textBoxContrasenia.Password.Contains(" "))
            {
                pwBxPasswordMask.Style = (Style)FindResource("ErrorTextBoxStyle");
                camposValidos = false;
            }
            return camposValidos;
        }

        private bool TryIniciarSesion() {
            bool exito = false;
            ServidorDescribelo.IServicioUsuario servicio = new ServidorDescribelo.ServicioUsuarioClient();
            string contraseniaHash = Encriptacion.OcuparSHA256(textBoxContrasenia.Password);
            Usuario resultadoUsuario = servicio.ValidarCredenciales(textBoxUsuario.Text, contraseniaHash);
            bool yaInicioSesion = servicio.YaIniciadoSesion(textBoxUsuario.Text);
            if (yaInicioSesion)
            {
                VentanasEmergentes.CrearVentanaEmergente("Ya has iniciado sesion",
                    "Ya has iniciado sesion, si no eres tu el que inicio sesion porfavor contacta a soporte", this);
            }
            else
            {
                if (resultadoUsuario != null)
                {
                    exito = true;
                    Singleton.Instance.NombreUsuario = textBoxUsuario.Text;
                    Singleton.Instance.IdUsuario = resultadoUsuario.IdUsuario;
                    BitmapImage imagenUsuario = Imagen.ConvertirStreamABitmapImagen(resultadoUsuario.FotoUsuario);
                    if (imagenUsuario == null)
                    {
                        MessageBox.Show("Error al cargar su imagen poravor cambiela");
                        this.Close();
                    }
                    Singleton.Instance.FotoJugador = imagenUsuario;
                    Singleton.Instance.NombreUsuario = resultadoUsuario.Nombre;
                    Singleton.Instance.Correo = resultadoUsuario.Correo;
                    Singleton.Instance.ContraniaHash = resultadoUsuario.ContraseniaHASH;
                    AbrirVentanaMenu();
                }
                labelCredencialesIncorrectas.Visibility = Visibility.Visible;
            }
            return exito;
        }


        private void buttonJugarComoInvitado_Click(object sender, RoutedEventArgs e)
        {
            //FIXME
            //UnirseSalaModalWindow modalWindow = new UnirseSalaModalWindow();
            //bool? result = modalWindow.ShowDialog();

            //if (result == true)
            //{
            //    string codigoSala = modalWindow.textBoxCodigoSala.Text;
            //    SalaEsperaWindow salaEspera = new SalaEsperaWindow(codigoSala);
            //}
        }

        private void AbrirVentanaMenu()
        {
            MenuWindow nuevaVentana = new MenuWindow();
            nuevaVentana.Show();
            this.Close();
        }

        private void passwordBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonIniciarSesion.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
