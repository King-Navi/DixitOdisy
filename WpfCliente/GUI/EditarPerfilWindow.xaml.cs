using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class EditarPerfilWindow : Window , IActualizacionUI
    {
        private bool cambioImagen = false;
        private readonly Window menuWindow;
        public EditarPerfilWindow(Window menuWindow)
        {
            this.menuWindow = menuWindow;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            InitializeComponent();
            CargarDatos();
            ActualizarUI();
        }

        private void CargarDatos()
        {
            textBoxCorreo.Text = Singleton.Instance.Correo;
            CargarImagen();
        }

        private void ClicButtonCambiarImagen(object sender, RoutedEventArgs e)
        {
            string rutaImagen = AbrirDialogoSeleccionImagen();
            if (!string.IsNullOrEmpty(rutaImagen))
            {
                if (EsImagenValida(rutaImagen))
                {
                    ProcesarImagen(rutaImagen);
                }
                else
                {
                    MostrarError("El archivo seleccionado no es una imagen válida. Por favor selecciona una imagen.");
                }
            }
            else
            {
                MostrarError("No se seleccionó ninguna imagen.");
            }
        }
        private string AbrirDialogoSeleccionImagen()
        {
            return Imagen.SelecionarRutaImagen();
        }

        private bool EsImagenValida(string rutaImagen)
        {
            return Imagen.EsImagenValida(rutaImagen);
        }
        private void ProcesarImagen(string rutaImagen)
        {
            BitmapImage nuevaImagen = new BitmapImage(new Uri(rutaImagen));
            imageFotoJugador.Source = nuevaImagen;
            VentanasEmergentes.CrearVentanaEmergente("Imagen seleccionada", rutaImagen, this);
            cambioImagen = true;
        }
        private void MostrarError(string mensaje)
        {
            VentanasEmergentes.CrearVentanaEmergenteErrorInesperado(this);
        }

        private void CargarImagen()
        {
            try
            {
                BitmapImage bitmap = Singleton.Instance.FotoJugador;
                imageFotoJugador.Source = bitmap;
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponentErrorException(ex);
            }
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelNombreJugador.Content = Singleton.Instance.NombreUsuario;
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
            
        }

        private void clicButtonCancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void clicButtonAceptar(object sender, RoutedEventArgs e)
        {
            Usuario usuarioEditado = new Usuario();

            bool realizoCambios = VerificarCambioImagen(usuarioEditado)
                                  || VerificarCambioCorreo(usuarioEditado)
                                  || VerificarCambioContrasenia(usuarioEditado);

            if (realizoCambios)
            {
                if (ValidarCampos())
                {
                    GuardarCambiosUsuario(usuarioEditado);
                }
            }
            else
            {
                MostrarMensajeSinCambios();
            }
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
                && textBoxCorreo.Text != Singleton.Instance.Correo)
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
            usuarioEditado.IdUsuario = Singleton.Instance.IdUsuario;
            usuarioEditado.Nombre = Singleton.Instance.NombreUsuario;

            var manejadorServicio = new ServicioManejador<ServicioUsuarioClient>();
            bool resultado = manejadorServicio.EjecutarServicio(proxy =>
            {
                return proxy.EditarUsuario(usuarioEditado);
            });

            if (resultado)
            {
                VentanasEmergentes.CrearVentanaEmergenteDatosEditadosExito(this);
                CerrarSesion();
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloEditarUsuario, Idioma.mensajeUsuarioEditadoFallo, this);
            }
        }

        private void MostrarMensajeSinCambios()
        {
            VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloEditarUsuario, Idioma.mensajeNoHuboCambios, this);
        }

        private bool ValidarCampos()
        {
            bool isValid = true;
            SetDefaultStyles();

            if (!ValidarCaracteristicasContrasenia())
            {
                isValid = false;
            }

            if (!ValidacionesString.EsCorreoValido(textBoxCorreo.Text.Trim()))
            {
                textBoxCorreo.Style = (Style)FindResource("ErrorTextBoxStyle");
                isValid = false;
            }

            return isValid;
        }

        private void SetDefaultStyles()
        {
            labelContraseniasNoCoinciden.Visibility = Visibility.Collapsed;
            textBoxCorreo.Style = (Style)FindResource("NormalTextBoxStyle");

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

            if (ValidacionesString.IsValidSymbol(textBoxContrasenia.Password))
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
            try
            {
                Conexion.CerrarUsuarioSesion();
                Conexion.CerrarConexionesSalaConChat();
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponentErrorException(excepcion);
            }
            IniciarSesion iniciarSesion = new IniciarSesion();
            iniciarSesion.Show();
            menuWindow?.Close();
            this.Close();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
