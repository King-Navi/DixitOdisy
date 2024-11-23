using System;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfCliente.Interfaz;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class MostrarCartaModalWindow : Window, IActualizacionUI
    {
        public string Pista {  get; set; }
        public MostrarCartaModalWindow(bool esNarrador, BitmapImage imagen)
        {
            InitializeComponent();
            DataContext = this;
            imagenElegida.Source = imagen;
            if (!esNarrador)
            {
                textBoxPista.Visibility = Visibility.Collapsed;
            }
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento; ActualizarUI();
        }

        private void ClicButtonEnviarPista(object sender, RoutedEventArgs e)
        {
            Pista = textBoxPista.Text;
            if (!string.IsNullOrWhiteSpace(Pista) && Pista.Contains(" "))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.labelEsperandoPista,
                    Properties.Idioma.mensajeAdvertenciaPista, this);
                DialogResult = false;
                this.Close();
                return;
            }
            DialogResult = true;
        }


        private void ClicButtonCerrar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            this.Title = Properties.Idioma.tituloMostrarCarta;  
            buttonSeleccionar.Content = Properties.Idioma.buttonAceptar;
        }
    }
}
