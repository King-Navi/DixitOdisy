using System;
using System.Windows;
using WpfCliente.Interfaz;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for UniserSalaModalWindow.xaml
    /// </summary>
    public partial class UnirseSalaModalWindow : Window, IActualizacionUI
    {
        public string ValorIngresado { get; private set; }

        public UnirseSalaModalWindow()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }


        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelIngresarCodigoSala.Content = Properties.Idioma.labelIngresarCodigoSala;
            buttonAceptar.Content = Properties.Idioma.buttonAceptar;
            this.Title = Properties.Idioma.tituloIngresarCodigoSala;
        }

        private void ClicButtonAceptar(object sender, RoutedEventArgs e)
        {
            ValorIngresado = textBoxCodigoSala.Text.ToUpper();
            
            SalaEspera salaWindow = new SalaEspera(ValorIngresado);
            salaWindow.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window != salaWindow) // No cerrar la nueva ventana
                {
                    window.Close();
                }
            }
        }
    }
}
