using System;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.Contexto;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class ChatUserControl : UserControl , IActualizacionUI , IHabilitadorBotones
    {
        private const int MAXIMO_CARACTERES_PERMITIDOS = 200;
        public ChatUserControl()
        {
            this.Loaded += CargarNuevoContexto;
            SingletonChat.Instancia.RecibirMensaje  += RecibirMensaje;
            InitializeComponent();
            gridChat.Visibility = Visibility.Collapsed;
            ActualizarUI();
        }

        private void CargarNuevoContexto(object sender, RoutedEventArgs e)
        {
            if (this.DataContext == null)
            {
                this.DataContext = this;
            }
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
                RecibirMensaje(new ChatMensaje
                {
                    Mensaje = Properties.Idioma.mensajeProfe,
                    HoraFecha = DateTime.Now,
                    Nombre = "Describelo"
                });
                return;
            }
            try
            {
                await SingletonChat.Instancia.ChatMotor.EnviarMensajeAsync(SingletonCliente.Instance.IdChat, new ChatMensaje
                {
                    Mensaje = textBoxEnviarMensaje.Text,
                    HoraFecha = DateTime.Now,
                    Nombre = SingletonCliente.Instance.NombreUsuario
                });
                textBoxEnviarMensaje.Text = "";
            }
            catch (TimeoutException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
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

        public void RecibirMensaje(ChatMensaje mensaje)
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
            SingletonChat.Instancia.RecibirMensaje -= RecibirMensaje;

        }

        public void HabilitarBotones(bool esHabilitado)
        {
            userControlChat.IsEnabled = esHabilitado;
        }
    }
}
