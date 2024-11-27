using System;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class RegistrarUsuarioWindow : IActualizacionUI, IHabilitadorBotones
    {
        private string rutaAbsolutaImagen;
        public RegistrarUsuarioWindow()
        {
            rutaAbsolutaImagen = null;
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }

        public void ActualizarUI()
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
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private void ClicButtonRegistrarUsuario(object sender, RoutedEventArgs e)
        {
            CrearCuentaAsync();
        }


        private void ClicButtonCambiarFoto(object sender, RoutedEventArgs e)
        {
            string rutaImagen = Imagen.SelecionarRutaImagen();
            if (!string.IsNullOrEmpty(rutaImagen))
            {
                if (Imagen.EsImagenValida(rutaImagen,this))
                {
                    rutaAbsolutaImagen = rutaImagen;
                    CargarImagen(rutaImagen);

                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida, 
                        Properties.Idioma.mensajeImagenInvalida, this);
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida, 
                    Properties.Idioma.mensajeImagenInvalida, this);
            }
        }


        private void ClicFlechaAtras(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }


        private async void CrearCuentaAsync()
        {
            if (!ValidarCampos())
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCamposInvalidos,
                    Properties.Idioma.mensajeCamposInvalidos, this);
            }
            else if (rutaAbsolutaImagen == null)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida,
                    Properties.Idioma.mensajeImagenInvalida, this);
            }
            else if (!Correo.VerificarCorreo(textBoxCorreo.Text, this))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCorreoInvalido,
                    Properties.Idioma.mensajeCorreoInvalido, this);
            }
            else
            {
                await RegistrarUsuarioAsync();
            }
        }

        private async Task RegistrarUsuarioAsync()
        {
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }

            ServidorDescribelo.IServicioRegistro servicio = new ServidorDescribelo.ServicioRegistroClient();
            try
            {
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
                            Nombre = textBoxGamertag.Text,
                            FotoUsuario = memoryStream
                        });
                        if (resultado)
                        {
                            VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloRegistroUsuario, 
                                Idioma.mensajeUsuarioRegistradoConExito, this);
                            this.Close();
                        }
                        else
                        {
                            VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, 
                                Properties.Idioma.mensajeErrorServidor, this);
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
            catch(FaultException<BaseDatosFalla>)
            {
                VentanasEmergentes.CrearVentanaEmergenteConCierre(Properties.Idioma.tituloUsuarioExiste, 
                    Properties.Idioma.mensajeUsuarioYaExiste,this);
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(ex, this);
            }
        }

        public void HabilitarBotones (bool esHabilitado)
        {
            buttonRegistrarUsuario.IsEnabled = esHabilitado;
            buttonCambiarFoto.IsEnabled = esHabilitado;
            imagenFlechaAtras.IsEnabled = esHabilitado;

            buttonRegistrarUsuario.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA : Utilidades.OPACIDAD_MINIMA;
            buttonCambiarFoto.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA : Utilidades.OPACIDAD_MINIMA;
            imagenFlechaAtras.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA : Utilidades.OPACIDAD_MINIMA;
        }


        public bool ValidarCampos()
        {
            bool isValid = true;
            string errorTextBoxStyle = "ErrorTextBoxStyle";

            ObtenerEstilos();
            if (!ValidarCaracteristicasContrasenia())
            {
                isValid = false;
            }

            if (!ValidacionesString.EsCorreoValido(textBoxCorreo.Text.Trim()))
            {
                textBoxCorreo.Style = (Style)FindResource(errorTextBoxStyle);
                labelCorreoInvalido.Visibility = Visibility.Visible;

                isValid = false;
            }

            if (!ValidacionesString.EsGamertagValido(textBoxGamertag.Text.Trim()))
            {
                textBoxGamertag.Style = (Style)FindResource(errorTextBoxStyle);

                isValid = false;
            }

            return isValid;
        }

        private void ObtenerEstilos()
        {
            string normalTextBoxStyle = "NormalTextBoxStyle";
            string normalPasswordBoxStyle = "NormalPasswordBoxStyle";

            textBoxGamertag.Style = (Style)FindResource(normalTextBoxStyle);
            textBoxCorreo.Style = (Style)FindResource(normalTextBoxStyle);
            passwordBoxContrasenia.Style = (Style)FindResource(normalPasswordBoxStyle);
            passwordBoxRepetirContrasenia.Style = (Style)FindResource(normalPasswordBoxStyle);

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
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(ex);
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloImagenInvalida, 
                    Properties.Idioma.mensajeImagenInvalida, this);
            }
        }

        private void CerrandoVentana(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
            IniciarSesion iniciarSesion = new IniciarSesion();
            iniciarSesion.Show();
        }
    }
}