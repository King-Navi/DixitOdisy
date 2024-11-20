using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para SalaEspera.xaml
    /// </summary>
    public partial class SalaEsperaWindow : Window, IActualizacionUI, IServicioSalaJugadorCallback
    {
        public ObservableCollection<Usuario> JugadoresSala { get; set; } = new ObservableCollection<Usuario>();
        private bool soyAnfitrion = false;
        private bool visibleConfigurarPartida = false;
        private const int SEGUNDOS_PARA_UNIRSE = 6;
        private const int NUMERO_RONDAS_PORDEFECTO = 3;
        private ConfiguracionPartida ConfiguracionPartida { get; set; }

        public SalaEsperaWindow(string idSala)
        {
            InitializeComponent();
            EsconderOpciones();
            VerificarConexion();
            ConfiguracionPartidaPorDefecto(); 
            if (idSala == null)
            {
                soyAnfitrion = true;
                GenerarSalaComoAnfitrion();
                VerDiposicionAnfitrion();
            }
            else
            {
                Singleton.Instance.IdSala = idSala;
                Singleton.Instance.IdChat = idSala;
                UnirseSala(idSala);
            }
            DataContext = this;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }

        private void EsconderOpciones()
        {
            stackPanePartida.Visibility = Visibility.Hidden;
            gridConfiguracion.Visibility = Visibility.Hidden;
            stakePaneListaExpulsion.Visibility = Visibility.Collapsed;
        }

        private void ConfiguracionPartidaPorDefecto()
        {
            ConfiguracionPartida = new ConfiguracionPartida()
            {
                Condicion = CondicionVictoriaPartida.PorCantidadRondas,
                NumeroRondas = NUMERO_RONDAS_PORDEFECTO,
                Tematica = TematicaPartida.Mixta
            };
        }

        private void VerDiposicionAnfitrion()
        {
            stackPanePartida.Visibility = Visibility.Visible;
            stakePaneListaExpulsion.Visibility= Visibility.Visible;
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
                VentanasEmergentes.CrearVentanaEmergenteLobbyNoEncontrado(this);
                NoHayConexion();
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
            labelCodigo.Content += idSala;
            UnirseChat();

        }

        private void HabilitarBotones(bool estaHabilitado)
        {
            buttonConfigurarPartida.IsEnabled = estaHabilitado;
            buttonEmpezarPartida.IsEnabled = estaHabilitado;
            buttonGuardarCambios.IsEnabled = estaHabilitado;
            buttonInvitarAmigos.IsEnabled = estaHabilitado;
            chatUserControl.IsEnabled = estaHabilitado;
            stakePaneListaExpulsion.IsEnabled = estaHabilitado;
            stackPanePartida.IsEnabled = estaHabilitado;
            gridConfiguracion.IsEnabled = estaHabilitado;

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
                ManejadorExcepciones.ManejarFatalException(excepcion,this);
                NoHayConexion();
            }

        }

        private void CrearChat()
        {
            try
            {
                var manajadorServicio = new ServicioManejador<ServicioChatClient>();
                bool resultado = manajadorServicio.EjecutarServicio(proxy =>
                {
                    return proxy.CrearChat(Singleton.Instance.IdChat);
                });
            }
            catch (Exception excepcion)
            {
                NoHayConexion();
                ManejadorExcepciones.ManejarFatalException(excepcion, this);
            }

        }

        private bool CrearChat(string idPartidaParaChat)
        {
            try
            {
                var manajadorServicio = new ServicioManejador<ServicioChatClient>();
                return manajadorServicio.EjecutarServicio(proxy =>
                {
                    return proxy.CrearChat(idPartidaParaChat);
                });
            }
            catch (Exception excepcion)
            {
                NoHayConexion();
                ManejadorExcepciones.ManejarFatalException(excepcion, this);
            }
            return false;
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
            //TODO: faltan recursos (botones)
            labelCodigoSala.Content = Idioma.labelCodigoSala;
            labelUsuariosLobby.Content = Idioma.labelUsuariosLobby;
            labelInvitaAmigos.Content = Idioma.labelInvitaAmigos;
            buttonInvitarAmigos.Content = Idioma.buttonInvitaAmigos;
            buttonConfigurarPartida.Content = Idioma.buttonConfigurarPartida;
            buttonEmpezarPartida.Content = Idioma.buttonEmpezarPartida;
            //groupBoxCondicionVicotoria.Header
            //grouoBoxTematica.Header
            //radioButtonAnimales
            //radioButtonMitologia
            //radioButtonMixta
            //radioButtonPaises
            //radioButtonFinCartas
            //radioButtonFinRondas
            //buttonConfigurarPartida
            //buttonEmpezarPartida
            //labelExpulsarUsuario
            //buttonGuardarCambios
            //labelNumeroRondas
        }

        public void EmpezarPartidaCallBack(string idPartida)
        {
            if (!Validacion.ExistePartida(idPartida))
            {
                VentanasEmergentes.CrearVentanaEmergenteLobbyNoEncontrado(this);
                NoHayConexion();
                return;
            }
            Singleton.Instance.IdPartida = idPartida;
            PartidaWindow partida = new PartidaWindow(idPartida);
            partida.Show();
            this.Close();
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
            if (!jugardoreNuevoEnSala.Nombre.Equals(Singleton.Instance.NombreUsuario , StringComparison.OrdinalIgnoreCase ))
            {
                JugadoresSala.Add(jugardoreNuevoEnSala);
            }
            List<Amigo> amigos = new List<Amigo>();
            usuariosSalaUserControl.ObtenerUsuarioSala(jugardoreNuevoEnSala, amigos);
        }

        public void EliminarJugadorSalaCallback(Usuario jugardoreRetiradoDeSala)
        {
            var jugadorARemover= JugadoresSala.FirstOrDefault(jugador  => jugador.Nombre == jugardoreRetiradoDeSala.Nombre);
            if (jugadorARemover != null)
            {
                JugadoresSala.Remove(jugadorARemover);
            }
            usuariosSalaUserControl.EliminarUsuarioSala(jugardoreRetiradoDeSala);
            if (jugardoreRetiradoDeSala.Nombre.Equals(Singleton.Instance.NombreUsuario, StringComparison.OrdinalIgnoreCase))
            {
                this.Close();
            }
        }

        private void ClicButtonEmpezarPartida(object sender, RoutedEventArgs e)
        {
            EvaluarCantidaRondas();
            EvaluarCondicionVictoria();
            EvaluarTematicaSelecionada();
            var manejador = new ServicioManejador<ServicioPartidaClient>();
            Singleton.Instance.IdPartida = manejador.EjecutarServicio(proxy =>
            {
                return proxy.CrearPartida(Singleton.Instance.NombreUsuario, this.ConfiguracionPartida);
            }
            );
            if (Singleton.Instance.IdPartida != null)
            {
                try
                {
                    CrearChat(Singleton.Instance.IdPartida);
                    Conexion.SalaJugador.ComenzarPartidaAnfrition(Singleton.Instance.NombreUsuario, Singleton.Instance.IdSala, Singleton.Instance.IdPartida);
                    Task.Delay(TimeSpan.FromSeconds(SEGUNDOS_PARA_UNIRSE));
                    PartidaWindow partida = new PartidaWindow(Singleton.Instance.IdPartida);
                    partida.Show();
                    this.Close();
                }
                catch (Exception)
                {
                }
                
            }
        }

        private void LabelCodigoSala_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (labelCodigo.Content != null)
            {
                Clipboard.SetText(labelCodigo.Content.ToString());

                VentanasEmergentes.CrearVentanaEmergenteCodigoCopiado(this);
            }
        }

        private async void buttonInvitarAmigos_Click(object sender, RoutedEventArgs e)
        {
            string gamertagInvitado = AbrirVentanaModalGamertag();
            if (gamertagInvitado != null && gamertagInvitado != Singleton.Instance.NombreUsuario) {
                if (await EnviarInvitacion(gamertagInvitado))
                {
                    VentanasEmergentes.CrearVentanaEmergenteInvitacionEnviada(this);
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergenteInvitacionNoEnviada(this);
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergenteInvitacionNoEnviada(this);
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

            }
            bool? resultado = ventanaModal.ShowDialog();

            if (resultado == true)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }

            return valorObtenido;
        }

        private async Task<bool> EnviarInvitacion(string gamertagReceptor)
        {
            bool resultado = false;
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return false;
            }
            try
            {
                resultado = Conexion.InvitacionPartida.EnviarInvitacion(Singleton.Instance.NombreUsuario, Singleton.Instance.IdSala, gamertagReceptor);
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearVentanaEmergenteInvitacionNoEnviada(this);
            }
            return resultado;
            
        }

        private void ClicButtonConfigurarPartida(object sender, RoutedEventArgs e)
        {
            if (visibleConfigurarPartida)
            {
                gridConfiguracion.Visibility = Visibility.Hidden;
                visibleConfigurarPartida = false;
            }
            else
            {
                gridConfiguracion.Visibility = Visibility.Visible;
                visibleConfigurarPartida = true;
            }
        }

        private void EvaluarTematicaSelecionada()
        {

            if (radioButtonMixta.IsChecked == true)
            {
                ConfiguracionPartida.Tematica = TematicaPartida.Mixta;
            }
            else if (radioButtonAnimales.IsChecked == true)
            {
                ConfiguracionPartida.Tematica = TematicaPartida.Animales;
            }
            else if (radioButtonPaises.IsChecked == true)
            {
                ConfiguracionPartida.Tematica = TematicaPartida.Paises;
            }
            else if (radioButtonMitologia.IsChecked == true)
            {
                ConfiguracionPartida.Tematica = TematicaPartida.Mitologia;
            }
        }

        private void EvaluarCondicionVictoria()
        {

            if (radioButtonFinCartas.IsChecked == true)
            {
                ConfiguracionPartida.Condicion = CondicionVictoriaPartida.PorCartasAgotadas;
            }
            else if (radioButtonFinRondas.IsChecked == true)
            {
                ConfiguracionPartida.Condicion = CondicionVictoriaPartida.PorCantidadRondas;

            }
            
        }

        private void EvaluarCantidaRondas()
        {
            if (comboBoxNumeroRondas.SelectedItem is ComboBoxItem seleccion)
            {
                if (int.TryParse(seleccion.Content.ToString(), out int numeroSeleccionado))
                {
                    ConfiguracionPartida.NumeroRondas = numeroSeleccionado;
                }
                else
                {
                    ConfiguracionPartida.NumeroRondas = NUMERO_RONDAS_PORDEFECTO;
                }
            }
        }

        private void ClicButtonEliminarUsuario(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton && boton.DataContext is Usuario usuario)
            {
                String id = usuario.Nombre;
                MessageBox.Show($"IdUsuario: {id}, Nombre: {usuario.Nombre}", "Información del Usuario");
                try
                {
                    Conexion.SalaJugador.ExpulsarJugadorSala(
                        Singleton.Instance.NombreUsuario, 
                        usuario.Nombre, 
                        Singleton.Instance.IdSala);
                }
                catch (Exception) 
                {
                }
            }
        }

        public void DelegacionRolCallback(bool esAnfitrion)
        {
            soyAnfitrion = esAnfitrion;
            VerDiposicionAnfitrion();
        }
    }
}
