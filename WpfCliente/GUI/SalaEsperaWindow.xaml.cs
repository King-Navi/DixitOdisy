using System;
using System.Collections.Generic;
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
using UtilidadesLibreria;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para SalaEspera.xaml
    /// </summary>
    public partial class SalaEspera : Window, IActualizacionUI, IServicioSalaJugadorCallback
    {
        private static string idSala;
        public SalaEspera(string idSala)
        {
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            InitializeComponent();
            if (idSala == null)
            {
                GenerarSalaComoAnfitrion();
            }
            else
            {
                UnirseSala(idSala);
            }
        }
        private void UnirseSala(string idSala)
        {
            if (!Validacion.ExisteSala(idSala))
            {
                this.Close();
                return;
            }
            Singleton.Instance.ServicioSalaJugadorCliente =new ServidorDescribelo.ServicioSalaJugadorClient(new InstanceContext(this));
            ServidorDescribelo.IServicioSalaJugador contextoSala = Singleton.Instance.ServicioSalaJugadorCliente;
            contextoSala.AgregarJugadorSala(Singleton.Instance.nombreUsuario, idSala);
            labelCodigoSala.Content += idSala.ToString();

        }

        private void GenerarSalaComoAnfitrion()
        {

            ServidorDescribelo.IServicioSala nuevaSala = new ServidorDescribelo.ServicioSalaClient();
            idSala = nuevaSala.CrearSala(Singleton.Instance.nombreUsuario);
            UnirseSala(idSala);
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            return;
        }

        public void ObtenerJugadoresSalaCallback(string[] jugardoresEnSala)
        {
            throw new NotImplementedException();
        }

        public void EmpezarPartidaCallBack()
        {
            throw new NotImplementedException();
        }

        public void AsignarColorCallback(Dictionary<string, char> jugadoresColores)
        {
            throw new NotImplementedException();
        }

        private void CerrandoVentana(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
            if (Singleton.Instance.ServicioSalaJugadorCliente != null)
            {
                try
                {
                    Singleton.Instance.ServicioSalaJugadorCliente.Close();
                }
                catch (Exception ex)
                {
                    //TODO Manejar el error
                }
            }

        }
    }
}
