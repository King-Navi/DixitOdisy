using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfCliente.ServidorDescribelo
{
    public partial class ImagenCarta
    {
        // Propiedad que convierte MemoryStream a BitmapImage solo una vez y lo reutiliza
        private BitmapImage _bitmapImagen;
        // Propiedad para convertir el MemoryStream en BitmapImage
        public BitmapImage BitmapImagen
        {
            get
            {
                if (ImagenStream == null)
                    return null;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(ImagenStream.ToArray()); // Convertimos el stream para asegurarnos de que está en la posición correcta
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze(); // Para mejorar el rendimiento en WPF
                return bitmap;
            }
        }

        //// Comando para manejar el clic en la imagen
        //public ICommand ImagenClickCommand { get; }

        //public ImagenCarta()
        //{
        //    // Asocia el comando al método que manejará el clic
        //    ImagenClickCommand = new RelayCommand(OnImagenClick);
        //}

        //private void OnImagenClick(object parameter)
        //{
        //    // Muestra el IdImagen o cualquier otra información relevante
        //    Console.WriteLine($"Imagen clickeada: {IdImagen}");
        //}
    }
    // Implementación simple de RelayCommand
    //public class RelayCommand : ICommand
    //{
    //    private readonly Action<object> execute;
    //    private readonly Func<object, bool> canExecute;

    //    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    //    {
    //        this.execute = execute;
    //        this.canExecute = canExecute;
    //    }

    //    public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

    //    public void Execute(object parameter) => execute(parameter);

    //    public event EventHandler CanExecuteChanged;
    //    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    //}


}

