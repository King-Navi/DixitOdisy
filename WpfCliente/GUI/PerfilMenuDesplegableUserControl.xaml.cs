using System;
using System.Windows;
using System.Windows.Controls;
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
            imagenPerfil.Source = SingletonCliente.Instance.FotoJugador;
        }

        private void AbrirEditarVentana(object sender, RoutedEventArgs e)
        {
            Window menuVentana = Window.GetWindow(this);
            EditarPerfilWindow editarPerfilVentana = new EditarPerfilWindow();
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
                        Application.Current.Shutdown();
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
