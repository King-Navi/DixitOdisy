using System.Windows.Controls;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class AmigoUserControl : UserControl
    {
        public AmigoUserControl()
        {
            InitializeComponent();
            ColocarFondoColorAleatorio();
        }
        public AmigoUserControl( Amigo amigo)
        {
            InitializeComponent();
            labelNombreAmigo.Content = amigo.Nombre;
            labelEstadoAmigo.Content = amigo.Estado;
            labelUltimaConexion.Content = amigo.UltimaConexion;
            imageAmigo.Source = Imagen.ConvertirStreamABitmapImagen(amigo.Foto);
            ColocarFondoColorAleatorio();
            
        }

        private void ColocarFondoColorAleatorio()
        {
            this.Background = Utilidades.ObtenerColorAleatorio();
        }
    }
}
