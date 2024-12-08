using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{

    public partial class SeleccionCartaUserControl : UserControl, IActualizacionUI
    {
        public ObservableCollection<ImagenCarta> Imagenes { get; set; }

        public SeleccionCartaUserControl()
        {
            InitializeComponent();
            Imagenes = SingletonGestorImagenes.Instancia.imagnesMazo.ImagenesMazo;
            SingletonPartida.Instancia.MostrarPista += ColocarPista;
            Loaded += SeleccionCartaUsercontrolLoaded;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            DataContext = this;
            labelPista.Content = Properties.Idioma.labelEsperandoPista;
            ActualizarUI();
        }

   
        private void SeleccionCartaUsercontrolLoaded(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Window mainWindow)
            {
                mainWindow.MinHeight = this.MinHeight;
                mainWindow.MinWidth = this.MinWidth;
            }
        }

        private void CerrandoSeleccionCartaUsercontrol(object sender, RoutedEventArgs e)
        {
            this.Loaded -= SeleccionCartaUsercontrolLoaded;

        }

        public void ColocarPista(string pista)
        {
            labelPista.Content = Properties.Idioma.labelPista + pista;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelInstrucciones.Text = Properties.Idioma.labelInstruccionPistaConfundir;
        }

        private void UnloadedSeleccionCartasUsercontrol(object sender, RoutedEventArgs e)
        {
            this.Loaded -= SeleccionCartaUsercontrolLoaded;
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
        }
    }
}
