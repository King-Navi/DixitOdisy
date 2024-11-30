using System.Windows.Controls;
using WpfCliente.Persistencia;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class CambiarIdiomaMenuDesplegable : UserControl 
    {

        public CambiarIdiomaMenuDesplegable()
        {
            InitializeComponent();
        }

        private void SelecionarIdioma(object sender, SelectionChangedEventArgs e)
        {
            if (cambiarIdiomaMenuDesplegable.SelectedItem is ComboBoxItem seleccionado)
            {
                string lenguajeSelecionado = seleccionado.Tag.ToString();
                IdiomaGuardo.SeleccionarIdioma(lenguajeSelecionado);
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
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloIdiomaInvalido, Properties.Idioma.mensajeIdiomaInvalido, this);
                    break;
            }
            WpfCliente.Properties.Settings.Default.Save();
        }

        private void ClicImagenIdioma(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cambiarIdiomaMenuDesplegable.IsDropDownOpen = true;
        }
    }
    
}
