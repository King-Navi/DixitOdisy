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
using WpfCliente.Interfaz;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for ErrorConexionModalWindow.xaml
    /// </summary>
    public partial class ErrorConexionModalWindow : Window, IActualizacionUI
    {
        public ErrorConexionModalWindow()
        {
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
            InitializeComponent();
        }
        public ErrorConexionModalWindow(string tituloVentana, string tituloEncabezado, string contenido)
        {
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
            InitializeComponent();
            this.Title = tituloVentana;
            labelTitulo.Content = tituloEncabezado;
            labelContenido.Text = contenido;

        }

        public void ActualizarUI()
        {
            //Pedirle a unaay el archivo .resx
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private void ClicButtonAceptar(object sender, RoutedEventArgs e)
        {

        }
    }
}
