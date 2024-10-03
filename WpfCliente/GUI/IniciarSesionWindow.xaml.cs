using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UtilidadesLibreria;
using WpfCliente.Interfaz;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class IniciarSesion : Window, IActualizacionUI
    {
        public IniciarSesion()
        {

            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            //stackPaneEncabezado.Children.Add(new CambiarIdioma());
            ActualizarUI();
        }

        private void SelecionarIdioma(object sender, SelectionChangedEventArgs e)
        {
            if (selecionarIdiomaMenuDesplegable.SelectedItem is ComboBoxItem itemSeleccionado)
            {
                string lenguajeSelecionado = itemSeleccionado.Tag.ToString();
                IdiomaGuardo.SeleccionarIdioma(lenguajeSelecionado);
                ActualizarUI();
                CambiarIdioma.EnCambioIdioma();
                GuardarConfiguracionIdioma();
            }

        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelTitulo.Content = Properties.Idioma.tituloBienvenida;
            labelIniciarSesion.Content = Properties.Idioma.labelInicioSesion;
            labelUsuario.Content = Properties.Idioma.labelUsuario;
            labelContrasenia.Content = Properties.Idioma.labelContrasenia;
            buttonIniciarSesion.Content = Properties.Idioma.buttonIniciarSesion;
            buttonRegistrar.Content = Properties.Idioma.buttonRegistrarse;

        }
        private void GuardarConfiguracionIdioma()
        {
            //FIXME No es correcta la implementacion trata de ocupar el tag del combobox en vez de su index (selecionarIdiomaMenuDesplegable)
            int seleccion = selecionarIdiomaMenuDesplegable.SelectedIndex;
            switch (seleccion)
            {
                case 0:
                    IdiomaGuardo.GuardarEspañolMX();
                    break;
                case 1:
                    IdiomaGuardo.GuardarInglesUS();
                    break;
                default:
                    //TODO manejar el default
                    IdiomaGuardo.GuardarInglesUS();
                    MessageBox.Show("Selección de idioma inválida. Se ha configurado el idioma predeterminado.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }
            Properties.Settings.Default.Save();
        }

        private void EnCierre(object sender, EventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
        }
        private async void ButtonClicIniciarSesion(object sender, RoutedEventArgs e)
        {
            Task<bool> verificarConexion = Validacion.ValidarConexion();
            HabilitarBotones(false);
            gridCarga.Visibility = Visibility.Visible;
            if (! await verificarConexion)
            {
                ErrorConexionModalWindow ventanaModal = new ErrorConexionModalWindow();
                ventanaModal.Owner = this;
                ventanaModal.ShowDialog();
                gridCarga.Visibility = Visibility.Collapsed;
                HabilitarBotones(true);
                return;
            }
            gridCarga.Visibility = Visibility.Collapsed;

            //TODO: Validar el textbox y el passwordbox
            Singleton.Instance.NombreUsuario = textBoxUsuario.Text;

            //TODO: antes de abrir la nueva ventana se tiene que hacer la logica de inicio de sesion con validacion y respuesta del servidor

            AbrirVentanaMenu();
        }

        private void HabilitarBotones(bool esValido)
        {
            textBoxUsuario.IsEnabled = esValido;
            textBoxContrasenia.IsEnabled = esValido;
            buttonIniciarSesion.IsEnabled = esValido;
            buttonRegistrar.IsEnabled = esValido;
        }

        private void AbrirVentanaMenu() 
        {
            MenuWindow nuevaVentana = new MenuWindow();
            nuevaVentana.Show();
            this.Close();
        }

        private void ButtonClicRegistrar(object sender, RoutedEventArgs e)
        {
            stackPanePrincipal.Children.Clear();
            //Esta es la que no es como invitado, si es un invitado colocar bool true
            stackPanePrincipal.Children.Add(new RegistrarUsuario());
        }
    }
}
