using System;
using System.Collections.Generic;
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

namespace WpfCliente.UsuarioControl
{
    /// <summary>
    /// Lógica de interacción para RegistrarUsuario.xaml
    /// </summary>
    public partial class RegistrarUsuario : UserControl, IActualizacionUI
    {
        private Boolean esInivtado;
        public RegistrarUsuario()
        {
            InitializeComponent();
        }public RegistrarUsuario(bool esInvitado)
        {
            this.esInivtado = esInvitado;
            InitializeComponent();
        }

        public void ActualizarUI()
        {
            throw new NotImplementedException();
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
