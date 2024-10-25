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
        public ObservableCollection<Usuario> UsuariosSala { get; set; } = new ObservableCollection<Usuario>();

        public ListaUsuariosSalaUserControl()
        {
            InitializeComponent();
            DataContext = this;
        }


        public void ObtenerUsuarioSala(List<Usuario> usuarios, List<Amigo> amigos)
        {
            foreach (var usuario in usuarios)
            {
                foreach(var amigo in amigos)
                {
                    if (usuario.Nombre.Equals(amigo.Nombre, StringComparison.OrdinalIgnoreCase) )
                    {
                        
                    }
                }
            }
        }
    }
}
