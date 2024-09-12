﻿using System;
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
            //FIXME No es correcta la implementacion trata de ocupar el tag del combobox en vez de su index (selecionarIdiomaMenuDesplegable)
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
                    //TODO manejar el default
                    IdiomaGuardo.GuardarInglesUS();
                    MessageBox.Show("Selección de idioma inválida. Se ha configurado el idioma predeterminado.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }
            WpfCliente.Properties.Settings.Default.Save();
        }

        public void ActualizarUI()
        {
            //throw new NotImplementedException();
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }
    }
    
}
