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
using System.Windows.Shapes;
using WpfCliente.Contexto;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class ReconectandoWindow : Window , IActualizacionUI , IHabilitadorBotones
    {
        public ReconectandoWindow()
        {
            InitializeComponent();
            CerrarTodasVentanas();
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            Title =Idioma.tituloReconectando;
            buttonReconectar.Content = Idioma.buttonReconectar;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private void CerrarTodasVentanas()
        {
            foreach (Window ventana in Application.Current.Windows)
            {
                try
                {
                    if (!(ventana is ReconectandoWindow))
                    {
                        ventana.Close();
                    }
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                }
            }
        }

        private void ClicButtonReconectar(object sender, RoutedEventArgs e)
        {
            HabilitarBotones(false);
            if (IntentarAbrirVentanaMenu())
            {
                CerrarVentanaActual();
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloReconexionFallida, Idioma.mensajeIntentoReconexion, this);
            }
            HabilitarBotones(true);
        }

        private bool IntentarAbrirVentanaMenu()
        {
            try
            {
                return SingletonGestorVentana.Instancia.NavegarA(new MenuPage());
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex, this);
            }
            return false;
        }

        private void CerrarVentanaActual()
        {
            try
            {
                this.Close();
            }
            catch (Exception excepcion)
            {
                Application.Current.Shutdown();
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, this);
            }
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            buttonReconectar.IsEnabled = esHabilitado;
        }
    }
}
