using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para SolicitudAmistadUserControl.xaml
    /// </summary>
    public partial class SolicitudAmistadUserControl : UserControl, IHabilitadorBotones
    {
        private SolicitudAmistad solicitudAmistadActual;

        public SolicitudAmistadUserControl()
        {
            InitializeComponent();
            SetFondoColorAleatorio();
            DataContextChanged += SolicitudAmistadUserControl_DataContextChanged;
        }

        private void SolicitudAmistadUserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is SolicitudAmistad solicitud && solicitud != null)
            {
                labelNombreAmigo.Content = solicitud.Remitente.Nombre;
                imageAmigo.Source = Imagen.ConvertirStreamABitmapImagen(solicitud.Remitente.FotoUsuario);
                solicitudAmistadActual = solicitud;
            }
        }

        private void SetFondoColorAleatorio()
        {
            this.Background = Utilidades.GetColorAleatorio();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            _ = AceptarSolicitud(solicitudAmistadActual);
        }

        private async Task<bool> AceptarSolicitud(SolicitudAmistad solicitud)
        {
            Window window = Window.GetWindow(this);
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, window);
            if (!conexionExitosa)
            {
                return false;
            }

            try
            {
                var resultado = Conexion.Amigos.AceptarSolicitudAmistad(solicitudAmistadActual.Remitente.IdUsuario, Singleton.Instance.IdUsuario);

                if (resultado)
                {
                    VentanasEmergentes.CrearVentanaEmergenteSolicitudAceptada(window, solicitudAmistadActual.Remitente.Nombre);
                    HabilitarBotones(false);
                }

                return resultado;
            }
            catch (Exception excepcion)
            {
                //TODO manejar excepcion
                VentanasEmergentes.CrearVentanaEmergenteCargarDatosAmigosFalla(this);
                return false;
            }
        }

        private void Rechazar_Click(object sender, RoutedEventArgs e)
        {
            _ = _ = RechazarSolicitud(solicitudAmistadActual);
        }

        private async Task<bool> RechazarSolicitud(SolicitudAmistad solicitud)
        {
            Window window = Window.GetWindow(this);
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, window);
            if (!conexionExitosa)
            {
                return false;
            }

            try
            {
                var resultado = Conexion.Amigos.RechazarSolicitudAmistad(solicitudAmistadActual.Remitente.IdUsuario, Singleton.Instance.IdUsuario);

                if (resultado)
                {
                    VentanasEmergentes.CrearVentanaEmergenteSolicitudRechazada(window, solicitudAmistadActual.Remitente.Nombre);
                    HabilitarBotones(false);
                }

                return resultado;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponentErrorException(excepcion);
                VentanasEmergentes.CrearVentanaEmergenteCargarDatosAmigosFalla(this);
                return false;
            }
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            buttonAceptar.IsEnabled = esHabilitado; 
            buttonRechazar.IsEnabled = esHabilitado;

            buttonAceptar.Opacity = esHabilitado ? 1.0 : 0.5;
            buttonRechazar.Opacity = esHabilitado ? 1.0 : 0.5;
        }
    }
}
