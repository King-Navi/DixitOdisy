using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            Task<bool> verificarConexion = Validacion.ValidarConexion();
            HabilitarBotones(false);
            if (!await verificarConexion)
            {
                VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
                HabilitarBotones(true);
                return;
            }
            HabilitarBotones(true);


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
            string contraseniaHash = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(textBoxContrasenia.Password))).Replace("-", "");
            Usuario resultado = servicio.ValidarCredenciales(textBoxUsuario.Text, contraseniaHash);
            if (resultado != null)
            {
                exito = true;
                Singleton.Instance.NombreUsuario = textBoxUsuario.Text;
                Singleton.Instance.IdUsuario = resultado.IdUsuario;
                AbrirVentanaMenu();
            }
            labelCredencialesIncorrectas.Visibility = Visibility.Visible;
            return exito;
        }


        private void buttonJugarComoInvitado_Click(object sender, RoutedEventArgs e)
        {
            UnirseSalaModalWindow modalWindow = new UnirseSalaModalWindow();
            bool? result = modalWindow.ShowDialog();

            if (result == true)
            {
                string codigoSala = modalWindow.textBoxCodigoSala.Text;
                SalaEspera salaEspera = new SalaEspera(codigoSala);
            }
        }

        private void AbrirVentanaMenu()
        {
            MenuWindow nuevaVentana = new MenuWindow();
            nuevaVentana.Show();
            this.Close();
        }
    }
}
