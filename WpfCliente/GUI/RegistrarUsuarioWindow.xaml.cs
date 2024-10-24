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
            if (ValidarCampos() && rutaAbsolutaImagen != null)
            {
                VerificarCorreo();
                RegistrarUsuario();
            }
        }

        private void VerificarCorreo()
        {
            //AbrirVentanaModal();
            //ServidorDescribelo.IServicioCorreo servicio = new ServidorDescribelo.ServicioCorreoClient();
            
        }

        private async void RegistrarUsuario()
        {
            Task<bool> verificarConexion = Validacion.ValidarConexion();
            HabilitarBotones(false);
            if (!await verificarConexion)
            {
                VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
                HabilitarBotones(true);
                return;
            }

            ServidorDescribelo.IServicioRegistro servicio = new ServidorDescribelo.ServicioRegistroClient();
            try
            {
                string contraseniaHash = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(textBoxContrasenia.Password))).Replace("-", "");
                //TODO:Preguntar por el path de la imagen
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
                            VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloRegistroUsuario, Idioma.mensajeUsuarioRegistradoConExito);
                            IniciarSesion iniciarSesion = new IniciarSesion();
                            iniciarSesion.Show();
                            this.Close();
                        }
                        else
                        {
                            VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
                        }
                    }
                }
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
            else
            {
                //TODO: I18N
                MessageBox.Show("No se ingresó ningún valor.");
            }

            return valorObtenido;
        }

        public bool ValidarCampos()
        {
            bool isValid = true;
            string errorTextBoxStyle = "ErrorTextBoxStyle";

            SetDefaultStyles();
            ValidarCaracteristicasContrasenia();

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

        private void ValidarCaracteristicasContrasenia()
        {
            if (textBoxContrasenia.Password.Trim().Length >= 5)
            {
                labelContraseniaMinimo.Foreground = Brushes.Green;
            }

            if (textBoxContrasenia.Password.Trim().Length <= 20)
            {
                labelContraseniaMaximo.Foreground = Brushes.Green;
            }

            if (ValidacionesString.IsValidSymbol(textBoxContrasenia.Password))
            {
                labelContraseniaSimbolos.Foreground = Brushes.Green;
            }

            if (textBoxContrasenia.Password != textBoxRepetirContrasenia.Password)
            {
                labelContraseniasNoCoinciden.Visibility = Visibility.Visible;
            }
            else
            {
                labelContraseniasNoCoinciden.Visibility = Visibility.Collapsed;
            }
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
            try
            {
                BitmapImage bitmap = new BitmapImage();
                using (FileStream stream = new FileStream(rutaImagen, FileMode.Open, FileAccess.Read))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }
                return true;
            }
            catch
            {
                return false;
            }
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
    }
}