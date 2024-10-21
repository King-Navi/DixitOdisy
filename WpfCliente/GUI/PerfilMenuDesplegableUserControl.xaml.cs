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
using WpfCliente.Interfaz;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for PerfilMenuDesplegableUserControl.xaml
    /// </summary>
    public partial class PerfilMenuDesplegableUserControl : UserControl , IActualizacionUI
    {
        public PerfilMenuDesplegableUserControl()
        {
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            InitializeComponent();
            ActualizarUI();
        }

        private void AbrirEditarVentana(object sender, RoutedEventArgs e)
        {

            EditarPerfilWindow editarPerfilVentana = new EditarPerfilWindow();
            editarPerfilVentana.Show();
            Window MenuVentana = Window.GetWindow(this);  
            if (MenuVentana != null)
            {
                editarPerfilVentana.Closed += (s, args) => {
                    MenuVentana.Show();
                };
                MenuVentana.Hide();
            }

        }

        public void ActualizarUI()
        {
            //Preguntar a unaay por recursos
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }
    }
}
