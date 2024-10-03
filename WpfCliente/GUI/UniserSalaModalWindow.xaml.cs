using System.Windows;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for UniserSalaModalWindow.xaml
    /// </summary>
    public partial class UniserSalaModalWindow : Window
    {
        public string ValorIngresado { get; private set; }

        public UniserSalaModalWindow()
        {
            InitializeComponent();
        }

        private void ClicButtonAceptar(object sender, RoutedEventArgs e)
        {
            ValorIngresado = textBoxCodigoSala.Text.ToUpper();
            DialogResult = true; 
            Close();
        }
    }
}
