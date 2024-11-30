using System;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.Contexto;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class ChatUserControl : UserControl , IServicioChatMotorCallback , IActualizacionUI , IHabilitadorBotones
    {
        private const int MAXIMO_CARACTERES_PERMITIDOS = 200;
        public ChatUserControl()
        {
            InitializeComponent();
            gridChat.Visibility = Visibility.Collapsed;
            ActualizarUI();
        }

        private void ClicButtonAbrirChat(object sender, RoutedEventArgs e)
        {
            gridChat.Visibility = Visibility.Visible;
            buttonAbrirChat.Visibility = Visibility.Collapsed;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
        }

        private async void ClicButtonEnviarAsync(object sender, RoutedEventArgs e)
        {
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                SingletonGestorVentana.Instancia.NavegarA(new IniciarSesionPage());
                return;
            }
            if (textBoxEnviarMensaje.Text.Length > MAXIMO_CARACTERES_PERMITIDOS || string.IsNullOrWhiteSpace(textBoxEnviarMensaje.Text))
            {
                RecibirMensajeClienteCallback(new ChatMensaje
                {
                    Mensaje = Properties.Idioma.mensajeProfe,
                    HoraFecha = DateTime.Now,
                    Nombre = "Describelo"
                });
                return;
            }
            try
            {
                await Conexion.ChatMotor.EnviarMensajeAsync(SingletonCliente.Instance.IdChat, new ChatMensaje
                {
                    Mensaje = textBoxEnviarMensaje.Text,
                    HoraFecha = DateTime.Now,
                    Nombre = SingletonCliente.Instance.NombreUsuario
                });
                textBoxEnviarMensaje.Text = "";
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
        }

        private void ClicButtonCerrarChat(object sender, RoutedEventArgs e)
        {
            gridChat.Visibility = Visibility.Collapsed;
            buttonAbrirChat.Visibility = Visibility.Visible;
        }

        public void RecibirMensajeClienteCallback(ChatMensaje mensaje)
        {
            textBoxReceptorMensaje.Text += $"{Environment.NewLine} {mensaje}";
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            buttonEnviar.Content = Idioma.buttonEnviar;
        }

        private void CerrarControl(object sender, RoutedEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            userControlChat.IsEnabled = esHabilitado;
        }
    }
}
