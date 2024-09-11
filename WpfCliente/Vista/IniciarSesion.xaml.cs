using System;
using System.Windows;
using System.Windows.Controls;
using UtilidadesLibreria;

namespace WpfCliente.Vista
{
    public partial class IniciarSesion : Window
    {
        public IniciarSesion()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
        }

        private void SelecionarIdioma(object sender, SelectionChangedEventArgs e)
        {
            if (selecionarIdiomaMenuDesplegable.SelectedItem is ComboBoxItem itemSeleccionado)
            {
                string lenguajeSelecionado = itemSeleccionado.Tag.ToString();
                IdiomaGuardo.SeleccionarIdioma(lenguajeSelecionado);
                ActualizarRecursosUI();
                CambiarIdioma.EnCambioIdioma();
                GuardarConfiguracionIdioma();
            }

        }

        private void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarRecursosUI();
        }

        private void ActualizarRecursosUI()
        {
            labelIniciarSesion.Content = Properties.Idioma.labelUsuario;

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
    }
}
