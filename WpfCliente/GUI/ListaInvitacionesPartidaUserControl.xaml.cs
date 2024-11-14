using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para ListaInvitacionesPartidaUserControl.xaml
    /// </summary>
    public partial class ListaInvitacionesPartidaUserControl : UserControl, IServicioInvitacionPartidaCallback, IActualizacionUI
    {
        public ObservableCollection<InvitacionPartida> InvitacionesPartida { get; set; } = new ObservableCollection<InvitacionPartida>();
        public ListaInvitacionesPartidaUserControl()
        {
            InitializeComponent();
            this.DataContext = this;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }
        public void RecibirInvitacion(InvitacionPartida invitacion)
        {
            // Agregar la invitación a la colección observable en el hilo de la UI.
            Application.Current.Dispatcher.Invoke(() =>
            {
                AgregarInvitacion(invitacion);
            });
        }

        public void AgregarInvitacion(InvitacionPartida invitacion)
        {
            InvitacionesPartida.Add(invitacion);
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelListaInvitaciones.Content = "Lista de invitaciones a partida";
        }
    }
}
