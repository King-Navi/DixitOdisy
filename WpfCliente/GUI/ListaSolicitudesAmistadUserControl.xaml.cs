using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para ListaSolicitudesAmistadUserControl.xaml
    /// </summary>
    public partial class ListaSolicitudesAmistadUserControl : UserControl, IActualizacionUI
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
                Usuario usuarioActual = new Usuario();
                usuarioActual.IdUsuario = Singleton.Instance.IdUsuario;
                usuarioActual.Nombre = Singleton.Instance.NombreUsuario;
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
                        //MessageBox.Show("No se encontraron solicitudes de amistad.");
                        return false;
                    }
                    foreach (var solicitud in listaSolicitudes)
                    {
                        Solicitudes.Add(new SolicitudAmistad
                        {
                            Remitente = solicitud
                        });
                    }
                    return true;
                }
                catch (Exception e)
                {
                    //TODO MANEJAR EL ERROR
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar solicitudes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void HabilitarBotones(bool v)
        {
            //TODO
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelSolicitudes.Content = Idioma.labelSolicitudesAmistad;
        }
    }
}
