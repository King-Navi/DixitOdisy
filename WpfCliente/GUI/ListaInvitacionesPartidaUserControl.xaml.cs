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
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para ListaInvitacionesPartidaUserControl.xaml
    /// </summary>
    public partial class ListaInvitacionesPartidaUserControl : UserControl, IServicioInvitacionPartidaCallback
    {
        public ObservableCollection<InvitacionPartida> InvitacionesPartida { get; set; } = new ObservableCollection<InvitacionPartida>();
        public ListaInvitacionesPartidaUserControl()
        {
            InitializeComponent();
            this.DataContext = this;
            InvitacionPartida invitacion = new InvitacionPartida();
            InvitacionesPartida.Add(invitacion);
            InvitacionPartida invitacion1 = new InvitacionPartida();
            InvitacionesPartida.Add(invitacion1);
            InvitacionPartida invitacion2 = new InvitacionPartida();
            InvitacionesPartida.Add(invitacion2);
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
    }
}
