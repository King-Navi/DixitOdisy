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
using WpfCliente.Properties;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
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
            Window menuVentana = Window.GetWindow(this);
            EditarPerfilWindow editarPerfilVentana = new EditarPerfilWindow(menuVentana);
            editarPerfilVentana.Show();
            Window MenuVentana = Window.GetWindow(this);  
            if (MenuVentana != null)
            {
                editarPerfilVentana.Closed += (s, args) => {
                    try
                    {
                        MenuVentana.Show();
                    }
                    catch (Exception)
                    {

                    }
                };
                MenuVentana.Hide();
            }
        }

        private void CerrarSesion(object sender, RoutedEventArgs e)
        {
            Window menuVentana = Window.GetWindow(this);
            menuVentana.Close();
        }

        public void ActualizarUI()
        {
            menuItemEditarPerfil.Header = Idioma.menuItemEditarperfil;
            menuItemEstadisticas.Header = Idioma.menuItemEstadisticas;
            menuItemCerrarSesion.Header = Idioma.buttonCerrarSesion;
            menuItemPerfil.Header = Idioma.menuItemPerfil;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }
    }
}
