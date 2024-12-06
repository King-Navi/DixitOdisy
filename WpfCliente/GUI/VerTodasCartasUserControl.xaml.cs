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
    public partial class VerTodasCartasUserControl : UserControl , IActualizacionUI
    {
        public ObservableCollection<ImagenCarta> TodasImagenes { get; set; }

        public VerTodasCartasUserControl()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            SingletonPartida.Instancia.MostrarPista += ColocarPista;
            TodasImagenes = SingletonGestorImagenes.Instancia.imagenesDeTodos.ImagenCartasTodos;
            DataContext = this;
            ActualizarUI();
        }

        public void ActualizarUI()
        { 
            textBoxInstruccionPista.Text = Properties.Idioma.labelInstruccionPista;
        }

        public void ColocarPista(string pista)
        {
            labelPista.Content = Properties.Idioma.labelPista + pista;
        }


        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private void CerrandoUserControl(object sender, RoutedEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
        }
    }
}
