using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for MazoUserControl.xaml
    /// </summary>
    public partial class MazoUserControl : UserControl
    {
        public ObservableCollection<ImagenCarta> ImagenCartas { get; set; } = new ObservableCollection<ImagenCarta>();

        public MazoUserControl()
        {
            InitializeComponent();
            DataContext = this;  // Establece el contexto de datos en el control
        }

        public void RecibirImagen(ImagenCarta imagen)
        {
            if (imagen != null)
            {
                ImagenCartas.Add(imagen);
            }
        }
    }
}

