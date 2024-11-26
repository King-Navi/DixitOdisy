using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class SolicitudAmistadUserControl : UserControl, IHabilitadorBotones, IActualizacionUI
    {
        private SolicitudAmistad solicitudAmistadActual;

        public SolicitudAmistadUserControl()
        {
            InitializeComponent();
            ColocarFondoColorAleatorio();
            DataContextChanged += SolicitudAmistadUserControlCambioDataContext;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
        }

        private void SolicitudAmistadUserControlCambioDataContext(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is SolicitudAmistad solicitud && solicitud != null)
            {
                labelNombreAmigo.Content = solicitud.Remitente.Nombre;
                imageAmigo.Source = Imagen.ConvertirStreamABitmapImagen(solicitud.Remitente.FotoUsuario);
                solicitudAmistadActual = solicitud;
            }
        }

        private void ColocarFondoColorAleatorio()
        {
            this.Background = Utilidades.ObtenerColorAleatorio();
        }

        private void ClicButtonAceptar(object sender, RoutedEventArgs e)
        {
            _ = AceptarSolicitudAsync(solicitudAmistadActual);
        }

        private async Task<bool> AceptarSolicitudAsync(SolicitudAmistad solicitud)
        {
            Window window = Window.GetWindow(this);
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, window);
            if (!conexionExitosa)
            {
                return false;
            }

            try
            {
                var resultado = Conexion.Amigos.AceptarSolicitudAmistad(solicitudAmistadActual.Remitente.IdUsuario, SingletonCliente.Instance.IdUsuario);

                if (resultado)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloSolicitudAmistad, Properties.Idioma.mensajeSolicitudAmistadAceptada + solicitudAmistadActual.Remitente.Nombre, window);
                    HabilitarBotones(false);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(ex);
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCargarAmigosFalla, Properties.Idioma.mensajeCargarAmigosFalla, this);
                return false;
            }
        }

        private void ClicButtonRechazar(object sender, RoutedEventArgs e)
        {
            _ = RechazarSolicitudAsync(solicitudAmistadActual);
        }

        private async Task<bool> RechazarSolicitudAsync(SolicitudAmistad solicitud)
        {
            Window window = Window.GetWindow(this);
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, window);
            if (!conexionExitosa)
            {
                return false;
            }

            try
            {
                var resultado = Conexion.Amigos.RechazarSolicitudAmistad(solicitudAmistadActual.Remitente.IdUsuario, SingletonCliente.Instance.IdUsuario);

                if (resultado)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloSolicitudAmistad, Properties.Idioma.mensajeSolicitudAmistadRechazada + solicitudAmistadActual.Remitente.Nombre, window);
                    HabilitarBotones(false);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(ex);
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCargarAmigosFalla, Properties.Idioma.mensajeCargarAmigosFalla, this);
                return false;
            }
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            buttonAceptar.IsEnabled = esHabilitado; 
            buttonRechazar.IsEnabled = esHabilitado;

            buttonAceptar.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA : Utilidades.OPACIDAD_MINIMA;
            buttonRechazar.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA : Utilidades.OPACIDAD_MINIMA;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            buttonAceptar.Content = Properties.Idioma.buttonAceptar;
            buttonRechazar.Content = Properties.Idioma.buttonRechazar;
        }

        private void CerrandoUserControl(object sender, RoutedEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;

        }
    }
}
