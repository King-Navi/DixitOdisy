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
using System.Windows.Shapes;

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
            ValorIngresado = textBoxCodigoSala.Text;
            ValorIngresado.ToUpper();
            DialogResult = true; 
            Close();
        }
    }
}
