using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para SeleccionCartaUsercontrol.xaml
    /// </summary>
    public partial class SeleccionCartaUsercontrol : UserControl
    {
        public ObservableCollection<ImagenCarta> Imagenes { get; set; }
        public ICommand EliminarImagenPorIdCommand { get; }

        public SeleccionCartaUsercontrol()
        {
            InitializeComponent();
            Imagenes = new ObservableCollection<ImagenCarta>();
            DataContext = this;
            EliminarImagenPorIdCommand = new RelayCommand<string>(EliminarImagenPorId);

        }

        internal void AgregarImagen(ImagenCarta imagen)
        {
            if (Imagenes.Count < 6)
            {
                Imagenes.Add(imagen);
            }
        }
        // Método para eliminar una imagen por su Id
        public void EliminarImagenPorId(string id)
        {
            var imagenAEliminar = Imagenes.FirstOrDefault(i => i.IdImagen == id);
            if (imagenAEliminar != null)
            {
                Imagenes.Remove(imagenAEliminar);
            }
        }
    }
}
