using System.Collections.ObjectModel;
using System.Windows.Controls;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.GUI
{
    public partial class ListaUsuariosSalaUserControl : UserControl
    {
        public ObservableCollection<Usuario> JugadoresEnSala { get; set; } = new ObservableCollection<Usuario>();

        public ListaUsuariosSalaUserControl()
        {
            InitializeComponent();
            DataContext = this;
            JugadoresEnSala = SingletonSalaJugador.Instancia.JugadoresSala;
        }
    }
}
