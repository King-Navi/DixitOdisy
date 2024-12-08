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
    public partial class NarradorSeleccionCartaUserControl : UserControl , IActualizacionUI
    {
        public ObservableCollection<ImagenCarta> Imagenes { get; set; }

        public NarradorSeleccionCartaUserControl()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento; 
            Imagenes = SingletonGestorImagenes.Instancia.imagnesMazo.ImagenesMazo;
            this.Loaded += LoadedNarradorSeleccionCartaUsercontrol;
            DataContext = this;
            ActualizarUI();
        }


        private void LoadedNarradorSeleccionCartaUsercontrol(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Window mainWindow)
            {
                mainWindow.MinHeight = this.MinHeight;
                mainWindow.MinWidth = this.MinWidth;
            }
        }

        private void CerrandoNarradorSeleccionCartaUsercontrol(object sender, RoutedEventArgs e)
        {
            this.Loaded -= LoadedNarradorSeleccionCartaUsercontrol;

        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelNarrador.Content = Properties.Idioma.labelNarrador;
            labelInstruccionNarrador.Text = Properties.Idioma.labelInstruccionNarrador;
        }

        private void CerrandoUserControl(object sender, RoutedEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;

        }
    }
}
