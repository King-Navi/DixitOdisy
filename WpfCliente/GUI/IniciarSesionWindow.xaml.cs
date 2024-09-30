using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using UtilidadesLibreria;
using WpfCliente.GUI;
using WpfCliente.UsuarioControl;
using WpfCliente.Utilidad;

namespace WpfCliente.Vista
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
        private void ButtonClicIniciarSesion(object sender, RoutedEventArgs e)
        {
            //Este metodo es de prueba
            Singleton.Instance.nombreUsuario = textBoxUsuario.Text;

            //TODO: antes de abrir la nueva ventana se tiene que hacer la logica de inicio de sesion con validacion y respuesta del servidor

            AbrirVentanaMenu();
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
            //Esta es la que no es como ivnitado if invitado colocar bool true
            stackPanePrincipal.Children.Add(new RegistrarUsuario());
        }
    }
}
