using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para RegistrarUsuario.xaml
    /// </summary>
    public partial class RegistrarUsuarioWindow : Window, IActualizacionUI
    {
        private Boolean esInivtado;
        private string[] imagenesPerfil = {
            "/Recursos/pfp1.png",
            "/Recursos/pfp2.png",
            "/Recursos/pfp3.png",
            "/Recursos/pfp4.png",
            "/Recursos/pfp5.png"
        };
        private int contadorImagen = 0;
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
            if (rutaAbsolutaImagen == null)
            {
                return;
            }
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
            //contadorImagen = (contadorImagen + 1) % imagenesPerfil.Length;
            //imgPerfil.Source = new BitmapImage(new Uri(imagenesPerfil[contadorImagen], UriKind.Relative));
        }


        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IniciarSesion iniciarSesionWindow = new IniciarSesion();
            iniciarSesionWindow.Show();
            this.Close();
        }


        private void CrearCuenta()
        {
            if (ValidarCampos())
            {
                RegistrarUsuario();
            }
        }

        private  void RegistrarUsuario()
        {
            //TODO: Realizar caso en el que no hay conexion
            ServidorDescribelo.IServicioRegistro servicio = new ServidorDescribelo.ServicioRegistroClient();
            try
            {
                string contraseniaHash = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(textBoxContrasenia.Password))).Replace("-", "");
                //TODO:Preguntar por el path de la imagen
                using (FileStream fileStream = new FileStream(@"C:\Users\USER\Downloads\profile.jpg", FileMode.Open, FileAccess.Read))
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
                            MessageBox.Show("Acierto");
                            this.Close();
                            IniciarSesion iniciarSesion = new IniciarSesion();
                            iniciarSesion.Show();
                        }
                        else
                        {
                            MessageBox.Show("error");
                        }
                    }
                }



            }
            catch (Exception)
            {
                //TODO: Manejar error
            }
            finally
            {
                if (servicio != null)
                {
                    ((ICommunicationObject)servicio).Close();
                }
            }
        }

        public bool ValidarCampos()
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            bool isValid = true;
            string errorTextBoxStyle = "ErrorTextBoxStyle";

            SetDefaultStyles();
            ValidarCaracteristicasContrasenia();

            if (!ValidacionesString.IsValidEmail(textBoxCorreo.Text.Trim()))
            {
                textBoxCorreo.Style = (Style)FindResource(errorTextBoxStyle);
                labelCorreoInvalido.Visibility = Visibility.Visible;

                isValid = false;
            }

            if (!ValidacionesString.IsValidGamertag(textBoxGamertag.Text.Trim()))
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
