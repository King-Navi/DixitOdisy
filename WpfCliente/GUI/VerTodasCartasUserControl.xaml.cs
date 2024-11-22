using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for VerTodasCartasUserControl.xaml
    /// </summary>
    public partial class VerTodasCartasUserControl : UserControl , IActualizacionUI
    {
        public ObservableCollection<ImagenCarta> TodasImagenes { get; set; }



        public VerTodasCartasUserControl(ObservableCollection<ImagenCarta> todasImagenes)
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            TodasImagenes = todasImagenes;
            DataContext = this;
        }

        public void ActualizarUI()
        {

        }


        public void ColocarPista(string pista)
        {
            labelPista.Content = "Pista : " + pista;
        }


        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }
        private void UnloadedVerTodasCartasUsercontrol(object sender, RoutedEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
        }


    }
}
