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
            cambiarIdiomaUserControl.IsEnabled = esValido;
            buttonJugarComoInvitado.IsEnabled = esValido;
            buttonOlvidarContrasenia.IsEnabled= esValido;
        }

        private void buttonRegistrar_Click(object sender, RoutedEventArgs e)
        {
            RegistrarUsuarioWindow registrarWindow = new RegistrarUsuarioWindow();
            registrarWindow.Show();
            this.Close();
        }

        private async void buttonIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            bool esValido = true;
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                HabilitarBotones(esValido);
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
                        VentanasEmergentes.CrearVentanaEmergenteImagenInvalida(this);
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


        private async void buttonJugarComoInvitado_Click(object sender, RoutedEventArgs e)
        {
            string codigoSala = AbrirVentanaModalSala();
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }
            if (codigoSala != null)
            {
                if (ValidacionExistenciaJuego.ExisteSala(codigoSala))
                {
                    Singleton.Instance.NombreUsuario = Utilidades.GenerarGamertagInvitado();
                    AbrirVentanaSala(codigoSala);
                    return;
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergenteLobbyNoEncontrado(this);
                }

            }
        }
        private string AbrirVentanaModalSala()
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

        private string AbrirVentanaModalCorreo()
        {
            string valorObtenido = null;
            bool olvidarContrasenia = true;
            VerificarCorreoModalWindow ventanaModal = new VerificarCorreoModalWindow(olvidarContrasenia);
            try
            {
                ventanaModal.Owner = this;

            }
            catch (Exception)
            {

            }
            bool? resultado = ventanaModal.ShowDialog();
            //TODO: resultado no puede ser null
            if (resultado == true && !ventanaModal.ValorIngresado.Contains(" ") && ventanaModal.ValorIngresado != null)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }


            return valorObtenido;
        }

        private string AbrirVentanaModalVerificarCorreo()
        {
            string valorObtenido = null;
            VerificarCorreoModalWindow ventanaModal = new VerificarCorreoModalWindow();
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
        private string AbrirVentanaModalGamertag()
        {
            string valorObtenido = null;
            IngresarGamertagModalWindow ventanaModal = new IngresarGamertagModalWindow();
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

        private void AbrirVentanaMenu()
        {
            try
            {
                MenuWindow nuevaVentana = new MenuWindow();
                nuevaVentana.Show();
                this.Close();
            }
            catch (Exception)
            {

                
            }
        }

        private void passwordBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonIniciarSesion.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void buttonOlvidarContrasenia_Click(object sender, RoutedEventArgs e)
        {
            OlvidarContrasenia();
        }

        private void OlvidarContrasenia()
        {
            //TODO avisarle al usuario si el correo es invalido
            string correoIngresado = AbrirVentanaModalCorreo();
            if (correoIngresado != null && Correo.VerificarCorreo(correoIngresado,this))
            {
                string gamertag = AbrirVentanaModalGamertag();
                if (gamertag != null && Correo.VerificarCorreoConGamertag(gamertag, correoIngresado))
                {
                    AbrirVentanaCambiarContrasenia(gamertag);
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergenteCorreoYGamertagNoCoinciden(this);
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergenteErrorInesperado(this);
            }
            
        }

        private void AbrirVentanaCambiarContrasenia(string gamertag)
        {
            CambiarContraseniaWindow cambiarContraseniaWindow = new CambiarContraseniaWindow(gamertag);
            cambiarContraseniaWindow.Show();
        }
    }
}
