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
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for ResumenRondaUserControl.xaml
    /// </summary>
    public partial class ResumenRondaUserControl : UserControl
    {
        //TODO: esto es solo si no queremos mostrar los nombres
        public ObservableCollection<Usuario> JugadorEstadisticas { get; set; }
        public ObservableCollection<string> Podio { get; set; }
        public ResumenRondaUserControl()
        {
            InitializeComponent();
            DataContext = this;
        }
        public ResumenRondaUserControl(ObservableCollection<Usuario> listaJugadores, ObservableCollection<string> podio)
        {
            InitializeComponent();
            DataContext = this;
            JugadorEstadisticas = listaJugadores;
            Podio = podio;
        }
        public void MostrarEnPodio(Usuario primerLugar, Usuario segundoLugar, Usuario tercerLugar)
        {
            labelPrimerLugar.Content = "CAMBIIIIIIIIIIIII";
            if (labelPrimerLugar != null)
            {
                labelPrimerLugar.Content = "Podio actualizado"; // Confirmar que el cambio es visible
                Application.Current.Dispatcher.Invoke(() =>
                {
                    labelPrimerLugar.Content = "Podio actualizado desde Dispatcher";
                });
            }
            else
            {
                MessageBox.Show("labelPrimerLugar no está inicializado");
            }


            //Podio[0] = primerLugar?.Nombre ?? "No hay primer lugar"; // Primer lugar
            //Podio[1] = segundoLugar?.Nombre ?? "No hay segundo lugar"; // Segundo lugar
            //Podio[2] = tercerLugar?.Nombre ?? "No hay tercer lugar"; // Tercer lugar

        }
    }
}
