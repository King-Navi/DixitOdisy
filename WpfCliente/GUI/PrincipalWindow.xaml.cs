using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfCliente.Interfaz;

namespace WpfCliente.GUI
{
    public partial class PrincipalWindow : Window , INavegacion
    {
        public Frame MarcoNavegacion { get; set; }
        public PrincipalWindow()
        {
            InitializeComponent();
            MarcoNavegacion = FrameVentanaContenedora;
            MarcoNavegacion.Navigate(new IniciarSesionPage());
        }

        private void VistaPreviaTeclaPresionada(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.BrowserBack || (e.SystemKey == Key.Left && (Keyboard.Modifiers & ModifierKeys.Alt) != 0))
            {
                e.Handled = true;
            }
        }
    }
}
