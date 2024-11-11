using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
