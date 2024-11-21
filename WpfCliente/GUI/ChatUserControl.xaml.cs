using System;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class ChatUserControl : UserControl , IServicioChatMotorCallback , IActualizacionUI
    {
        private const int MAXIMO_CARACTERES_PERMITIDOS = 200;
        public ChatUserControl()
        {
            InitializeComponent();
            gridChat.Visibility = Visibility.Collapsed;
        }

        private void ClicButtonAbrirChat(object sender, RoutedEventArgs e)
        {
            gridChat.Visibility = Visibility.Visible;
            buttonAbrirChat.Visibility = Visibility.Collapsed;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
        }

        private async void ClicButtonEnviar(object sender, RoutedEventArgs e)
        {
            if (textBoxEnviarMensaje.Text.Length > MAXIMO_CARACTERES_PERMITIDOS || string.IsNullOrWhiteSpace(textBoxEnviarMensaje.Text))
            {
                RecibirMensajeCliente(new ChatMensaje
                {
                    //TODO: I18N
                    Mensaje = "Profe ese texto no vale",
                    HoraFecha = DateTime.Now,
                    Nombre = "Describelo"
                });
                return;
            }
            try
            {
                await Conexion.ChatMotor.EnviarMensajeAsync(Singleton.Instance.IdChat, new ChatMensaje
                {
                    Mensaje = textBoxEnviarMensaje.Text,
                    HoraFecha = DateTime.Now,
                    Nombre = Singleton.Instance.NombreUsuario
                });
                textBoxEnviarMensaje.Text = "";
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponentErrorException(excepcion);
            }
        }

        private void ClicButtonCerrarChar(object sender, RoutedEventArgs e)
        {
            gridChat.Visibility = Visibility.Collapsed;
            buttonAbrirChat.Visibility = Visibility.Visible;
        }

        public void RecibirMensajeCliente(ChatMensaje mensaje)
        {
            textBoxReceptorMensaje.Text += $"{Environment.NewLine} {mensaje.ToString()}";
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
    }
}
