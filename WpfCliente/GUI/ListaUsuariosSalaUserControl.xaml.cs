using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for ListaUsuariosSalaUserControl.xaml
    /// </summary>
    public partial class ListaUsuariosSalaUserControl : UserControl
    {
        public ObservableCollection<Usuario> JugadoresEnSala { get; set; } = new ObservableCollection<Usuario>();

        public ListaUsuariosSalaUserControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void ObtenerUsuarioSala(Usuario usuario, List<Amigo> amigos) //FIXME: Falta terminar los amigos
        {
            if (usuario == null)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCargarAmigosFalla, Properties.Idioma.mensajeCargarAmigosFalla, this);
            }
            else
            {
                // Agregar el amigo a la colección ObservableCollection, esto actualizará automáticamente el ItemsControl
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
