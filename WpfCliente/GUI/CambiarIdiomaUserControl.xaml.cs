using System;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.Interfaz;
using WpfCliente.Persistencia;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for CambiarIdiomaMenuDesplegable.xaml
    /// </summary>
    public partial class CambiarIdiomaMenuDesplegable : UserControl , IActualizacionUI
    {

        public CambiarIdiomaMenuDesplegable()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;

        }

        private void SelecionarIdioma(object sender, SelectionChangedEventArgs e)
        {
            if (cambiarIdiomaMenuDesplegable.SelectedItem is ComboBoxItem itemSeleccionado)
            {
                string lenguajeSelecionado = itemSeleccionado.Tag.ToString();
                IdiomaGuardo.SeleccionarIdioma(lenguajeSelecionado);
                ActualizarUI();
                CambiarIdioma.EnCambioIdioma();
                GuardarConfiguracionIdioma();
            }
        }

        private void GuardarConfiguracionIdioma()
        {
            int seleccion = cambiarIdiomaMenuDesplegable.SelectedIndex;
            switch (seleccion)
            {
                case 0:
                    IdiomaGuardo.GuardarEspañolMX();
                    break;
                case 1:
                    IdiomaGuardo.GuardarInglesUS();
                    break;
                default:
                    IdiomaGuardo.GuardarInglesUS();
                    VentanasEmergentes.CrearVentanaEmergenteIdiomaInvalido(this);
                    break;
            }
            WpfCliente.Properties.Settings.Default.Save();
        }

        public void ActualizarUI()
        {
            //TODO: Pedirle a unaay los .resx
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private void CerrarControl(object sender, RoutedEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
        }

        private void clicImagenIdioma(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cambiarIdiomaMenuDesplegable.IsDropDownOpen = true;
        }
    }
    
}
