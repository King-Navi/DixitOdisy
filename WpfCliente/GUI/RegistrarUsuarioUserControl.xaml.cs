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
using UtilidadesLibreria;

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
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }public RegistrarUsuario(bool esInvitado)
        {
            this.esInivtado = esInvitado;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            InitializeComponent();
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            //labelSelecionUsuario.Content = WpfCliente.Properties.Idioma.labe; 
            labelContrania.Content = WpfCliente.Properties.Idioma.labelContrasenia; 
            labelUsuario.Content = WpfCliente.Properties.Idioma.labelUsuario;
            //labelRegistro.Content = WpfCliente.Properties.Idioma.label;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }
    }
}
