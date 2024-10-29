using System;
using System.Collections.Generic;
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
    /// Interaction logic for UsuarioUserControl.xaml
    /// </summary>
    public partial class UsuarioUserControl : UserControl
    {
        private bool esAmigo = false;
        private string nombre;
        private BitmapImage bitmapImage;

        public UsuarioUserControl()
        {
            InitializeComponent();
            SetFondoColorAleatorio();
        }
        public UsuarioUserControl(bool _esAmigo, Usuario usuario)
        {
            esAmigo= _esAmigo;
            InitializeComponent();
            nombre = usuario.Nombre;
            bitmapImage = Imagen.ConvertirStreamABitmapImagen(usuario.FotoUsuario);
            if (bitmapImage != null)
            {
                imageAmigo.Source = bitmapImage;
            }
            if (gridInvitacionAmistad != null && !esAmigo)
            {
                gridPrincipal.Children.Remove(gridInvitacionAmistad);
            }
        }

        private void clicButtonEnviarSolicitud(object sender, RoutedEventArgs e)
        {
            try
            {
                //var manejadorServicio = new ServicioManejador<ServicioAmistadClient>();
                //manejadorServicio.EjecutarServicioAsync(proxy => {
                //    return proxy.
                //});
            }
            catch (Exception ex)
            { 
            
            }
        }
        private void SetFondoColorAleatorio()
        {
            this.Background = Utilidades.GetColorAleatorio();
        }
    }
}
