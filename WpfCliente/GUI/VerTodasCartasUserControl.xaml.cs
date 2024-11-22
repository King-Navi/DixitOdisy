using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
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
