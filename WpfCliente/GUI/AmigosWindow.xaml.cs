using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
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
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para AmigosWindow.xaml
    /// </summary>
    public partial class AmigosWindow : Window
    {
        private int contadorClics = 0;
        private const int LIMITE_CLICS = 2;
        public AmigosWindow(MenuWindow menuWindow)
        {
            InitializeComponent();
        }

        private void FlechaAtras_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void FlechaRecargar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (contadorClics >= LIMITE_CLICS)
            {
                MessageBox.Show("Ya no puede hacer más solicitudes profe.", "Límite alcanzado", MessageBoxButton.OK, MessageBoxImage.Warning);
                imagenFlechaRecargar.Visibility = Visibility.Collapsed;
                return;
            }
            contadorClics++;
            RecargarSolicitudes();
        }

        private void RecargarSolicitudes()
        {
            var contenedorPadre = (Panel)listaSolicitudesAmistadUserControl.Parent;
            if (contenedorPadre != null)
            {
                contenedorPadre.Children.Remove(listaSolicitudesAmistadUserControl);

                ListaSolicitudesAmistadUserControl nuevaListaSolicitudes = new ListaSolicitudesAmistadUserControl();
                contenedorPadre.Children.Add(nuevaListaSolicitudes);

                listaSolicitudesAmistadUserControl = nuevaListaSolicitudes; 
            }
        }

        private void NuevaSolicitud_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TryEnviarSolicitud();            
        }

        private async void TryEnviarSolicitud()
        {
            string gamertagSolicitud = AbrirVentanaModalGamertag();
            if (gamertagSolicitud != null && gamertagSolicitud != Singleton.Instance.NombreUsuario)
            {
                try {
                    if (await EnviarSolicitud(gamertagSolicitud))
                    {
                        VentanasEmergentes.CrearVentanaEmergenteSolicitudEnviada(this);
                    }
                }
                catch (FaultException<SolicitudAmistadFalla> ex)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Idioma.mensajeErrorInesperado, ex.Detail.Mensaje, this);
                }
                catch (FaultException ex)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Idioma.mensajeErrorInesperado, ex.Message, this);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Idioma.mensajeErrorInesperado, ex.Message, this);
                }
            }
        }

        private string AbrirVentanaModalGamertag()
        {
            string valorObtenido = null;
            IngresarGamertagModalWindow ventanaModal = new IngresarGamertagModalWindow();
            try
            {
                ventanaModal.Owner = this;

            }
            catch (Exception)
            {
                //TODO
            }
            bool? resultado = ventanaModal.ShowDialog();

            if (resultado == true)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }

            return valorObtenido;
        }

        private async Task<bool> EnviarSolicitud(string gamertagReceptor)
        {
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return false;
            }

            try
            {
                Usuario usuarioRemitente = new Usuario();
                usuarioRemitente.Nombre = Singleton.Instance.NombreUsuario;
                usuarioRemitente.FotoUsuario = Imagen.ConvertirBitmapImageAMemoryStream(Singleton.Instance.FotoJugador);

                var resultado = Conexion.Amigos.EnviarSolicitudAmistad(usuarioRemitente, gamertagReceptor);
                return resultado;
            }
            catch (Exception excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(excepcion.InnerException.ToString(), excepcion.StackTrace, this);
                return false;
            }
        }

        private void HabilitarBotones(bool esHabilitado)
        {
            imagenFlechaRecargar.IsEnabled = esHabilitado;
            imagenFlechaAtras.IsEnabled = esHabilitado;
            imagenAgregarAmigo.IsEnabled = esHabilitado;

            imagenFlechaRecargar.Opacity = esHabilitado ? 1.0 : 0.5;  
            imagenFlechaAtras.Opacity = esHabilitado ? 1.0 : 0.5;
            imagenAgregarAmigo.Opacity = esHabilitado ? 1.0 : 0.5;
        }
    }
}
