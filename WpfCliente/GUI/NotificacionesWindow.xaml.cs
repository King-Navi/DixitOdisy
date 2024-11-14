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
using System.Windows.Shapes;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para NotificacionesWindow.xaml
    /// </summary>
    public partial class NotificacionesWindow : Window, IServicioInvitacionPartidaCallback
    {
        public ObservableCollection<InvitacionPartida> Invitaciones { get; private set; }

        public NotificacionesWindow(MenuWindow menuWindow)
        {
            InitializeComponent();
            Invitaciones = new ObservableCollection<InvitacionPartida>();

            this.DataContext = this;

            menuWindow.InvitacionRecibida += ManejarInvitacionRecibida;
            this.Closed += (s, e) => menuWindow.InvitacionRecibida -= ManejarInvitacionRecibida;
        }

        private void ManejarInvitacionRecibida(InvitacionPartida invitacion)
        {
            Invitaciones.Add(invitacion);
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        public void RecibirInvitacion(InvitacionPartida invitacion)
        {
            throw new NotImplementedException();
        }
    }
}
