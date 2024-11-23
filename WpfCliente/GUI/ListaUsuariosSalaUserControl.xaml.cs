using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class ListaUsuariosSalaUserControl : UserControl
    {
        public ObservableCollection<Usuario> JugadoresEnSala { get; set; } = new ObservableCollection<Usuario>();

        public ListaUsuariosSalaUserControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void ObtenerUsuarioSala(Usuario usuario)
        {
            if (usuario == null)
            {
                VentanasEmergentes.CrearVentanaEmergenteCargarDatosAmigosFalla(this);
            }
            else
            {
                JugadoresEnSala.Add(usuario);
            }
        }

        public void EliminarUsuarioSala(Usuario usuario)
        {
            var usuarioAEliminar = JugadoresEnSala.FirstOrDefault(busqueda => busqueda.Nombre == usuario.Nombre);
            if (usuarioAEliminar != null)
            {
                JugadoresEnSala.Remove(usuarioAEliminar);
            }
        }

        private void LimpiarItemsControl()
        {
            JugadoresEnSala.Clear();
        }
    }
}
