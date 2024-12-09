using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
        private const int TIEMPO_ESPERA_SEGUNDOS = 10;
        private string NOMBRE_DESCRIBELO = "Describelo";
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
            bool conexionExitosa = await Conexion.VerificarConexionConBaseDatosSinCierreAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                DeshabilitarBotonSiEsValido(sender);
                return;
            }

            HabilitarBotonSiEsValido(sender);

            if (!await VerificarChatMotorAsync())
            {
                return;
            }

            if (!TextoValido(textBoxEnviarMensaje.Text))
            {
                RecibirMensaje(MensajeInvalido());
                return;
            }

            await EnviarMensajeAsync(textBoxEnviarMensaje.Text);
            textBoxEnviarMensaje.Text = "";
        }

        private async Task<bool> VerificarChatMotorAsync()
        {
            if (SingletonChat.Instancia.ChatMotor == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA_SEGUNDOS));
                if (SingletonChat.Instancia.ChatMotor == null)
                {
                    return false;
                }
            }
            return true;
        }

        private bool TextoValido(string texto)
        {
            return texto.Length <= MAXIMO_CARACTERES_PERMITIDOS && !string.IsNullOrWhiteSpace(texto);
        }

        private ChatMensaje MensajeInvalido()
        {
            return new ChatMensaje
            {
                Mensaje = Properties.Idioma.mensajeProfe,
                HoraFecha = DateTime.Now,
                Nombre = NOMBRE_DESCRIBELO
            };
        }

        private async Task EnviarMensajeAsync(string mensaje)
        {
            try
            {
                await SingletonChat.Instancia.ChatMotor.EnviarMensajeAsync(SingletonCliente.Instance.IdChat, new ChatMensaje
                {
                    Mensaje = mensaje,
                    HoraFecha = DateTime.Now,
                    Nombre = SingletonCliente.Instance.NombreUsuario
                });
            }
            catch (TimeoutException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
        }

        private void DeshabilitarBotonSiEsValido(object sender)
        {
            if (sender is Button boton)
            {
                try
                {
                    boton.IsEnabled = false;
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                }
            }
        }

        private void HabilitarBotonSiEsValido(object sender)
        {
            if (sender is Button boton)
            {
                try
                {
                    boton.IsEnabled = true;
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                }
            }
        }


        private void ClicButtonCerrarChat(object sender, RoutedEventArgs e)
        {
            gridChat.Visibility = Visibility.Collapsed;
            buttonAbrirChat.Visibility = Visibility.Visible;
        }

        public void RecibirMensaje(ChatMensaje mensaje)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    textBoxReceptorMensaje.Text += $"{Environment.NewLine} {mensaje}";
                }
                catch (InvalidOperationException excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                }
            });
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
