using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public partial class SalaEsperaWindow : Window, IActualizacionUI, IServicioSalaJugadorCallback, IHabilitadorBotones
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
            stakePaneListaExpulsion.Visibility = Visibility.Visible;
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
            if (!ValidacionExistenciaJuego.ExisteSala(idSala))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLobbyNoEncontrado, Properties.Idioma.mensajeLobbyNoEncontrado, this);
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



        public void HabilitarBotones(bool esHabilitado)
        {
            buttonConfigurarPartida.IsEnabled = esHabilitado;
            buttonEmpezarPartida.IsEnabled = esHabilitado;
            buttonGuardarCambios.IsEnabled = esHabilitado;
            buttonInvitarAmigos.IsEnabled = esHabilitado;
            chatUserControl.IsEnabled = esHabilitado;
            stakePaneListaExpulsion.IsEnabled = esHabilitado;
            stackPanePartida.IsEnabled = esHabilitado;
            gridConfiguracion.IsEnabled = esHabilitado;

        }

        private void UnirseChat()
        {
            Conexion.AbrirConexionChatMotorCallbackAsync(chatUserControl);
            var resultado = Conexion.ChatMotor.AgregarUsuarioChat(Singleton.Instance.IdChat, Singleton.Instance.NombreUsuario);
            if (!resultado)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorInesperado, Properties.Idioma.mensajeErrorInesperado, this);
                NoHayConexion();
            }
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
                ManejadorExcepciones.ManejarFatalException(excepcion, this);
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
            labelCodigoSala.Content = Idioma.labelCodigoSala;
            labelUsuariosLobby.Content = Idioma.labelUsuariosLobby;
            labelInvitaAmigos.Content = Idioma.labelInvitaAmigos;
            buttonInvitarAmigos.Content = Idioma.buttonInvitaAmigos;
            buttonConfigurarPartida.Content = Idioma.buttonConfigurarPartida;
            buttonEmpezarPartida.Content = Idioma.buttonEmpezarPartida;
            groupBoxCondicionVictoria.Header = Idioma.labelCondicionDeVictoria;
            grouoBoxTematica.Header = Idioma.labelTematica;
            radioButtonAnimales.Content = Idioma.buttonAnimales;
            radioButtonMitologia.Content = Idioma.buttonMitologia;
            radioButtonMixta.Content = Idioma.buttonMixta;
            radioButtonPaises.Content = Idioma.buttonPaises;
            radioButtonFinCartas.Content = Idioma.buttonFinCartas;
            radioButtonFinRondas.Content = Idioma.buttonFinRondas;
            labelExpulsarUsuario.Content = Idioma.buttonExpulsar;
            buttonGuardarCambios.Content = Idioma.buttonGuardarCambios;
            labelNumeroRondas.Content = Idioma.labelNumeroRondas;
        }

        public void EmpezarPartidaCallBack(string idPartida)
        {
            if (!ValidacionExistenciaJuego.ExistePartida(idPartida))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLobbyNoEncontrado, Properties.Idioma.mensajeLobbyNoEncontrado, this);
                NoHayConexion();
                return;
            }
            Singleton.Instance.IdPartida = idPartida;
            OcultarVetanaHastaCierre();
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
            if (!jugardoreNuevoEnSala.Nombre.Equals(Singleton.Instance.NombreUsuario, StringComparison.OrdinalIgnoreCase))
            {
                JugadoresSala.Add(jugardoreNuevoEnSala);
            }
            List<Amigo> amigos = new List<Amigo>();
            usuariosSalaUserControl.ObtenerUsuarioSala(jugardoreNuevoEnSala, amigos);
        }

        public void EliminarJugadorSalaCallback(Usuario jugardoreRetiradoDeSala)
        {
            var jugadorARemover = JugadoresSala.FirstOrDefault(jugador => jugador.Nombre == jugardoreRetiradoDeSala.Nombre);
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
            buttonEmpezarPartida.IsEnabled = false;
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
                    OcultarVetanaHastaCierre();
                }
                catch (Exception)
                {
                }
                buttonEmpezarPartida.IsEnabled = true;
            }
        }

        private void OcultarVetanaHastaCierre()
        {
            if (Singleton.Instance.IdPartida == null)
            {
                this.Close();
                return;
            }
            PartidaWindow partida = new PartidaWindow(Singleton.Instance.IdPartida);
            partida.Show();
            this.Hide();
            partida.Closed += (s, args) => {
                if (!Conexion.CerrarConexionesPartida())
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, Properties.Idioma.mensajeErrorServidor, this);
                    this.Close();
                }
                if (!Conexion.CerrarChatMotor())
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorServidor, Properties.Idioma.mensajeErrorServidor, this);
                    this.Close();
                }
                this.Show();
            };
        }

        private void ClicButtonCopiar(object sender, MouseButtonEventArgs e)
        {
            if (labelCodigo.Content != null)
            {
                Clipboard.SetText(labelCodigo.Content.ToString());

                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCodigoCopiado, Properties.Idioma.mensajeCodigoCopiado, this);
            }
        }

        private async void ClicButtonInvitarAmigos(object sender, RoutedEventArgs e)
        {
            string gamertagInvitado = AbrirVentanaModalGamertag();
            if (gamertagInvitado != null && gamertagInvitado != Singleton.Instance.NombreUsuario)
            {
                if (await EnviarInvitacion(gamertagInvitado))
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloInvitacionPartida, Properties.Idioma.mensajeInvitacionExitosa, this);
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloInvitacionPartida, Properties.Idioma.mensajeInvitacionFallida, this);
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
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponentErrorException(ex);
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloInvitacionPartida, Properties.Idioma.mensajeInvitacionFallida, this);
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

            var tematicas = new Dictionary<RadioButton, TematicaPartida>
            {
                { radioButtonMixta, TematicaPartida.Mixta },
                { radioButtonAnimales, TematicaPartida.Animales },
                { radioButtonPaises, TematicaPartida.Paises },
                { radioButtonMitologia, TematicaPartida.Mitologia }
            };

            foreach (var entry in tematicas)
            {
                if (entry.Key.IsChecked == true)
                {
                    ConfiguracionPartida.Tematica = entry.Value;
                    break;
                }
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

