using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class AmigosWindow : Window, IHabilitadorBotones , IActualizacionUI
    {
        private int contadorClics = 0;
        private const int LIMITE_CLICS = 2;

        public AmigosWindow(MenuWindow menuWindow)
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
        }

        private void ClicButtonFlechaAtras(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void ClicButtonFlechaRecargar(object sender, MouseButtonEventArgs e)
        {
            if (contadorClics >= LIMITE_CLICS)
            {
                imagenFlechaRecargar.Visibility = Visibility.Collapsed;
                return;
            }
            contadorClics++;
            RecargarSolicitudes();
        }

        private void RecargarSolicitudes()
        {
            try
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
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(ex);
            }
        }

        private void ClicButtonNuevaSolicitud(object sender, MouseButtonEventArgs e)
        {
            TryEnviarSolicitud();
        }

        private async void TryEnviarSolicitud()
        {
            string gamertagSolicitud = VentanaModal.AbrirVentanaModalGamertag(this);

            var resultado = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!resultado)
            {
                return;
            }

            if (ValidacionesString.EsGamertagValido(gamertagSolicitud) && gamertagSolicitud != SingletonCliente.Instance.NombreUsuario)
            {
                try {
                    if (await EnviarSolicitud(gamertagSolicitud))
                    {
                        VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloSolicitudAmistad, Properties.Idioma.mensajeSolicitudAmistadExitosa, this);
                    }
                }
                catch (FaultException<SolicitudAmistadFalla> ex)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloSolicitudAmistad, ex.Detail.Mensaje, this);
                }
                catch (FaultException ex)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Idioma.mensajeErrorInesperado, ex.Message, this);
                }
                catch (Exception ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex, this);
                }
            }
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
                usuarioRemitente.Nombre = SingletonCliente.Instance.NombreUsuario;
                usuarioRemitente.FotoUsuario = Imagen.ConvertirBitmapImageAMemoryStream(SingletonCliente.Instance.FotoJugador);

                var resultado = Conexion.Amigos.EnviarSolicitudAmistad(usuarioRemitente, gamertagReceptor);
                return resultado;
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(ex);
                return false;
            }
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            imagenFlechaRecargar.IsEnabled = esHabilitado;
            imagenFlechaAtras.IsEnabled = esHabilitado;
            imagenAgregarAmigo.IsEnabled = esHabilitado;
            listaSolicitudesAmistadUserControl.IsEnabled = esHabilitado;
            imagenFlechaRecargar.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA: Utilidades.OPACIDAD_MINIMA;
            imagenFlechaAtras.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA : Utilidades.OPACIDAD_MINIMA;
            imagenAgregarAmigo.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA : Utilidades.OPACIDAD_MINIMA;
            
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            this.Title = Properties.Idioma.tituloAmigos;
        }

        private void CerrandoVentana(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento; 
        }
    }
}
