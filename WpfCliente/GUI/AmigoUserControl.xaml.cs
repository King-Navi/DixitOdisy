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
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for AmigoUserControl.xaml
    /// </summary>
    public partial class AmigoUserControl : UserControl
    {
        public AmigoUserControl()
        {
            InitializeComponent();
        }
        public AmigoUserControl( string nombre, string estado)
        {
            InitializeComponent();
            labelNombreAmigo.Content = nombre;
            labelEstadoAmigo.Content = estado;

        }
    }
}
