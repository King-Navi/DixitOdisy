using System;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
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
    public partial class RegistrarUsuarioPage : Page,  IActualizacionUI, IHabilitadorBotones
    {
        private string rutaAbsolutaImagen;
        private const string ESTILO_NORMAL_TEXTO = "TextBoxEstiloNormal";
        private const string ERROR_ESTILO_TEXTO = "TextBoxEstiloError";
        private const string PALABRA_RESERVADA_GUEST = "guest";
        private const int LONGITUD_MINIMA_CONTRASENIA = 5;
        private const int LONGITUD_MAXIMA_CONTRASENIA = 20;
        public RegistrarUsuarioPage()
        {
            KeepAlive = false;
            rutaAbsolutaImagen = null;
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            try
            {
                labelRegistro.Content = Properties.Idioma.tituloRegistroUsuario;
                labelFotoPerfil.Content = Properties.Idioma.labelSeleccionarFotoPerfil;
                labelRepetirContrasenia.Content = Properties.Idioma.labelRepitaContraseña;
                labelContrasenia.Content = Properties.Idioma.labelContrasenia;
                labelGamertag.Content = Properties.Idioma.labelUsuario;
                labelCorreo.Content = Properties.Idioma.labelCorreoE;
                labelFotoPerfil.Content = Properties.Idioma.labelSeleccionarFotoPerfil;
                labelCamposObligatorios.Content = Properties.Idioma.labelCamposObligatorios;
                labelContraseniaInstruccion.Content = Properties.Idioma.labelContraseniaInstruccion;
                labelContraseniaMinimo.Content = Properties.Idioma.labelContraseniaMinimo;
                labelContraseniaMaximo.Content = Properties.Idioma.labelContraseniaMaximo;
                labelContraseniaSimbolos.Content = Properties.Idioma.labelContraseniaSimbolos;
                buttonRegistrarUsuario.Content = Properties.Idioma.buttonRegistrarse;
                buttonCambiarFoto.Content = Properties.Idioma.buttonCambiarFotoPerfil;
                labelContraseniasNoCoinciden.Content = Properties.Idioma.labelContraseniaNoCoincide;
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

        private async void ClicButtonRegistrarUsuarioAsync(object sender, RoutedEventArgs e)
        {
            if (textBoxCorreo.Text.Contains(" ")
                || textBoxNombreUsuario.Text.Contains(" ")
                || passwordBoxContrasenia.Password.Contains(" ")
                || passwordBoxRepetirContrasenia.Password.Contains(" "))
            {
                VentanasEmergentes.CrearVentanaEmergente(
                    Idioma.tituloCamposInvalidos, 
                    Idioma.mensajeCaracteresInvalidoEspacio, 
                    Window.GetWindow(this));
                return;
            }
            buttonRegistrarUsuario.IsEnabled = false;
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                return;
            }
            await CrearCuenta();
            buttonRegistrarUsuario.IsEnabled = true;
        }


        private void ClicButtonCambiarFoto(object sender, RoutedEventArgs e)
        {
            string rutaImagen = Imagen.SelecionarRutaImagen();
            if (!string.IsNullOrEmpty(rutaImagen))
            {
                if (Imagen.EsImagenValida(rutaImagen, Window.GetWindow(this)))
                {
                    rutaAbsolutaImagen = rutaImagen;
                    CargarImagen(rutaImagen);

                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                        Properties.Idioma.mensajeImagenInvalida, Window.GetWindow(this));
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeImagenInvalida, Window.GetWindow(this));
            }
        }


        private void ClicImagenFlechaAtras(object sender, MouseButtonEventArgs e)
        {
            SingletonGestorVentana.Instancia.Regresar();
        }

        private async Task CrearCuenta()
        {
            if (ValidarCampos() && Correo.VerificarCorreo(textBoxCorreo.Text, Window.GetWindow(this)) && rutaAbsolutaImagen != null)
            {
                var resultado = await RegistrarUsuarioAsync();
                if (resultado)
                {
                    SingletonGestorVentana.Instancia.Regresar();
                }
            }
            else
            {
                if (rutaAbsolutaImagen == null)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloRegistroUsuario,
                        Properties.Idioma.mensajeImagenNoSelecionadaEditar, Window.GetWindow(this));
                }
            }
        }

        private async Task<bool> RegistrarUsuarioAsync()
        {
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                return false;
            }
            try
            {
                EvaluarPalabrasProhibidas();
                ServidorDescribelo.IServicioRegistro servicio = new ServidorDescribelo.ServicioRegistroClient();
                string contraseniaHash = Encriptacion.OcuparSHA256(passwordBoxContrasenia.Password);
                using (FileStream fileStream = new FileStream(rutaAbsolutaImagen, FileMode.Open, FileAccess.Read))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await fileStream.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;

                        bool resultado = await servicio.RegistrarUsuarioAsync(new Usuario()
                        {
                            ContraseniaHASH = contraseniaHash,
                            Correo = textBoxCorreo.Text,
                            Nombre = textBoxNombreUsuario.Text,
                            FotoUsuario = memoryStream
                        });
                        if (resultado)
                        {
                            VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloRegistroUsuario,
                                Idioma.mensajeUsuarioRegistradoConExito, Window.GetWindow(this));
                            return true;
                        }
                        else
                        {
                            VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor,
                                Properties.Idioma.mensajeErrorServidor, Window.GetWindow(this));
                        }
                    }
                }
            }
            catch (IOException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (FaultException<BaseDatosFalla>)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloUsuarioExiste, 
                    Properties.Idioma.mensajeUsuarioYaExiste, Window.GetWindow(this));
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion, Window.GetWindow(this));
            }
            return false;
        }

        private void EvaluarPalabrasProhibidas()
        {
            if (textBoxNombreUsuario.Text?.ToLower().Contains(PALABRA_RESERVADA_GUEST) == true)
            {
                throw new FaultException<BaseDatosFalla>(new BaseDatosFalla());
            }
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            try
            {
                buttonRegistrarUsuario.IsEnabled = esHabilitado;
                buttonCambiarFoto.IsEnabled = esHabilitado;
                imagenFlechaAtras.IsEnabled = esHabilitado;
                buttonRegistrarUsuario.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA : Utilidades.OPACIDAD_MINIMA;
                buttonCambiarFoto.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA : Utilidades.OPACIDAD_MINIMA;
                imagenFlechaAtras.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA : Utilidades.OPACIDAD_MINIMA;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
        }


        public bool ValidarCampos()
        {
            bool isValid = true;
            ObtenerEstilos();
            if (!ValidarCaracteristicasContrasenia())
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCamposInvalidos,
                    Properties.Idioma.mensajeCamposInvalidos,
                    Window.GetWindow(this));
                return false;
            }

            if (!ValidacionesString.EsCorreoValido(textBoxCorreo.Text.Trim()))
            {
                textBoxCorreo.Style = (Style)FindResource(ERROR_ESTILO_TEXTO);
                labelCorreoInvalido.Visibility = Visibility.Visible;

                return false;
            }

            if (!ValidacionesString.EsGamertagValido(textBoxNombreUsuario.Text.Trim()))
            {
                textBoxNombreUsuario.Style = (Style)FindResource(ERROR_ESTILO_TEXTO);

                return false;
            }

            return isValid;
        }

        private void ObtenerEstilos()
        {
            try
            {
                textBoxNombreUsuario.Style = (Style)FindResource(ESTILO_NORMAL_TEXTO);
                textBoxCorreo.Style = (Style)FindResource(ESTILO_NORMAL_TEXTO);
            }
            catch (ResourceReferenceKeyNotFoundException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            labelCorreoInvalido.Visibility = Visibility.Hidden;
            labelCorreoExistente.Visibility = Visibility.Hidden;
            labelGamertagExistente.Visibility = Visibility.Hidden;
            labelContraseniaMinimo.Foreground = Brushes.Red;
            labelContraseniaMaximo.Foreground = Brushes.Red;
            labelContraseniaSimbolos.Foreground = Brushes.Red;
        }

        private bool ValidarCaracteristicasContrasenia()
        {
            bool esValida = EsContraseniaValida();
            ActualizarLabelsContrasenia();
            return esValida;
        }

        private bool EsContraseniaValida()
        {
            bool esValida = true;
            if (passwordBoxContrasenia.Password.Contains(" "))
            {
                esValida = false;
            }
            if (passwordBoxContrasenia.Password.Trim().Length < LONGITUD_MINIMA_CONTRASENIA)
            {
                esValida = false;
            }
            if (passwordBoxContrasenia.Password.Trim().Length > LONGITUD_MAXIMA_CONTRASENIA)
            {
                esValida = false;
            }
            if (!ValidacionesString.EsSimboloValido(passwordBoxContrasenia.Password))
            {
                esValida = false;
            }
            if (passwordBoxContrasenia.Password != passwordBoxRepetirContrasenia.Password)
            {
                esValida = false;
            }
            return esValida;
        }

        private void ActualizarLabelsContrasenia()
        {
            if (passwordBoxContrasenia.Password.Trim().Length >= LONGITUD_MINIMA_CONTRASENIA)
            {
                labelContraseniaMinimo.Foreground = Brushes.Green;
            }
            else
            {
                labelContraseniaMinimo.Foreground = Brushes.Red;
            }
            if (passwordBoxContrasenia.Password.Trim().Length <= LONGITUD_MAXIMA_CONTRASENIA)
            {
                labelContraseniaMaximo.Foreground = Brushes.Green;
            }
            else
            {
                labelContraseniaMaximo.Foreground = Brushes.Red;
            }
            if (ValidacionesString.EsSimboloValido(passwordBoxContrasenia.Password))
            {
                labelContraseniaSimbolos.Foreground = Brushes.Green;
            }
            else
            {
                labelContraseniaSimbolos.Foreground = Brushes.Red;
            }
            if (passwordBoxContrasenia.Password != passwordBoxRepetirContrasenia.Password)
            {
                labelContraseniasNoCoinciden.Visibility = Visibility.Visible;
            }
            else
            {
                labelContraseniasNoCoinciden.Visibility = Visibility.Collapsed;
            }
        }

        private void CargarImagen(string ruta)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(ruta, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                imagePerfil.Source = bitmap;
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeImagenInvalida, Window.GetWindow(this));
            }
        }

        private void CerrandoVentana(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
        }
    }
}