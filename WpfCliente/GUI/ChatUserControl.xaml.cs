using System;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for ChatUserControl.xaml
    /// </summary>
    public partial class ChatUserControl : UserControl , IServicioChatMotorCallback , IActualizacionUI
    {
        public ChatUserControl()
        {
            InitializeComponent();
            gridChat.Visibility = Visibility.Collapsed;
        }

        private void ClicButtonAbrirChat(object sender, RoutedEventArgs e)
        {
            gridChat.Visibility = Visibility.Visible;
            buttonAbrirChat.Visibility = Visibility.Collapsed;
            //CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            //ActualizarUI();
        }

        private async void ClicButtonEnviar(object sender, RoutedEventArgs e)
        {
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
                //TODO:Manejar execpcion
            }
        }

        private void ClicButtonCerrarChar(object sender, RoutedEventArgs e)
        {
            gridChat.Visibility = Visibility.Collapsed;
            buttonAbrirChat.Visibility = Visibility.Visible;
        }

        public void RecibirMensajeCliente(ChatMensaje mensaje)
        {
            //TODO: Realizar el diseño grafico del chat, si es que tiene colores logica para colocarlos
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
