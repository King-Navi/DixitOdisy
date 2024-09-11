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
using WpfCliente;
namespace UtilidadesLibreria.UsuarioControl
{
    /// <summary>
    /// Interaction logic for CambiarIdiomaMenuDesplegable.xaml
    /// </summary>
    public partial class CambiarIdiomaMenuDesplegable : UserControl
    {

        public CambiarIdiomaMenuDesplegable()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;

        }

        private void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SelecionarIdioma(object sender, SelectionChangedEventArgs e)
        {
            if (cambiarIdiomaMenuDesplegable.SelectedItem is ComboBoxItem itemSeleccionado)
            {
                string lenguajeSelecionado = itemSeleccionado.Tag.ToString();
                IdiomaGuardo.SeleccionarIdioma(lenguajeSelecionado);
                ActualizarRecursosUI();
                CambiarIdioma.EnCambioIdioma();
                //GuardarConfiguracionIdioma();
            }
        }
        private void ActualizarRecursosUI()
        {
            throw new NotImplementedException();

        }
        
    }
    
}
