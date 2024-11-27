using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfCliente.Contexto;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class SalaEsperaWindow : Window, IActualizacionUI, IHabilitadorBotones
    {
        public ObservableCollection<Usuario> JugadoresSala { get; set; } = new ObservableCollection<Usuario>();
        private bool visibleConfigurarPartida = false;
        private const int NUMERO_RONDAS_PORDEFECTO = 3;
        private const int TIEMPO_CLIC_EXPULSION_SEGUNDOS = 5;
        private ConfiguracionPartida ConfiguracionPartida { get; set; }

        public SalaEsperaWindow(string idSala)
        {
            try
            {
                InitializeComponent();
                EsconderOpciones();
                VerificarConexion();
                ConfiguracionPartidaPorDefecto();
                if (idSala == null)
                {
                    GenerarSalaComoAnfitrion();
                    VerDiposicionAnfitrion();
                }
                else
                {
                    SingletonCliente.Instance.IdSala = idSala;
                    SingletonCliente.Instance.IdChat = idSala;
                    UnirseSala(idSala);
                }
                DataContext = this;
                CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
                ActualizarUI();
                JugadoresSala = SingletonSalaJugador.Instancia.JugadoresSala;
                SingletonSalaJugador.Instancia.DelegacionRolAnfitrion += DelegacionRol;
                SingletonSalaJugador.Instancia.EmepzarPartida += EmpezarPartidaCallback;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.SalaEspera);
            }
        }

        private void EsconderOpciones()
        {
            try
            {
                stackPanePartida.Visibility = Visibility.Hidden;
                gridConfiguracion.Visibility = Visibility.Hidden;
                stakePaneListaExpulsion.Visibility = Visibility.Collapsed;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
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
            try
            {
                stackPanePartida.Visibility = Visibility.Visible;
                stakePaneListaExpulsion.Visibility = Visibility.Visible;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
        }

        private async void VerificarConexion()
        {
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.SalaEspera);
                return;
            }
        }

        private async void UnirseSala(string idSala)
        {
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }
            if (!ValidacionExistenciaJuego.ExisteSala(idSala))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLobbyNoEncontrado, Properties.Idioma.mensajeLobbyNoEncontrado, this);
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.SalaEspera);
                return;
            }
            var resultado = SingletonSalaJugador.Instancia.AbrirConexion();
            if (!resultado)
            {
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.SalaEspera);
                return;
            }
            SingletonSalaJugador.Instancia.Sala.AgregarJugadorSala(SingletonCliente.Instance.NombreUsuario, idSala);
            labelCodigo.Content += idSala;
            UnirseChatAsync();
        }



        public void HabilitarBotones(bool esHabilitado)
        {
            try
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
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }

        }

        private async void UnirseChatAsync()
        {
            await Conexion.AbrirConexionChatMotorCallbackAsync(chatUserControl);
            var resultado = Conexion.ChatMotor.AgregarUsuarioChat(SingletonCliente.Instance.IdChat, SingletonCliente.Instance.NombreUsuario);
            if (!resultado)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorInesperado, Properties.Idioma.mensajeErrorInesperado, this);
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.SalaEspera);
            }
        }

        private void GenerarSalaComoAnfitrion()
        {
            try
            {
                var manejadorServicio = new ServicioManejador<ServicioSalaClient>();
                SingletonCliente.Instance.IdSala = manejadorServicio.EjecutarServicio(proxy =>
                {
                    return proxy.CrearSala(SingletonCliente.Instance.NombreUsuario);
                });
                SingletonCliente.Instance.IdChat = SingletonCliente.Instance.IdSala;

                CrearChat();
                UnirseSala(SingletonCliente.Instance.IdSala);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, this);
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.SalaEspera);
            }

        }

        private void CrearChat()
        {
            try
            {
                var manajadorServicio = new ServicioManejador<ServicioChatClient>();
                bool resultado = manajadorServicio.EjecutarServicio(proxy =>
                {
                    return proxy.CrearChat(SingletonCliente.Instance.IdChat);
                });
            }
            catch (Exception excepcion)
            {
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.SalaEspera);
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, this);
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
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.SalaEspera);
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, this);
            }
            return false;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            try
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
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
        }

        public void EmpezarPartidaCallback(string idPartida)
        {
            if (!ValidacionExistenciaJuego.ExistePartida(idPartida))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLobbyNoEncontrado, Properties.Idioma.mensajeLobbyNoEncontrado, this);
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.SalaEspera);
                return;
            }
            SingletonCliente.Instance.IdPartida = idPartida;
            OcultarVentanaHastaCierre();
        }

        private void CerrandoVentana(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
        }

        private async void ClicButtonEmpezarPartidaAsync(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.SalaEspera);
                return;
            }
            if (!ValidarExistenciaSala())
            {
                MostrarVentanaSalaNoEncontrada();
                return;
            }
            PrepararPartida();
            var idPartida = CrearPartida();
            if (idPartida != null)
            {
                SingletonCliente.Instance.IdPartida = idPartida;

                try
                {
                    var resultado = IniciarPartida(idPartida);
                    if (resultado)
                    {
                        OcultarVentanaHastaCierre();
                    }
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                }

                buttonEmpezarPartida.IsEnabled = true;
            }
            this.IsEnabled = true;
        }

        private bool ValidarExistenciaSala()
        {
            return ValidacionExistenciaJuego.ExisteSala(SingletonCliente.Instance.IdSala);
        }
        private void MostrarVentanaSalaNoEncontrada()
        {
            VentanasEmergentes.CrearVentanaEmergenteConCierre(
                Properties.Idioma.tituloLobbyNoEncontrado,
                Idioma.mensajeLobbyNoDisponible,
                this
            );
        }

        private void PrepararPartida()
        {
            try
            {
                buttonEmpezarPartida.IsEnabled = false;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            EvaluarCantidaRondas();
            EvaluarCondicionVictoria();
            EvaluarTematicaSelecionada();
        }

        private string CrearPartida()
        {
            var manejador = new ServicioManejador<ServicioPartidaClient>();
            return manejador.EjecutarServicio(proxy =>
                proxy.CrearPartida(SingletonCliente.Instance.NombreUsuario, this.ConfiguracionPartida)
            );
        }

        private bool IniciarPartida(string idPartida)
        {
            CrearChat(idPartida);
            return SingletonSalaJugador.Instancia.Sala.ComenzarPartidaAnfrition(
                SingletonCliente.Instance.NombreUsuario,
                SingletonCliente.Instance.IdSala,
                idPartida
            );
        }
        private void OcultarVentanaHastaCierre()
        {
            if (SingletonCliente.Instance.IdPartida == null)
            {
                SingletonGestorVentana.Instancia.CerrarVentana(Ventana.Partida);
                return;
            }
            PartidaWindow partida = new PartidaWindow(SingletonCliente.Instance.IdPartida);
            if (partida != null)
            {
                SingletonGestorVentana.Instancia.AbrirNuevaVentana(Ventana.Partida, partida);
            }
        }

        private void ClicButtonCopiar(object sender, MouseButtonEventArgs e)
        {
            if (labelCodigo.Content != null)
            {
                Clipboard.SetText(labelCodigo.Content.ToString());

                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCodigoCopiado,
                    Properties.Idioma.mensajeCodigoCopiado,
                    this);
            }
        }

        private async void ClicButtonInvitarAmigos(object sender, RoutedEventArgs e)
        {
            string gamertagInvitado = AbrirVentanaModalGamertag();
            if (gamertagInvitado != null && gamertagInvitado != SingletonCliente.Instance.NombreUsuario)
            {
                if (await EnviarInvitacion(gamertagInvitado))
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloInvitacionPartida,
                        Properties.Idioma.mensajeInvitacionExitosa,
                        this);
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloInvitacionPartida,
                    Properties.Idioma.mensajeInvitacionFallida,
                    this);
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
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(ex);
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
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return false;
            }
            try
            {
                resultado = SingletonInvitacionPartida.Instancia.InvitacionPartida.EnviarInvitacion(SingletonCliente.Instance.NombreUsuario,
                    SingletonCliente.Instance.IdSala,
                    gamertagReceptor);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloInvitacionPartida,
                    Properties.Idioma.mensajeInvitacionFallida,
                    this);
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

            foreach (var entrada in tematicas)
            {
                if (entrada.Key.IsChecked == true)
                {
                    ConfiguracionPartida.Tematica = entrada.Value;
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

        private async void ClicButtonEliminarUsuario(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                Application.Current.Shutdown();
                return;
            }
            if (sender is Button boton && boton.DataContext is Usuario usuario)
            {
                try
                {
                    SingletonSalaJugador.Instancia.Sala.ExpulsarJugadorSala(
                        SingletonCliente.Instance.NombreUsuario,
                        usuario.Nombre,
                        SingletonCliente.Instance.IdSala);
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(TIEMPO_CLIC_EXPULSION_SEGUNDOS));
            this.IsEnabled = true;
        }

        private void ClicImagenFlechaAtras(object sender, MouseButtonEventArgs e)
        {
            SingletonGestorVentana.Instancia.CerrarVentana(Ventana.SalaEspera);
        }
        public void DelegacionRol(bool esAnfitrion)
        {
            VerDiposicionAnfitrion();
        }
    }
}

