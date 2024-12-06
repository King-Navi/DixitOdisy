using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfCliente.Contexto;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class EditarPerfilPage : Page, IActualizacionUI
    {
        private bool cambioImagen = false;
        private const string RECURSO_ESTILO_CORREO = "TextBoxEstiloNormal";
        private const string RECURSO_ESTILO_CORREO_ERROR = "TextBoxEstiloError";

        public EditarPerfilPage()
        {
            KeepAlive = false;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            InitializeComponent();
            CargarDatos();
            ActualizarUI();
        }

        private void CargarDatos()
        {
            textBoxCorreo.Text = SingletonCliente.Instance.Correo;
            CargarImagen();
        }

        private void ClicButtonCambiarImagen(object sender, RoutedEventArgs e)
        {
            string rutaImagen = Imagen.SelecionarRutaImagen();
            if (!string.IsNullOrEmpty(rutaImagen))
            {
                if (Imagen.EsImagenValida(rutaImagen,Window.GetWindow(this)))
                {
                    ProcesarImagen(rutaImagen);
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida, 
                        Properties.Idioma.mensajeImagenInvalida, 
                        Window.GetWindow(this));
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenNoSelecionadaEditar, 
                    Properties.Idioma.mensajeImagenNoSelecionadaEditar, 
                    Window.GetWindow(this));
            }
        }

        private void ProcesarImagen(string rutaImagen)
        {
            BitmapImage nuevaImagen = new BitmapImage(new Uri(rutaImagen));
            imageFotoJugador.Source = nuevaImagen;
            VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenSelecionada, 
                rutaImagen, 
                Window.GetWindow(this));
            cambioImagen = true;
        }

        private void CargarImagen()
        {
            try
            {
                BitmapImage bitmap = SingletonCliente.Instance.FotoJugador;
                imageFotoJugador.Source = bitmap;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelNombreJugador.Content = SingletonCliente.Instance.NombreUsuario;
            buttonEditarUsuario.Content = Properties.Idioma.buttonEditarUsuario;
            buttonCancelarCambio.Content = Properties.Idioma.buttonCancelar;
            buttonCambiarFoto.Content = Properties.Idioma.buttonCambiarFotoPerfil;
            labelRepetirContrasenia.Content = Properties.Idioma.labelRepitaContraseña;
            labelContrasenia.Content = Properties.Idioma.labelContrasenia;
            labelCorreo.Content = Properties.Idioma.labelCorreoE;
            labelFotoPerfil.Content = Properties.Idioma.labelSeleccionarFotoPerfil;
            labelContraseniaInstruccion.Content = Properties.Idioma.labelContraseniaInstruccion;
            labelContraseniaMinimo.Content = Properties.Idioma.labelContraseniaMinimo;
            labelContraseniaMaximo.Content = Properties.Idioma.labelContraseniaMaximo;
            labelContraseniaSimbolos.Content = Properties.Idioma.labelContraseniaSimbolos;
            this.Title = Properties.Idioma.tituloEditarUsuario;
        }

        private void ClicButtonCancelar(object sender, RoutedEventArgs e)
        {
            SingletonGestorVentana.Instancia.Regresar();
        }

        private void ClicButtonAceptar(object sender, RoutedEventArgs e)
        {
            Usuario usuarioEditado = new Usuario();

            if (!HayCambiosPendientes(usuarioEditado))
            {
                MostrarMensajeSinCambios();
                return;
            }

            if (HayCambioDeCorreoInvalido(usuarioEditado))
            {
                MostrarMensajeCorreoInvalido();
                return;
            }

            if (ValidarCampos())
            {
                GuardarCambiosUsuario(usuarioEditado);
            }
        }

        private bool HayCambiosPendientes(Usuario usuarioEditado)
        {
            return VerificarCambioImagen(usuarioEditado)
                || VerificarCambioCorreo(usuarioEditado)
                || VerificarCambioContrasenia(usuarioEditado);
        }

        private bool HayCambioDeCorreoInvalido(Usuario usuarioEditado)
        {
            if (VerificarCambioCorreo(usuarioEditado))
            {
                if (!ValidacionesString.EsCorreoValido(textBoxCorreo.Text))
                {
                    return false;
                }
                return !Correo.VerificarCorreo(usuarioEditado.Correo, Window.GetWindow(this));
            }
            return false;
        }

        private void MostrarMensajeSinCambios()
        {
            VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloEditarUsuario,
                Idioma.mensajeNoHuboCambios,
                Window.GetWindow(this));
        }

        private void MostrarMensajeCorreoInvalido()
        {
            VentanasEmergentes.CrearVentanaEmergente(
                Properties.Idioma.tituloCorreoInvalido,
                Properties.Idioma.mensajeCorreoInvalido,
                Window.GetWindow(this));
        }


        private bool VerificarCambioImagen(Usuario usuarioEditado)
        {
            if (cambioImagen && imageFotoJugador.Source is BitmapImage bitmapImage)
            {
                usuarioEditado.FotoUsuario = Imagen.ConvertirBitmapImageAMemoryStream(bitmapImage);
                return true;
            }
            return false;
        }

        private bool VerificarCambioCorreo(Usuario usuarioEditado)
        {
            if (!string.IsNullOrWhiteSpace(textBoxCorreo.Text)
                && !textBoxCorreo.Text.Contains(" ")
                && textBoxCorreo.Text != SingletonCliente.Instance.Correo)
            {
                usuarioEditado.Correo = textBoxCorreo.Text;
                return true;
            }
            return false;
        }

        private bool VerificarCambioContrasenia(Usuario usuarioEditado)
        {
            if (!string.IsNullOrEmpty(textBoxContrasenia.Password)
                && textBoxContrasenia.Password == textBoxRepetirContrasenia.Password)
            {
                usuarioEditado.ContraseniaHASH = Encriptacion.OcuparSHA256(textBoxContrasenia.Password);
                return true;
            }
            return false;
        }

        private void GuardarCambiosUsuario(Usuario usuarioEditado)
        {
            usuarioEditado.IdUsuario = SingletonCliente.Instance.IdUsuario;
            usuarioEditado.Nombre = SingletonCliente.Instance.NombreUsuario;
            var manejadorServicio = new ServicioManejador<ServicioUsuarioClient>();
            bool resultado = manejadorServicio.EjecutarServicio(proxy =>
            {
                return proxy.EditarUsuario(usuarioEditado);
            });

            if (resultado)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloEditarUsuario, 
                    Properties.Idioma.mensajeUsuarioEditadoConExito, 
                    Window.GetWindow(this));

                CerrarSesion();
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloEditarUsuario, 
                    Idioma.mensajeUsuarioEditadoFallo, 
                    Window.GetWindow(this));
            }
        }

        private bool ValidarCampos()
        {
            bool isValid = true;
            ColocarEstilos();

            if (!ValidarCaracteristicasContrasenia())
            {
                isValid = false;
            }

            if (!ValidacionesString.EsCorreoValido(textBoxCorreo.Text.Trim()))
            {
                textBoxCorreo.Style = (Style)FindResource(RECURSO_ESTILO_CORREO_ERROR);
                isValid = false;
            }

            return isValid;
        }

        private void ColocarEstilos()
        {
            labelContraseniasNoCoinciden.Visibility = Visibility.Collapsed;
            textBoxCorreo.Style = (Style)FindResource(RECURSO_ESTILO_CORREO);
            labelContraseniaMinimo.Foreground = Brushes.Red;
            labelContraseniaMaximo.Foreground = Brushes.Red;
            labelContraseniaSimbolos.Foreground = Brushes.Red;
        }

        private bool ValidarCaracteristicasContrasenia()
        {
            if (string.IsNullOrEmpty(textBoxContrasenia.Password) && string.IsNullOrEmpty(textBoxRepetirContrasenia.Password))
            {
                return true;
            }

            bool isValid = true;

            if (textBoxContrasenia.Password.Trim().Length >= 5)
            {
                labelContraseniaMinimo.Foreground = Brushes.Green;
            }
            else
            {
                isValid = false;
            }

            if (textBoxContrasenia.Password.Trim().Length <= 20)
            {
                labelContraseniaMaximo.Foreground = Brushes.Green;
            }
            else
            {
                isValid = false;
            }

            if (ValidacionesString.EsSimboloValido(textBoxContrasenia.Password))
            {
                labelContraseniaSimbolos.Foreground = Brushes.Green;
            }
            else
            {
                isValid = false;
            }

            if (textBoxContrasenia.Password != textBoxRepetirContrasenia.Password)
            {
                labelContraseniasNoCoinciden.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                labelContraseniasNoCoinciden.Visibility = Visibility.Collapsed;
            }

            return isValid;
        }

        private void CerrarSesion()
        {
            SingletonCliente.Instance.LimpiarSingleton();
            SingletonGestorVentana.Instancia.LimpiarHistorial();
            SingletonGestorVentana.Instancia.NavegarA(new IniciarSesionPage());
        }

        private void ClicFlechaAtras(object sender, MouseButtonEventArgs e)
        {
            SingletonGestorVentana.Instancia.Regresar();
        }

    }
}
