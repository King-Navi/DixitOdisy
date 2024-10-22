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
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class EditarPerfilWindow : Window , IActualizacionUI
    {
        private bool cambioImagen = false;
        public EditarPerfilWindow()
        {
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
        private void ProcesarImagen(string rutaImagen)
        {
            MessageBox.Show($"Imagen válida seleccionada: {rutaImagen}");
            // Aquí puedes asignar la imagen a un control de imagen o hacer más procesamiento
            cambioImagen = true;
        }
        private void MostrarError(string mensaje)
        {
            //TODO: Podemos hacer una windows de advertencia o ocupar la de error
            MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Error al cargar la imagen: {ex.Message}");
            }
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelNombreJugador.Content = Singleton.Instance.NombreUsuario;
        }

        private void clicButtonCancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void clicButtonAceptar(object sender, RoutedEventArgs e)
        {
            bool realizoCambios = false;
            Usuario usuarioEditado = new Usuario();
            if (cambioImagen)
            {
                usuarioEditado.FotoUsuario = Imagen.ConvertirBitmapImageAMemoryStream(imageFotoJugador.Source as BitmapImage);
                realizoCambios = true;
            }
            if (!string.IsNullOrWhiteSpace(textBoxCorreo.Text)
                || !textBoxCorreo.Text.Contains(" ")
                || textBoxCorreo.Text != Singleton.Instance.Correo)
            {
                usuarioEditado.Correo = textBoxCorreo.Text;
                realizoCambios = true;

            }
            if (textBoxContrania.Text == textBoxConfirmacionContrania.Text
                || textBoxContrania.Text != Singleton.Instance.ContraniaHash)
            {
                realizoCambios = true;
            }
            if (realizoCambios)
            {
                usuarioEditado.IdUsuario = Singleton.Instance.IdUsuario;
                var manejadorServicio = new ServicioManejador<ServicioUsuarioClient>();
                bool  resultado = manejadorServicio.EjecutarServicio(proxy =>
                {
                    return proxy.EditarUsuario(usuarioEditado);
                });
                if (resultado)
                {
                    MessageBox.Show("Exito");

                }
                else
                {
                    MessageBox.Show("Fallo en la llamada al servidor para editar tus datos");

                }
            }
            else 
            {
                MessageBox.Show("No hiciste nada");
            
            }
    
        }
    }
}
