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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for CartaUserControl.xaml
    /// </summary>
    public partial class CartaUserControl : UserControl
    {
        public static readonly DependencyProperty ImagenCartaProperty = DependencyProperty.Register(
            nameof(ImagenCarta), typeof(ImagenCarta), typeof(CartaUserControl), new PropertyMetadata(null, OnImagenCartaChanged));

        public ImagenCarta ImagenCarta
        {
            get => (ImagenCarta)GetValue(ImagenCartaProperty);
            set => SetValue(ImagenCartaProperty, value);
        }

        public CartaUserControl()
        {
            InitializeComponent();
        }

        private static void OnImagenCartaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CartaUserControl)d;
            var imagenCarta = e.NewValue as ImagenCarta;

            if (imagenCarta?.ImagenStream != null)
            {
                control.ImagenDisplay.Source = Imagen.ConvertirStreamABitmapImagen(imagenCarta.ImagenStream);
            }
        }
    }

}

