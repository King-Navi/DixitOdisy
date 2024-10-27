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
        public BitmapImage BitmapImagen
        {
            get
            {
                if (_bitmapImagen == null && ImagenStream != null)
                {
                    ImagenStream.Position = 0;
                    _bitmapImagen = new BitmapImage();
                    _bitmapImagen.BeginInit();
                    _bitmapImagen.StreamSource = new MemoryStream(ImagenStream.ToArray());
                    _bitmapImagen.CacheOption = BitmapCacheOption.OnLoad;
                    _bitmapImagen.EndInit();
                    _bitmapImagen.Freeze();
                }
                return _bitmapImagen;
            }

        }
        // Comando para manejar el clic en la imagen
        public ICommand ImagenClickCommand { get; }

        public ImagenCarta()
        {
            // Asocia el comando al método que manejará el clic
            ImagenClickCommand = new RelayCommand(OnImagenClick);
        }

        private void OnImagenClick(object parameter)
        {
            // Muestra el IdImagen o cualquier otra información relevante
            Console.WriteLine($"Imagen clickeada: {IdImagen}");
        }
    }
    // Implementación simple de RelayCommand
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

        public void Execute(object parameter) => execute(parameter);

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }


}

