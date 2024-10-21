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

namespace WpfCliente.GUI
{
    public partial class EditarPerfilWindow : Window
    {
        public EditarPerfilWindow()
        {
            InitializeComponent();
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
        }
        private void MostrarError(string mensaje)
        {
            //TODO: Podemos hacer una windows de advertencia o ocupar la de error
            MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
