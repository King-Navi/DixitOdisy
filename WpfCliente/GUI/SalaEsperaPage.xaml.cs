using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
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
    public partial class SalaEsperaPage : Page, IActualizacionUI, IHabilitadorBotones
    {
        public ObservableCollection<Usuario> JugadoresSala { get; set; } = new ObservableCollection<Usuario>();
        private bool visibleConfigurarPartida = false;
        private const int NUMERO_RONDAS_PORDEFECTO = 3;
        private const int TIEMPO_CLIC_EXPULSION_SEGUNDOS = 5;
        private const int MINIMO_JUGADORES = 3;
        private const int MAXIMO_JUGADORES = 4;
        private ConfiguracionPartida ConfiguracionPartida { get; set; }

        public SalaEsperaPage(string idSala)
        {
            KeepAlive = false;
            try
            {
                CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
                InitializeComponent();
                SingletonSalaJugador.Instancia.DelegacionRolAnfitrion += DelegacionRol;
                SingletonSalaJugador.Instancia.EmepzarPartida += EmpezarPartidaCallback;
                JugadoresSala = SingletonSalaJugador.Instancia.JugadoresSala;
                EsconderOpciones();
                _ = VerificarConexionAsync();
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
                    _ = UnirseSalaAsync(idSala);
                }
                DataContext = this;
                ActualizarUI();
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                SingletonGestorVentana.Instancia.Regresar();
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
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
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
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }

        private async Task VerificarConexionAsync()
        {
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                SingletonGestorVentana.Instancia.Regresar();
                return;
            }
        }

        private async Task UnirseSalaAsync(string idSala)
        {
            try
            {
                bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
                if (!conexionExitosa)
                {
                    return;
                }
                if (!ValidacionExistenciaJuego.ExisteSala(idSala))
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLobbyNoEncontrado, Properties.Idioma.mensajeLobbyNoEncontrado, Window.GetWindow(this));
                    SingletonGestorVentana.Instancia.Regresar();
                    return;
                }
                var resultado = SingletonSalaJugador.Instancia.AbrirConexionSala();
                if (!resultado)
                {
                    SingletonGestorVentana.Instancia.Regresar();
                    return;
                }
                await SingletonSalaJugador.Instancia.Sala.AgregarJugadorSalaAsync(SingletonCliente.Instance.NombreUsuario, idSala);
                labelCodigo.Content += idSala;
                UnirseChat();
            }
            catch (FaultException<SalaFalla>)
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloSalaLlena,
                    Idioma.mensajeSalaLlena,
                    Window.GetWindow(this));
                SingletonGestorVentana.Instancia.Regresar();
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }



        public void HabilitarBotones(bool esHabilitado)
        {
            try
            {
                buttonConfigurarPartida.IsEnabled = esHabilitado;
                buttonEmpezarPartida.IsEnabled = esHabilitado;
                buttonInvitarAmigos.IsEnabled = esHabilitado;
                chatUserControl.IsEnabled = esHabilitado;
                stakePaneListaExpulsion.IsEnabled = esHabilitado;
                stackPanePartida.IsEnabled = esHabilitado;
                gridConfiguracion.IsEnabled = esHabilitado;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }

        }

        private void UnirseChat()
        {
            SingletonChat.Instancia.AbrirConexionChat();
            var resultado = SingletonChat.Instancia.ChatMotor.AgregarUsuarioChat(SingletonCliente.Instance.IdChat, SingletonCliente.Instance.NombreUsuario);
            if (!resultado)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorInesperado, Properties.Idioma.mensajeErrorInesperado, Window.GetWindow(this));
                SingletonGestorVentana.Instancia.Regresar();
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
                _ = UnirseSalaAsync(SingletonCliente.Instance.IdSala);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion, Window.GetWindow(this));
                SingletonGestorVentana.Instancia.Regresar();
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
                SingletonGestorVentana.Instancia.Regresar();
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion, Window.GetWindow(this));
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
                SingletonGestorVentana.Instancia.Regresar();
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion, Window.GetWindow(this));
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
                radioButtonEspacio.Content = Idioma.buttonEspacio;
                radioButtonFinCartas.Content = Idioma.buttonFinCartas;
                radioButtonFinRondas.Content = Idioma.buttonFinRondas;
                labelExpulsarUsuario.Content = Idioma.buttonExpulsar;
                labelNumeroRondas.Content = Idioma.labelNumeroRondas;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
        }

        public void EmpezarPartidaCallback(string idPartida)
        {
            if (!ValidacionExistenciaJuego.ExistePartida(idPartida))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloLobbyNoEncontrado, Properties.Idioma.mensajeLobbyNoEncontrado, Window.GetWindow(this));
                SingletonGestorVentana.Instancia.Regresar();
                return;
            }
            SingletonCliente.Instance.IdPartida = idPartida;
            AbrirPartida();
        }


        private async void ClicButtonEmpezarPartidaAsync(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                SingletonGestorVentana.Instancia.Regresar();
                return;
            }
            if (JugadoresSala.Count < MINIMO_JUGADORES || JugadoresSala.Count > MAXIMO_JUGADORES)
            {
                MostrarVentanaNoPuedeIniciarPartida();
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
                        AbrirPartida();
                    }
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                }

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
                Window.GetWindow(this)
            );
        }

        private void MostrarVentanaNoPuedeIniciarPartida()
        {
            VentanasEmergentes.CrearVentanaEmergenteConCierre(
                Properties.Idioma.tituloErrorPartida,
                Idioma.mensajeErrorPartida,
                Window.GetWindow(this)
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
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
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
        private void AbrirPartida()
        {
            if (SingletonCliente.Instance.IdPartida == null)
            {
                SingletonGestorVentana.Instancia.Regresar();
                return;
            }
            PartidaPage partida = new PartidaPage(SingletonCliente.Instance.IdPartida);
            if (partida != null)
            {
                SingletonGestorVentana.Instancia.NavegarA(partida);
            }
        }

        private void ClicButtonCopiar(object sender, MouseButtonEventArgs e)
        {
            if (labelCodigo.Content != null)
            {
                Clipboard.SetText(labelCodigo.Content.ToString());

                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCodigoCopiado,
                    Properties.Idioma.mensajeCodigoCopiado,
                    Window.GetWindow(this));
            }
        }

        private async void ClicButtonInvitarAmigosAsync(object sender, RoutedEventArgs e)
        {
            string gamertagInvitado = AbrirVentanaModalGamertag();
            if (gamertagInvitado != null && gamertagInvitado != SingletonCliente.Instance.NombreUsuario)
            {
                if (await EnviarInvitacionAsync(gamertagInvitado))
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloInvitacionPartida,
                        Properties.Idioma.mensajeInvitacionExitosa,
                        Window.GetWindow(this));
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloInvitacionPartida,
                        Properties.Idioma.mensajeInvitacionFallida,
                        Window.GetWindow(this));
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloInvitacionPartida,
                    Properties.Idioma.mensajeInvitacionFallida,
                    Window.GetWindow(this));
            }
        }

        private string AbrirVentanaModalGamertag()
        {
            string valorObtenido = null;
            IngresarGamertagModalWindow ventanaModal = new IngresarGamertagModalWindow();
            try
            {
                ventanaModal.Owner = Window.GetWindow(this);

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            bool? resultado = ventanaModal.ShowDialog();

            if (resultado == true)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }

            return valorObtenido;
        }

        private async Task<bool> EnviarInvitacionAsync(string nombreReceptor)
        {
            bool resultado = false;
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                return false;
            }
            try
            {
                resultado = await SingletonCanal.Instancia.InvitacionPartida.EnviarInvitacionAsync(new InvitacionPartida
                {
                    CodigoSala = SingletonCliente.Instance.IdSala,
                    NombreEmisor = SingletonCliente.Instance.NombreUsuario,
                    NombreReceptor = nombreReceptor
                });
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloInvitacionPartida,
                    Properties.Idioma.mensajeInvitacionFallida,
                    Window.GetWindow(this));
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
                { radioButtonMitologia, TematicaPartida.Mitologia },
                { radioButtonEspacio, TematicaPartida.Espacio }

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

        private async void ClicButtonEliminarUsuarioAsync(object sender, RoutedEventArgs e)
        {
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                Application.Current.Shutdown();
                return;
            }
            if (sender is Button boton && boton.DataContext is Usuario usuario)
            {
                boton.IsEnabled = false;
                boton.Visibility = Visibility.Hidden;
                try
                {
                    if (SingletonCliente.Instance.NombreUsuario.Equals(usuario.Nombre, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException();
                    }
                    await SingletonSalaJugador.Instancia.Sala.ExpulsarJugadorSalaAsync(
                        SingletonCliente.Instance.NombreUsuario,
                        usuario.Nombre,
                        SingletonCliente.Instance.IdSala);
                }
                catch(ArgumentException)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloExpulsionInvalida, Idioma.mensajeNoAutoExpulsion, Window.GetWindow(this));
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                }
                finally
                {
                    try
                    {
                        boton.IsEnabled = true;
                        boton.Visibility = Visibility.Visible;
                    }
                    catch (InvalidOperationException excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                    }
                    catch (Exception excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                    }
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(TIEMPO_CLIC_EXPULSION_SEGUNDOS));
        }

        private void ClicImagenFlechaAtras(object sender, MouseButtonEventArgs e)
        {
            SingletonGestorVentana.Instancia.Regresar();
        }
        public void DelegacionRol(bool esAnfitrion)
        {
            VerDiposicionAnfitrion();
        }

        private void ClicFlechaAtras(object sender, MouseButtonEventArgs e)
        {
            SingletonGestorVentana.Instancia.Regresar();
        }

        private void CerrandoPage(object sender, RoutedEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            if (listaUsuarioSalaUserControl != null)
            {
                try
                {
                    
                    listaUsuarioSalaUserControl = null;
                    JugadoresSala = new ObservableCollection<Usuario>();
                }
                catch(Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                }
            }
        }
    }
}

