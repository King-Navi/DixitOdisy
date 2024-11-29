using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.ImplementacionesCallbacks;
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
                    IdUsuario = SingletonCliente.Instance.IdUsuario,
                    Nombre = SingletonCliente.Instance.NombreUsuario
                };
                bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
                if (!conexionExitosa)
                {
                    return false;
                }

                try
                {
                    var listaSolicitudes = SingletonCanal.Instancia.Amigos.ObtenerSolicitudesAmistad(usuarioActual);
                    if (listaSolicitudes == null || listaSolicitudes.Count() == 0)
                    {
                        textBlockNoHaySolicitudes.Visibility = Visibility.Visible;
                        return false;
                    }
                    Solicitudes.Clear();
                    foreach (var solicitud in listaSolicitudes)
                    {
                        Solicitudes.Add(solicitud);
                    }

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
            textBlockNoHaySolicitudes.Text = Idioma.labelNoHaySolicitudes;
        }
    }
}
