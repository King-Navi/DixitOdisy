using System;
using System.Windows;
using WpfCliente.Interfaz;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{ 
    public partial class VentanaEmergenteModalWindow : Window, IActualizacionUI
    {
        public VentanaEmergenteModalWindow(string titulo, string descripcion)
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();

            labelTituloVentanaEmergente.Content = titulo;
            textBlockDescripcionVentanaEmergente.Text = descripcion;

        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            this.Title = Properties.Idioma.tituloVentanaEmergente;
            buttonAceptar.Content = Properties.Idioma.buttonAceptar;
        }


        private void ClicButtonAceptar(object sender, RoutedEventArgs e)
        {
            TratarCerrarVentana();
        }
        private void TratarCerrarVentana()
        {
            try
            {
                this.Close();
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
        }
    }
}
