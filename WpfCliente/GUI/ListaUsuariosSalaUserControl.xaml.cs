using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
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
                MessageBox.Show("No se pudo cargar los datos de tus amigos");
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
