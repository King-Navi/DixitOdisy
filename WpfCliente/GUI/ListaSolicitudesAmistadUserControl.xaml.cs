using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class ListaSolicitudesAmistadUserControl : UserControl, IActualizacionUI, IHabilitadorBotones
    {
        public ObservableCollection<SolicitudAmistad> Solicitudes { get; set; } = new ObservableCollection<SolicitudAmistad>();
        public ListaSolicitudesAmistadUserControl()
        {
            InitializeComponent();
            DataContext = this;
            _ = CargarSolicitudes();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }

        private async Task<bool> CargarSolicitudes()
        {
            try
            {
                Usuario usuarioActual = new Usuario
                {
                    IdUsuario = Singleton.Instance.IdUsuario,
                    Nombre = Singleton.Instance.NombreUsuario
                };

                Window window = Window.GetWindow(this);

                bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, window);
                if (!conexionExitosa)
                {
                    return false;
                }

                try
                {
                    var listaSolicitudes = Conexion.Amigos.ObtenerSolicitudesAmistad(usuarioActual);
                    if (listaSolicitudes == null || listaSolicitudes.Count() == 0)
                    {
                        labelNoHaySolicitudes.Visibility = Visibility.Visible;
                        return false;
                    }
                    Solicitudes.Clear();
                    Solicitudes = new ObservableCollection<SolicitudAmistad>(listaSolicitudes);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorCargarSolicitudesAmistad, Properties.Idioma.mensajeErrorAlCargarLasSolicitudesAmistad, this);
                return false;
            }
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            itemsControlAmigos.IsEnabled = esHabilitado;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelSolicitudes.Content = Idioma.labelSolicitudesAmistad;
            labelNoHaySolicitudes.Content = Idioma.labelNoHaySolicitudes;
        }
    }
}
