using System;
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
                if (sender is Button boton)
                {
                    try
                    {
                        boton.IsEnabled = false;
                    }
                    catch (InvalidOperationException excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                    }
                    catch (Exception excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                    }
                }
                return;
            }
            if (sender is Button botonEnviar)
            {
                try
                {
                    botonEnviar.IsEnabled = true;
                }
                catch (InvalidOperationException excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                }
            }
            if (SingletonChat.Instancia.ChatMotor == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA_SEGUNDOS));
                if (SingletonChat.Instancia.ChatMotor == null)
                {
                    return;
                }
            }

            if (textBoxEnviarMensaje.Text.Length > MAXIMO_CARACTERES_PERMITIDOS || string.IsNullOrWhiteSpace(textBoxEnviarMensaje.Text))
            {
                RecibirMensaje(new ChatMensaje
                {
                    Mensaje = Properties.Idioma.mensajeProfe,
                    HoraFecha = DateTime.Now,
                    Nombre = NOMBRE_DESCRIBELO
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
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
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
