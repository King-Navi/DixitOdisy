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
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private async void ClicButtonRegistrarUsuarioAsync(object sender, RoutedEventArgs e)
        {
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                return;
            }
            CrearCuenta();
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

        private async void CrearCuenta()
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
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloRegistroUsuario,
                        Properties.Idioma.mensajeCamposIntroducidosInvalidos, Window.GetWindow(this));
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
                        fileStream.CopyTo(memoryStream);
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
            catch (IOException ex)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(ex);
            }
            catch (ArgumentNullException ex)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(ex);
            }
            catch (FaultException<BaseDatosFalla>)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloUsuarioExiste, 
                    Properties.Idioma.mensajeUsuarioYaExiste, Window.GetWindow(this));
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(ex, Window.GetWindow(this));
            }
            return false;
        }

        private void EvaluarPalabrasProhibidas()
        {
            if (textBoxNombreUsuario.Text?.ToLower().Contains(PALABRA_RESERVADA_GUEST) == true)
            {
                throw new FaultException<BaseDatosFalla>(new BaseDatosFalla());
            };
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
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
        }


        public bool ValidarCampos()
        {
            bool isValid = true;
            ObtenerEstilos();
            if (!ValidarCaracteristicasContrasenia())
            {
                isValid = false;
            }

            if (!ValidacionesString.EsCorreoValido(textBoxCorreo.Text.Trim()))
            {
                textBoxCorreo.Style = (Style)FindResource(ERROR_ESTILO_TEXTO);
                labelCorreoInvalido.Visibility = Visibility.Visible;

                isValid = false;
            }

            if (!ValidacionesString.EsGamertagValido(textBoxNombreUsuario.Text.Trim()))
            {
                textBoxNombreUsuario.Style = (Style)FindResource(ERROR_ESTILO_TEXTO);

                isValid = false;
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
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
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
            bool isValid = false;
            if (passwordBoxContrasenia.Password.Trim().Length >= 5)
            {
                labelContraseniaMinimo.Foreground = Brushes.Green;
                isValid = true;
            }

            if (passwordBoxContrasenia.Password.Trim().Length <= 20)
            {
                labelContraseniaMaximo.Foreground = Brushes.Green;
                isValid = true;
            }

            if (ValidacionesString.EsSimboloValido(passwordBoxContrasenia.Password))
            {
                labelContraseniaSimbolos.Foreground = Brushes.Green;
                isValid = true;
            }

            if (passwordBoxContrasenia.Password != passwordBoxRepetirContrasenia.Password)
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