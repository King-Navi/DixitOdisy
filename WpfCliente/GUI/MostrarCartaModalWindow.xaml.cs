using System;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class MostrarCartaModalWindow : Window, IActualizacionUI
    {
        public string Pista {  get; set; }
        private readonly Action enCierreAccion;
        public MostrarCartaModalWindow(bool esNarrador, BitmapImage imagen, Action cerrarVenana)
        {
            InitializeComponent();
            ActualizarUI();
            DataContext = this;
            cerrarVenana = enCierreAccion;
            imagenElegida.Source = imagen;
            if (!esNarrador)
            {
                textBoxPista.Visibility = Visibility.Collapsed;
            }
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento; ActualizarUI();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            enCierreAccion?.Invoke();
        }

        private void ClicButtonEnviarPista(object sender, RoutedEventArgs e)
        {
            Pista = textBoxPista.Text;
            if (string.IsNullOrWhiteSpace(Pista) || Pista.Contains(" "))
            {
                textBlockAdvertenciaPista.Visibility = Visibility.Visible;
                textBlockAdvertenciaPista.Text = Idioma.mensajeAdvertenciaPista;
                return;
            }
            DialogResult = true;
        }

        private void VentanaRatonAbajo(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            { 
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion); 
            }
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
            textBlockAdvertenciaPista.Text = Idioma.mensajeAdvertenciaPista;
            this.Title = Properties.Idioma.tituloMostrarCarta;  
            buttonSeleccionar.Content = Properties.Idioma.buttonAceptar;
        }
    }
}
