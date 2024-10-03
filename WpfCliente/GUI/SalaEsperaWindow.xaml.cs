using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using UtilidadesLibreria;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para SalaEspera.xaml
    /// </summary>
    public partial class SalaEspera : Window, IActualizacionUI, IServicioSalaJugadorCallback
    {
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
                Singleton.Instance.IdSala = idSala;
                Singleton.Instance.IdChat = idSala;
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
            Singleton.Instance.ServicioSalaJugadorCliente = new ServidorDescribelo.ServicioSalaJugadorClient(new InstanceContext(this));
            ServidorDescribelo.IServicioSalaJugador contextoSala = Singleton.Instance.ServicioSalaJugadorCliente;
            contextoSala.AgregarJugadorSala(Singleton.Instance.NombreUsuario, idSala);
            labelCodigoSala.Content += idSala;
            UnirseChat();

        }

        private void UnirseChat()
        {
            Singleton.Instance.ServicioChatCliente = new ServicioChatMotorClient(new InstanceContext(chatUserControl));
            Singleton.Instance.ServicioChatCliente.AgregarUsuarioChat(Singleton.Instance.IdChat, Singleton.Instance.NombreUsuario);
        }

        private void GenerarSalaComoAnfitrion()
        {
            //TODO: evaluar si es mejor dispose()
            //Si no hay conexion saltara una excepcion
            ServidorDescribelo.IServicioSala nuevaSala = new ServidorDescribelo.ServicioSalaClient();
            try
            {
                Singleton.Instance.IdSala = nuevaSala.CrearSala(Singleton.Instance.NombreUsuario);
                Singleton.Instance.IdChat = Singleton.Instance.IdSala;
                CrearChat();
                UnirseSala(Singleton.Instance.IdSala);
            }catch (Exception exceçion)
            {
                //TODO: Manejo de otras excepciones

            }
            finally
            {
                if (nuevaSala != null)
                {
                    ((ICommunicationObject)nuevaSala).Close();
                }
            }
        }

        private void CrearChat()
        {
            //TODO: Agregar caso en el que no hay conexion
            ServidorDescribelo.IServicioChat contexto = new ServicioChatClient();
            contexto.CrearChat(Singleton.Instance.IdChat);
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            //TODO:Actualizar toda la inferfaz, Pedir a Unnay los .resx
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
            try
            {
                if (Singleton.Instance.ServicioChatCliente != null)
                {
                    Singleton.Instance.ServicioChatCliente.Close();
                    Singleton.Instance.ServicioChatCliente = null;
                }
                if (Singleton.Instance.ServicioSalaJugadorCliente != null)
                {
                    Singleton.Instance.ServicioSalaJugadorCliente.Close();
                    Singleton.Instance.ServicioSalaJugadorCliente = null;

                }
            }
            catch (Exception excepcion)
            {

                throw;
            }

        }
    }
}
