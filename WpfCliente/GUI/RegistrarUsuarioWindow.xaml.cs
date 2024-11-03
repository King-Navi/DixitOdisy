using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para RegistrarUsuario.xaml
    /// </summary>
    public partial class RegistrarUsuarioWindow : IActualizacionUI
    {
        private Boolean esInivtado;
        private string rutaAbsolutaImagen;
        public RegistrarUsuarioWindow()
        {
            rutaAbsolutaImagen = null;
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }
        public RegistrarUsuarioWindow(bool esInvitado)
        {
            this.esInivtado = esInvitado;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            InitializeComponent();
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

            labelCamposObligatorios.Inlines.Clear();
            labelCamposObligatorios.Inlines.Add(new Run("*") { Foreground = Brushes.Red });
            labelCamposObligatorios.Inlines.Add(new Run(Properties.Idioma.labelCamposObligatorios) { Foreground = Brushes.White });

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

        private void ButtonClicRegistrarUsuario(object sender, RoutedEventArgs e)
        {
            CrearCuenta();
        }


        private void buttonCambiarFoto_Click(object sender, RoutedEventArgs e)
        {
            string rutaImagen = AbrirDialogoSeleccionImagen();
            if (!string.IsNullOrEmpty(rutaImagen))
            {
                if (EsImagenValida(rutaImagen))
                {
                    rutaAbsolutaImagen = rutaImagen;
                    CargarImagen(rutaImagen);

                }
                else
                {
                    MessageBox.Show("El archivo seleccionado no es una imagen válida. Por favor selecciona una imagen.");
                }
            }
            else
            {
                MessageBox.Show("No se seleccionó ninguna imagen.");
            }
        }


        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IniciarSesion iniciarSesionWindow = new IniciarSesion();
            iniciarSesionWindow.Show();
            this.Close();
        }


        private void CrearCuenta()
        {
            if (ValidarCampos() && VerificarCorreo() && rutaAbsolutaImagen != null)
            {
                RegistrarUsuario();
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergenteErrorInesperado(this);
            }
        }

        private bool VerificarCorreo() {
            var manejadorServicio = new ServicioManejador<ServicioCorreoClient>();
            var resultado = manejadorServicio.EjecutarServicio(proxy => {
                return proxy.VerificarCorreo(new Usuario()
                {
                    ContraseniaHASH = null,
                    Correo = textBoxCorreo.Text,
                    Nombre = textBoxGamertag.Text,
                    FotoUsuario = null
                });
            });
            if (resultado)
            {
                string codigoIngresado = AbrirVentanaModal();
                return manejadorServicio.EjecutarServicio(proxy =>
                {
                    return proxy.VerificarCodigo(codigoIngresado);
                });
            }
            else
            {
                return false;
            }
        }

        private async void RegistrarUsuario()
        {
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }

            ServidorDescribelo.IServicioRegistro servicio = new ServidorDescribelo.ServicioRegistroClient();
            try
            {
                string contraseniaHash = Encriptacion.OcuparSHA256(textBoxContrasenia.Password);
                using (FileStream fileStream = new FileStream(rutaAbsolutaImagen, FileMode.Open, FileAccess.Read))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        // Copiar el contenido del FileStream al MemoryStream
                        fileStream.CopyTo(memoryStream);
                        memoryStream.Position = 0; // Restablecer la posición al inicio del MemoryStream


                        // Llamar al servicio con el MemoryStream
                        bool resultado = servicio.RegistrarUsuario(new Usuario()
                        {
                            ContraseniaHASH = contraseniaHash,
                            Correo = textBoxCorreo.Text,
                            Nombre = textBoxGamertag.Text,
                            FotoUsuario = memoryStream
                        });
                        if (resultado)
                        {
                            //TODO: Manejar el error
                            VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloRegistroUsuario, Idioma.mensajeUsuarioRegistradoConExito, this);
                            this.Close();
                        }
                        else
                        {
                            VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
                        }
                    }
                }



            }catch(FaultException<BaseDatosFalla>)
            {
                VentanasEmergentes.CrearVentanaEmergente("El usuario ya existe", "Ya hay un gamertag, busca otro gamertag",this);
            }
            catch (Exception e)
            {
                //TODO: Manejar error
            }
        }

        private void HabilitarBotones(bool habilitado)
        {
            buttonRegistrarUsuario.IsEnabled = habilitado;
            buttonCambiarFoto.IsEnabled = habilitado;
            //TODO desuscribir del evento y suscribirse en otro momento miImagen.mouseLeftButtonDown -= event;
        }

        private string AbrirVentanaModal()
        {
            string valorObtenido = null;
            VerificarCorreoModalWindow ventanaModal = new VerificarCorreoModalWindow();
            ventanaModal.Owner = this;
            bool? resultado = ventanaModal.ShowDialog();

            if (resultado == true)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }

            return valorObtenido;
        }

        public bool ValidarCampos()
        {
            bool isValid = true;
            string errorTextBoxStyle = "ErrorTextBoxStyle";

            SetDefaultStyles();
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

        private void SetDefaultStyles()
        {
            string normalTextBoxStyle = "NormalTextBoxStyle";
            string normalPasswordBoxStyle = "NormalPasswordBoxStyle";

            textBoxGamertag.Style = (Style)FindResource(normalTextBoxStyle);
            textBoxCorreo.Style = (Style)FindResource(normalTextBoxStyle);
            textBoxContrasenia.Style = (Style)FindResource(normalPasswordBoxStyle);
            textBoxRepetirContrasenia.Style = (Style)FindResource(normalPasswordBoxStyle);

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
            if (textBoxContrasenia.Password.Trim().Length >= 5)
            {
                labelContraseniaMinimo.Foreground = Brushes.Green;
                isValid = true;
            }

            if (textBoxContrasenia.Password.Trim().Length <= 20)
            {
                labelContraseniaMaximo.Foreground = Brushes.Green;
                isValid = true;
            }

            if (ValidacionesString.IsValidSymbol(textBoxContrasenia.Password))
            {
                labelContraseniaSimbolos.Foreground = Brushes.Green;
                isValid = true;
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

        private string AbrirDialogoSeleccionImagen()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Seleccionar una imagen",
                Filter = "Archivos de imagen (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png"
            };

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }

        private bool EsImagenValida(string rutaImagen)
        {
            bool resultado = false;
            try
            {
                FileInfo fileInfo = new FileInfo(rutaImagen);
                if (fileInfo.Length > 5 * 1024 * 1024) // 5 MB en bytes verificar
                {
                    VentanasEmergentes.CrearVentanaEmergente("Limite de MB superado",
                        "La imagen no puede pesar mas de 5MB profe, favor de escoger otra imagen", this);
                }
                else
                {
                    BitmapImage bitmap = new BitmapImage();
                    using (FileStream stream = new FileStream(rutaImagen, FileMode.Open, FileAccess.Read))
                    {
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                    }
                    resultado = true;
                }
            }
            catch
            {

            }
            return resultado;
        }
        private void CargarImagen(string ruta)
        {
            try
            {
                // Crear un BitmapImage con la ruta proporcionada
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(ruta, UriKind.Absolute); // UriKind.Absolute si la ruta es completa
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Cargar la imagen en el momento
                bitmap.EndInit();

                // Asignar la imagen cargada al control Image
                imgPerfil.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la imagen: {ex.Message}");
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