using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para SalaEspera.xaml
    /// </summary>
    public partial class SalaEsperaWindow : Window, IActualizacionUI, IServicioSalaJugadorCallback
    {
        public ObservableCollection<Usuario> jugadoresSala = new ObservableCollection<Usuario>();
        public SalaEsperaWindow(string idSala)
        {
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            InitializeComponent();
            VerificarConexion();
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
            DataContext = this;
        }

        private async void VerificarConexion()
        {
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                this.Close();
                return;
            }
        }

        private async void UnirseSala(string idSala)
        {
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }
            if (!Validacion.ExisteSala(idSala))
            {
                //No existe la sala ¿¿??
                this.Close();
                return;
            }
            var resultadoTask = Conexion.AbrirConexionSalaJugadorCallbackAsync(this);
            bool resultado = resultadoTask.Result;

            if (!resultado)
            {
                NoHayConexion();
                return;
            }
            Conexion.SalaJugador.AgregarJugadorSala(Singleton.Instance.NombreUsuario, idSala);
            labelCodigoSala.Content += idSala;
            UnirseChat();

        }

        private void HabilitarBotones(bool v)
        {
            //TODO
        }

        private void UnirseChat()
        {
            Conexion.AbrirConexionChatMotorCallbackAsync(chatUserControl);
            Conexion.ChatMotor.AgregarUsuarioChat(Singleton.Instance.IdChat, Singleton.Instance.NombreUsuario);
            
        }

        private void GenerarSalaComoAnfitrion()
        {
            try
            {
                var manejadorServicio = new ServicioManejador<ServicioSalaClient>();
                Singleton.Instance.IdSala = manejadorServicio.EjecutarServicio(proxy =>
                 {
                     return proxy.CrearSala(Singleton.Instance.NombreUsuario);
                 });
                Singleton.Instance.IdChat = Singleton.Instance.IdSala;

                CrearChat();
                UnirseSala(Singleton.Instance.IdSala);
            }
            catch (Exception excepcion)
            {
                //TODO: Manejo de otras excepciones
                NoHayConexion();
            }

        }

        private void CrearChat()
        {
            //TODO: Agregar caso en el que no hay conexion
            try
            {
                var manajadorServicio = new ServicioManejador<ServicioChatClient>();
                bool resultado = manajadorServicio.EjecutarServicio(proxy =>
                {
                    return proxy.CrearChat(Singleton.Instance.IdChat);
                });
            }
            catch (Exception)
            {
                //TODO: Manejo de otras excepciones
                NoHayConexion();
            }

        }
        private void NoHayConexion()
        {
            this.Close();
        }
        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            //TODO:Actualizar toda la inferfaz, Pedir a Unnay los .resx
        }

        

        public void EmpezarPartidaCallBack(string idPartida)
        {
            PartidaWindow partida = new PartidaWindow(idPartida);
            partida.Show();

            this.Close();
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
                Conexion.CerrarChatMotor();
                Conexion.CerrarSalaJugador();
            }
            catch (Exception excepcion)
            {
                //TODO Manejar excepcion
            }

        }

        public void ObtenerJugadorSalaCallback(Usuario jugardoreNuevoEnSala)
        {
            List<Amigo> amigos = new List<Amigo>();
            usuariosSalaUserControl.ObtenerUsuarioSala(jugardoreNuevoEnSala, amigos);
        }

        public void EliminarJugadorSalaCallback(Usuario jugardoreRetiradoDeSala)
        {
            usuariosSalaUserControl.EliminarUsuarioSala(jugardoreRetiradoDeSala);
            if (jugardoreRetiradoDeSala.Nombre.Equals(Singleton.Instance.NombreUsuario, StringComparison.OrdinalIgnoreCase))
            {
                this.Close();
            }
        }

        private void ClicButtonEmpezarPartida(object sender, RoutedEventArgs e)
        {
            ConfiguracionPartida prueba = new ConfiguracionPartida()
            {
                Condicion = CondicionVictoriaPartida.PorCantidadRondas,
                NumeroRondas = 2,
                Tematica = TematicaPartida.Mixta
            };
            //TODO: Evaluar que tengar un configuracion partida valido
            //TODO: Evaluar que seas el anfitrion para poder ver el boton
            var manejador = new ServicioManejador<ServicioPartidaClient>();
            Singleton.Instance.IdPartida = manejador.EjecutarServicio(proxy =>
            {
                return proxy.CrearPartida(Singleton.Instance.NombreUsuario, prueba);
            }
            );
            if (Singleton.Instance.IdPartida != null)
            {
                Conexion.SalaJugador.ComenzarPartidaAnfrition(
                    Singleton.Instance.NombreUsuario, Singleton.Instance.IdSala , Singleton.Instance.IdPartida);
            }
        }
    }
}
