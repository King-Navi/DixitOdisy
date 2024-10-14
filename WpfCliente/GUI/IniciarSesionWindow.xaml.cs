using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using UtilidadesLibreria;
using WpfCliente.Interfaz;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class IniciarSesion : Window, IActualizacionUI
    {
        private const string FUENTE_SECUNDARIA = "Arial";

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

        private void AbrirVentanaMenu() 
        {
            MenuWindow nuevaVentana = new MenuWindow();
            nuevaVentana.Show();
            this.Close();
        }

        private void buttonRegistrar_Click(object sender, RoutedEventArgs e)
        {
            RegistrarUsuario registrarWindow = new RegistrarUsuario();
            registrarWindow.Show();
            this.Close();
        }

        private async void buttonIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            Task<bool> verificarConexion = Validacion.ValidarConexion();
            HabilitarBotones(false);
            if (!await verificarConexion)
            {
                ErrorConexionModalWindow ventanaModal = new ErrorConexionModalWindow();
                ventanaModal.Owner = this;
                ventanaModal.ShowDialog();
                HabilitarBotones(true);
                return;
            }

            //TODO: Validar el textbox y el passwordbox
            Singleton.Instance.NombreUsuario = textBoxUsuario.Text;

            //TODO: antes de abrir la nueva ventana se tiene que hacer la logica de inicio de sesion con validacion y respuesta del servidor

            AbrirVentanaMenu();
        }
    }
}
