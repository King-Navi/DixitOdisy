using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{

    public partial class SeleccionCartaUsercontrol : UserControl, IActualizacionUI
    {
        public ObservableCollection<ImagenCarta> Imagenes { get; set; }

        public SeleccionCartaUsercontrol(ObservableCollection<ImagenCarta> imagenCartas)
        {
            InitializeComponent();
            Imagenes = imagenCartas;
            Loaded += LoadedSeleccionCartaUsercontrol;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            DataContext = this;
            labelPista.Content = Properties.Idioma.labelEsperandoPista;
        }

   
        private void LoadedSeleccionCartaUsercontrol(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Window mainWindow)
            {
                mainWindow.MinHeight = this.MinHeight;
                mainWindow.MinWidth = this.MinWidth;
            }
        }

        private void UnloadedSeleccionCartaUsercontrol(object sender, RoutedEventArgs e)
        {
            this.Loaded -= LoadedSeleccionCartaUsercontrol;

        }

        public void ColocarPista(string pista)
        {
            labelPista.Content = "Pista : " + pista;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
        }

        private void UnloadedSeleccionCartasUsercontrol(object sender, RoutedEventArgs e)
        {
            this.Loaded -= LoadedSeleccionCartaUsercontrol;
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
        }
    }
}
