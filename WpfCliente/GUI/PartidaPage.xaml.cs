using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using WpfCliente.Contexto;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class PartidaPage : IHabilitadorBotones, INotifyPropertyChanged , IActualizacionUI
    {
        private const int SELECCION_MAXIMA_NARRADOR = 1;
        private const int SELECCION_MAXIMA_JUGADOR = 1;
        private const int ADIVINAR_MAXIMA_JUGADOR = 1;
        private const int UNICA_SOLICITUD = 1;
        private const int CONTADOR_SELECCION_CERO = 0;
        private const int MAXIMO_EJECUCION_VERIFICACION = 10;
        private int contadorSeleccion = 0;
        private int contadorSeleccionAdivinar = 0;
        private const int INICIALIZAR_CONTADOR = 0;
        private const int TIEMPO_CARTA_SEGUNDOS = 6;
        private const int TIEMPO_MENSAJE_SEGUNDOS = 3;
        private const int TIEMPO_ESPERA_VERIFICACON = 20;
        private const string NOMBRE_RESERVADO= "guest";
        private DispatcherTimer evaluadorConexionServidor;
        public event PropertyChangedEventHandler PropertyChanged;
        SeleccionCartaUserControl seleccionCartasUserControl;
        NarradorSeleccionCartaUserControl narradorSeleccionCartasUserControl;
        VerTodasCartasUserControl verTodasCartasUserControl;
        ResumenRondaUserControl resumenRondaUserControl;
        private MostrarCartaModalWindow ventanaModalCartas;
        private DispatcherTimer temporizador;
        private bool primeraEjecucion = true;
        private bool enEjecucionVerificacion = false;
        private int contadorDeEjecuciones = 0;
        private bool esNarrador;
        public bool EsNarrador
        {
            get => esNarrador;
            set
            {
                esNarrador = value;
                if (value)
                {
                    AvanzarPantalla(PantallasPartida.PANTALLA_JUGADOR_SELECION);
                }
                else
                {
                    AvanzarPantalla(PantallasPartida.PANTALLA_NARRADOR_SELECION);
                }
                OnPropertyChanged();
            }
        }
        private int pantallaActual;
        public int PantallaActual
        {
            get => pantallaActual;
            set
            {
                pantallaActual = value;
                OnPropertyChanged();
            }
        }
        private bool comandoHabilitado = true;
        public bool ComandoHabilitado
        {
            get => comandoHabilitado;
            set
            {
                comandoHabilitado = value;
                OnPropertyChanged();
                (ComandoImagenSelecionCorrecta as ComandoRele<string>)?.RaiseCanExecuteChanged();
            }
        }
        public ICommand ComandoImagenGlobal { get; set; }
        public ICommand ComandoImagenSelecionCorrecta { get; set; }

        public PartidaPage(string idPartida)
        {
            KeepAlive = false;
            SingletonPartida.Instancia.CambiarPantalla += CambiarPantalla;
            SingletonPartida.Instancia.NotificarEsNarrador += NotificarNarrador;
            SingletonPartida.Instancia.MostrarPista += MostrarPista;
            SingletonPartida.Instancia.DesbloquearChat += DesbloqueoChat;
            SingletonPartida.Instancia.PerdisteTurno += PerdisteTurno;
            SingletonPartida.Instancia.TeHanExpulsado += EnExpulsion;
            SingletonPartida.Instancia.PartidaFaltaJugadores += FaltaJugadores;
            InitializeComponent();
            InicializarComponenetes();
            DataContext = this;
            _ = UnirsePartidaAsync(idPartida);
            ActualizarUI();
            textBlockAvisos.Visibility = Visibility.Collapsed;
            ConfigurarTemporizador();
        }

        private void ConfigurarTemporizador()
        {
            evaluadorConexionServidor = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(TIEMPO_ESPERA_VERIFICACON)
            };
            evaluadorConexionServidor.Tick += async (emisor, evento) =>
            {
                try
                {
                    if (enEjecucionVerificacion)
                    {
                        contadorDeEjecuciones++;
                        if (contadorDeEjecuciones <MAXIMO_EJECUCION_VERIFICACION)
                        {
                            evaluadorConexionServidor.Stop();

                        }
                        return;
                    }
                    contadorDeEjecuciones = 0;
                    enEjecucionVerificacion = true;
                    bool continuar = await VerificarConexionPartida();
                    if (!continuar)
                    {
                        evaluadorConexionServidor.Stop();
                    }
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                }
            };
            evaluadorConexionServidor.Start();
        }

        private async Task<bool> VerificarConexionPartida()
        {
            try
            {
                labelEstadoConexion.Content = Idioma.labelVerificandoConexion;
                bool conexionExitosa = await Task.Run(async () =>
                {
                    try
                    {
                        return await Conexion.VerificarExistenciaPartida();

                    }
                    catch (CommunicationException)
                    {
                        throw new CommunicationException();
                    }
                    catch (ArgumentException)
                    {
                        throw new ArgumentException();
                    }
                    catch (Exception excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                    }
                    return false;
                });

                labelEstadoConexion.Content = conexionExitosa ? Idioma.labelConectado : Idioma.labelErrorConexion;
                labelEstadoConexion.Visibility = Visibility.Visible;
                return true;
            }
            catch (CommunicationException excepcion)
            {
                ActualizarUI();
                labelEstadoConexion.Content = Idioma.labelErrorAlVerificarConexion;
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                VentanasEmergentes.CrearVentanaEmergenteConCierre(Idioma.tituloErrorServidor, Idioma.mensajeErrorServidor, Window.GetWindow(this));
            }
            catch (ArgumentException excepcion)
            {
                labelEstadoConexion.Content = Idioma.labelErrorConexion;
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloErrorServidor, Idioma.mensajePartidaExpiro, Window.GetWindow(this));
                chatUserControl.IsEnabled = false;
                buttonSolicitarImagen.IsEnabled = false;
            }
            catch (Exception excepcion)
            {
                labelEstadoConexion.Content = Idioma.labelErrorAlVerificarConexion;
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            return false;
        }

        private void FaltaJugadores()
        {
            try
            {
                textBlockAvisos.Visibility = Visibility.Visible;
                textBlockAvisos.Text = Idioma.labelFaltaJugadoresPartida;
                TerminoPartida();
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }

        private void EnExpulsion()
        {
            TerminoPartida();
            try
            {
                textBlockAvisos.Text = Idioma.labelHasSidoExpulsado;
                textBlockAvisos.Visibility = Visibility.Visible;
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }

        private void TerminoPartida()
        {
            try
            {
                enEjecucionVerificacion = true;
                buttonSolicitarImagen.Visibility = Visibility.Hidden;
                buttonSolicitarImagen.IsEnabled = false;
                chatUserControl.Visibility = Visibility.Hidden;
                chatUserControl.IsEnabled = false;
                PantallaActual = PantallasPartida.PANTALLA_FIN_PARTIDA;
                evaluadorConexionServidor.Stop();
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            SingletonPartida.Instancia.CerrarConexionPartida();
            SingletonChat.Instancia.CerrarConexionChat();
        }

        private void PerdisteTurno()
        {
            try
            {
                textBlockAvisos.Visibility = Visibility.Visible;
                temporizador = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(TIEMPO_MENSAJE_SEGUNDOS)
                };
                temporizador.Tick += (emisor, argumentos) =>
                {
                    textBlockAvisos.Visibility = Visibility.Collapsed;
                    temporizador.Stop();
                };
                temporizador.Start();
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }

        public void DesbloqueoChat()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    chatUserControl.IsEnabled = true;
                }
                catch (InvalidOperationException excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                }
            });
        }

        private void InicializarComponenetes()
        {
            try
            {
                chatUserControl.IsEnabled = false;
                ComandoImagenGlobal = new ComandoRele<string>(ComandoImagenPorId);
                ComandoImagenSelecionCorrecta = new ComandoRele<string>(ComandoTratarAdivinarAsync,
                    (param) => ComandoHabilitado);
                narradorSeleccionCartasUserControl = new NarradorSeleccionCartaUserControl();
                seleccionCartasUserControl = new SeleccionCartaUserControl();
                verTodasCartasUserControl = new VerTodasCartasUserControl();
                resumenRondaUserControl = new ResumenRondaUserControl();
                gridPantallaCartaMazo.Children.Add(seleccionCartasUserControl);
                gridPantallaCartasNarrador.Children.Add(narradorSeleccionCartasUserControl);
                gridPantallaTodasCartas.Children.Add(verTodasCartasUserControl);
                gridPantallaResumenRonda.Children.Add(resumenRondaUserControl);
                PantallaActual = PantallasPartida.PANTALLA_INICIO;
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CambiarPantalla(int numeroPantalla)
        {
            if (numeroPantalla == PantallasPartida.PANTALLA_FIN_PARTIDA)
            {
                TerminoPartida();
                return;
            }
            if (primeraEjecucion)
            {
                buttonSolicitarImagen.IsEnabled = true;
                primeraEjecucion = false;
            }
            if (esNarrador && numeroPantalla == PantallasPartida.PANTALLA_TODOS_CARTAS)
            {
                comandoHabilitado = false;
            }
            else
            {
                comandoHabilitado = true;
            }
            (ComandoImagenSelecionCorrecta as ComandoRele<string>)?.RaiseCanExecuteChanged();
            PantallaActual = numeroPantalla;
            try
            {
                ventanaModalCartas?.Close();
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }

        public void NotificarNarrador(bool esNarrador)
        {
            EsNarrador = esNarrador;
        }

        public void MostrarPista(string pista)
        {
            if (EsNarrador)
            {
                AvanzarPantalla(PantallasPartida.PANTALLA_ESPERA);
            }
        }

        public void AvanzarPantalla(int numeroPantallla)
        {
            PantallaActual = numeroPantallla;
        }

        private async Task UnirsePartidaAsync(string idPartida)
        {
            try
            {
                bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
                if (!conexionExitosa)
                {
                    return;
                }
                if (!ValidacionExistenciaJuego.ExistePartida(idPartida))
                {
                    VentanasEmergentes.CrearVentanaEmergenteConCierre(Properties.Idioma.tituloErrorInesperado,
                        Properties.Idioma.mensajeErrorInesperado,
                        Window.GetWindow(this));
                    await SalirDePartidaAsync();
                    return;
                }
                var resultado = SingletonPartida.Instancia.AbrirConexionPartida();
                if (!resultado)
                {
                    await SalirDePartidaAsync();
                    return;
                }
                await SingletonPartida.Instancia.Partida.UnirsePartidaAsync(SingletonCliente.Instance.NombreUsuario, idPartida);
            }
            catch (FaultException<PartidaFalla> excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergenteConCierre(Properties.Idioma.tituloErrorUnirsePartida,
                        Properties.Idioma.mensajeErrorUnirsePartida,
                        Window.GetWindow(this));
                await SalirDePartidaAsync();
                ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
            }
            catch (Exception excepcion)
            {
                await SalirDePartidaAsync();
                ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
            }
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            seleccionCartasUserControl.IsEnabled = esHabilitado;
            resumenRondaUserControl.IsEnabled = esHabilitado;
            narradorSeleccionCartasUserControl.IsEnabled = esHabilitado;
            verTodasCartasUserControl.IsEnabled = esHabilitado;
        }

        private async Task SalirDePartidaAsync()
        {
            BindingOperations.ClearAllBindings(this);
            SingletonPartida.Instancia.CerrarConexionPartida();
            SingletonChat.Instancia.CerrarConexionChat();
            bool resultado = await Conexion.VerificarConexionAsync(HabilitarBotones,Window.GetWindow(this));
            if (!resultado)
            {
                SingletonGestorVentana.Instancia.NavegarA(new IniciarSesionPage());
                return;
            }

            if (SingletonCliente.Instance.NombreUsuario != null 
                && SingletonCliente.Instance.NombreUsuario is string nombre)
            {
                if (String.IsNullOrEmpty(nombre) || nombre.ToLower().Contains(NOMBRE_RESERVADO))
                {
                    SingletonGestorVentana.Instancia.NavegarA(new IniciarSesionPage());
                    SingletonGestorVentana.Instancia.LimpiarHistorial();
                }
                else
                {
                    SingletonGestorVentana.Instancia.NavegarAMenuDesdePartida(new MenuPage());
                    SingletonGestorVentana.Instancia.LimpiarHistorial();
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloErrorInesperado,
                   Idioma.mensajeErrorInesperado,
                   Window.GetWindow(this));
            }
        }

        public void ComandoImagenPorId(string claveImagen)
        {
            if (EsNarrador)
            {
                _ = EscogerImagenNarradorAsync(claveImagen);
            }
            else
            {
                _ = EscogerImagenPorIdAsync(claveImagen);
            }
        }

        public async Task EscogerImagenPorIdAsync(string id)
        {
            ImagenCarta imagenEscogida = SingletonGestorImagenes.Instancia.imagnesMazo.ImagenesMazo
                .RealizarConsultaSegura(lista => lista.FirstOrDefault(busqueda => busqueda.IdImagen == id));
            if (imagenEscogida == null)
            {
                return;
            }
            string claveImagen = imagenEscogida.IdImagen;
            ventanaModalCartas = new MostrarCartaModalWindow(false, imagenEscogida.BitmapImagen , () => ventanaModalCartas?.Close());
            bool? resultado = ventanaModalCartas.ShowDialog();
            if ((bool)resultado)
            {
                try
                {
                    contadorSeleccion++;
                    SingletonGestorImagenes.Instancia.imagnesMazo.ImagenesMazo.Remove(imagenEscogida);
                    await Conexion.VerificarConexionSinBaseDatosAsync(HabilitarBotones, Window.GetWindow(this));
                    await SingletonPartida.Instancia.Partida.ConfirmarMovimientoAsync(SingletonCliente.Instance.NombreUsuario,
                        SingletonCliente.Instance.IdPartida,
                        imagenEscogida.IdImagen,
                        null);
                    await SingletonGestorImagenes.Instancia.imagnesMazo.Imagen.SolicitarImagenMazoAsync(SingletonCliente.Instance.IdPartida, UNICA_SOLICITUD);

                    if (contadorSeleccion >= SELECCION_MAXIMA_JUGADOR)
                    {
                        AvanzarPantalla(PantallasPartida.PANTALLA_ESPERA);
                        contadorSeleccion = CONTADOR_SELECCION_CERO;
                    }
                }
                catch (CommunicationException excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
                }
                catch (InvalidOperationException excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
                }

            }

        }

        public async Task EscogerImagenNarradorAsync(string id)
        {
            ImagenCarta imagenAEscoger = SingletonGestorImagenes.Instancia.imagnesMazo.ImagenesMazo
               .RealizarConsultaSegura(lista => lista.FirstOrDefault(busqueda => busqueda.IdImagen == id));
            if (imagenAEscoger == null)
            {
                return;
            }
            ventanaModalCartas = new MostrarCartaModalWindow(true, imagenAEscoger.BitmapImagen, () => ventanaModalCartas?.Close());
            bool? resultado = ventanaModalCartas.ShowDialog();
            string pista = ventanaModalCartas.Pista;
            if ((bool)resultado)
            {
                try
                {
                    contadorSeleccion++;
                    SingletonGestorImagenes.Instancia.imagnesMazo.ImagenesMazo.Remove(imagenAEscoger);
                    await Conexion.VerificarConexionSinBaseDatosAsync(HabilitarBotones, Window.GetWindow(this));
                    await SingletonPartida.Instancia.Partida.ConfirmarMovimientoAsync(
                        SingletonCliente.Instance.NombreUsuario,
                        SingletonCliente.Instance.IdPartida,
                        imagenAEscoger.IdImagen,
                        pista);
                    await SingletonGestorImagenes.Instancia.imagnesMazo.Imagen
                        .SolicitarImagenMazoAsync(SingletonCliente.Instance.IdPartida, UNICA_SOLICITUD);
                    if (contadorSeleccion >= SELECCION_MAXIMA_NARRADOR)
                    {
                        AvanzarPantalla(PantallasPartida.PANTALLA_ESPERA);
                        contadorSeleccion = CONTADOR_SELECCION_CERO;
                    }
                }
                catch (CommunicationException excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
                }
                catch (InvalidOperationException excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
                }

            }
        }

        private async void ComandoTratarAdivinarAsync(string idImagen)
        {
            ImagenCarta imagenAEscoger =
                SingletonGestorImagenes.Instancia.imagenesTablero.ImagenesTablero
                    .RealizarConsultaSegura(lista => lista.FirstOrDefault(busqueda => busqueda.IdImagen == idImagen));
            if (imagenAEscoger == null)
            {
                return;
            }
            ventanaModalCartas = new MostrarCartaModalWindow(false, imagenAEscoger.BitmapImagen, () => ventanaModalCartas?.Close());
            bool? resultado = ventanaModalCartas.ShowDialog();
            string pista = ventanaModalCartas.Pista;
            if ((bool)resultado)
            {
                try
                {
                    contadorSeleccionAdivinar++;
                    await Conexion.VerificarConexionSinBaseDatosAsync(HabilitarBotones, Window.GetWindow(this));
                    await SingletonPartida.Instancia.Partida.TratarAdivinarAsync(SingletonCliente.Instance.NombreUsuario,
                        SingletonCliente.Instance.IdPartida, idImagen);
                    if (contadorSeleccionAdivinar >= ADIVINAR_MAXIMA_JUGADOR)
                    {
                        AvanzarPantalla(PantallasPartida.PANTALLA_ESPERA);
                        contadorSeleccionAdivinar = INICIALIZAR_CONTADOR;
                    }

                }
                catch (CommunicationException excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
                }
                catch (InvalidOperationException excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
                }
            }
        }

        public async void ClicImagenFlechaAtrasAsync(object sender, RoutedEventArgs e)
        {
            await SalirDePartidaAsync();
        }

        private async void ClicButtonSolicitarImagenAsync(object sender, RoutedEventArgs e)
        {
            buttonSolicitarImagen.IsEnabled = false;
            try
            {
                var numeroCartasActual = 
                    SingletonGestorImagenes.Instancia.imagnesMazo.ImagenesMazo.ContarSeguro();

                if (numeroCartasActual > SingletonGestorImagenes.MAXIMO_IMAGENES_MAZO)
                {
                    await SingletonGestorImagenes.Instancia.imagnesMazo.Imagen
                        .SolicitarImagenMazoAsync(SingletonCliente.Instance.IdPartida, UNICA_SOLICITUD);
                    await Task.Delay(TimeSpan.FromSeconds(TIEMPO_CARTA_SEGUNDOS));
                }
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
            }
            finally
            {
                buttonSolicitarImagen.IsEnabled = true;
            }
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            buttonSolicitarImagen.Content = Idioma.buttonSolicitarImagen;
            textBlockAvisos.Text = Idioma.labelPerdisteTurno;
            labelFinPartida.Content = Idioma.labelFinPartida;
        }

    }
}
