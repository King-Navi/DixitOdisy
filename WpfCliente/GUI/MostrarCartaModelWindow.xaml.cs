using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for MostrarCartaModelWindow.xaml
    /// </summary>
    public partial class MostrarCartaModelWindow : Window
    {
        public string Pista {  get; set; }
        public MostrarCartaModelWindow(bool esNarrador, BitmapImage imagen)
        {
            InitializeComponent();
            DataContext = this;
            imagenElegida.Source = imagen;
            if (!esNarrador)
            {
                textBoxPista.Visibility = Visibility.Collapsed;
            }

        }

        private void ClicButtonEnviarPista(object sender, RoutedEventArgs e)
        {
            Pista = textBoxPista.Text;
            if (!string.IsNullOrWhiteSpace(Pista) && Pista.Contains(" "))
            {
                //TODO: I18n
                MessageBox.Show("La pista no puede estar en blanco o contener espacios");
                DialogResult = false;
                this.Close();
                return;
            }
            DialogResult = true;
        }


        private void ClicButtonCerrar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
