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
        private const int CONTADOR_SELECCION_CERO = 0;
        private int contadorSeleccion = 0;
        private int contadorSeleccionAdivinar = 0;
        private const int INICIALIZAR_CONTADOR = 0;
        private const int TIEMPO_CARTA_SEGUNDOS = 6;
        private const int TIEMPO_MENSAJE_SEGUNDOS = 3;
        private const string NOMBRE_RESERVADO= "guest";
        public ICommand ComandoImagenGlobal { get; set; }
        public ICommand ComandoImagenSelecionCorrecta { get; set; }
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
        public event PropertyChangedEventHandler PropertyChanged;
        SeleccionCartaUserControl seleccionCartasUserControl;
        NarradorSeleccionCartaUserControl narradorSeleccionCartasUserControl;
        VerTodasCartasUserControl verTodasCartasUserControl;
        ResumenRondaUserControl resumenRondaUserControl;
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
        private MostrarCartaModalWindow ventanaModalCartas;
        private DispatcherTimer temporizador;
        private bool primeraEjecucion = true;

        public PartidaPage(string idPartida)
        {
            KeepAlive = false;
            SingletonPartida.Instancia.CambiarPantalla += CambiarPantalla;
            SingletonPartida.Instancia.NotificarEsNarrador += NotificarNarrador;
            SingletonPartida.Instancia.MostrarPista += this.MostrarPista;
            SingletonPartida.Instancia.DesbloquearChat += DesbloqueoChat;
            SingletonPartida.Instancia.PerdisteTurno += PerdisteTurno;
            SingletonPartida.Instancia.SeTerminoPartida += TerminoPartida;
            SingletonPartida.Instancia.TeHanExpulsado += EnExpulsion;
            InitializeComponent();
            InicializarComponenetes();
            DataContext = this;
            UnirsePartidaAsync(idPartida);
            ActualizarUI();
            textBlockPerdisteTurno.Visibility = Visibility.Collapsed;
        }

        private void EnExpulsion()
        {
            TerminoPartida();
            try
            {
                textBlockPerdisteTurno.Text = Idioma.labelHasSidoExpulsado;
                textBlockPerdisteTurno.Visibility = Visibility.Visible;
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
                buttonSolicitarImagen.Visibility = Visibility.Hidden;
                buttonSolicitarImagen.IsEnabled = false;
                chatUserControl.Visibility = Visibility.Hidden;
                chatUserControl.IsEnabled = false;
                CambiarPantalla(PantallasPartida.PANTALLA_FIN_PARTIDA);
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
                textBlockPerdisteTurno.Visibility = Visibility.Visible;
                temporizador = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(TIEMPO_MENSAJE_SEGUNDOS)
                };
                temporizador.Tick += (emisor, argumentos) =>
                {
                    textBlockPerdisteTurno.Visibility = Visibility.Collapsed;
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
            chatUserControl.IsEnabled = true;
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
        private async void ComandoTratarAdivinarAsync(string idImagen)
        {
            ImagenCarta imagenAEscoger =
                SingletonGestorImagenes.Instancia.imagenesDeTodos.ImagenCartasTodos
                    .RealizarConsultaSegura(lista => lista.FirstOrDefault(busqueda => busqueda.IdImagen == idImagen));
            if (imagenAEscoger == null)
                return;
            ventanaModalCartas = new MostrarCartaModalWindow(false, imagenAEscoger.BitmapImagen, () => ventanaModalCartas?.Close());
            bool? resultado = ventanaModalCartas.ShowDialog();
            string pista = ventanaModalCartas.Pista;
            if ((bool)resultado)
            {
                contadorSeleccionAdivinar++;
                await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
                await SingletonPartida.Instancia.Partida.TratarAdivinarAsync(SingletonCliente.Instance.NombreUsuario,
                    SingletonCliente.Instance.IdPartida, idImagen);
                if (contadorSeleccionAdivinar >= ADIVINAR_MAXIMA_JUGADOR)
                {
                    AvanzarPantalla(PantallasPartida.PANTALLA_ESPERA);
                    contadorSeleccionAdivinar = INICIALIZAR_CONTADOR;
                }

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

        private async void UnirsePartidaAsync(string idPartida)
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
                EscogerImagenNarradorAsync(claveImagen);
            }
            else
            {
                EscogerImagenPorIdAsync(claveImagen);
            }
        }

        public async void EscogerImagenPorIdAsync(string id)
        {
            ImagenCarta imagenEscogida = SingletonGestorImagenes.Instancia.imagnesMazo.ImagenCartasMazo
                .RealizarConsultaSegura(lista => lista.FirstOrDefault(busqueda => busqueda.IdImagen == id));
            if (imagenEscogida == null)
                return;
            string claveImagen = imagenEscogida.IdImagen;
            ventanaModalCartas = new MostrarCartaModalWindow(false, imagenEscogida.BitmapImagen , () => ventanaModalCartas?.Close());
            bool? resultado = ventanaModalCartas.ShowDialog();
            if ((bool)resultado)
            {
                contadorSeleccion++;
                SingletonGestorImagenes.Instancia.imagnesMazo.ImagenCartasMazo.Remove(imagenEscogida);
                await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
                SingletonPartida.Instancia.Partida.ConfirmarMovimiento(SingletonCliente.Instance.NombreUsuario,
                    SingletonCliente.Instance.IdPartida,
                    imagenEscogida.IdImagen,
                    null);
                await SingletonGestorImagenes.Instancia.imagnesMazo.Imagen.SolicitarImagenCartaAsync(SingletonCliente.Instance.IdPartida);

                if (contadorSeleccion >= SELECCION_MAXIMA_JUGADOR)
                {
                    AvanzarPantalla(PantallasPartida.PANTALLA_ESPERA);
                    contadorSeleccion = CONTADOR_SELECCION_CERO;
                }

            }

        }

        public async void EscogerImagenNarradorAsync(string id)
        {

            ImagenCarta imagenAEscoger = SingletonGestorImagenes.Instancia.imagnesMazo.ImagenCartasMazo
               .RealizarConsultaSegura(lista => lista.FirstOrDefault(busqueda => busqueda.IdImagen == id));
            if (imagenAEscoger == null)
                return;
            ventanaModalCartas = new MostrarCartaModalWindow(true, imagenAEscoger.BitmapImagen, () => ventanaModalCartas?.Close());
            bool? resultado = ventanaModalCartas.ShowDialog();
            string pista = ventanaModalCartas.Pista;
            if ((bool)resultado)
            {
                contadorSeleccion++;
                SingletonGestorImagenes.Instancia.imagnesMazo.ImagenCartasMazo.Remove(imagenAEscoger);
                await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
                await SingletonPartida.Instancia.Partida.ConfirmarMovimientoAsync(
                    SingletonCliente.Instance.NombreUsuario, 
                    SingletonCliente.Instance.IdPartida, 
                    imagenAEscoger.IdImagen, 
                    pista);
                await SingletonGestorImagenes.Instancia.imagnesMazo.Imagen.SolicitarImagenCartaAsync(SingletonCliente.Instance.IdPartida);
                if (contadorSeleccion >= SELECCION_MAXIMA_NARRADOR)
                {
                    AvanzarPantalla(PantallasPartida.PANTALLA_ESPERA);
                    contadorSeleccion = CONTADOR_SELECCION_CERO;
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
                    SingletonGestorImagenes.Instancia.imagnesMazo.ImagenCartasMazo.ContarSeguro();

                if (numeroCartasActual > SingletonGestorImagenes.MAXIMO_IMAGENES_MAZO)
                {
                    await SingletonGestorImagenes.Instancia.imagnesMazo.Imagen
                        .SolicitarImagenCartaAsync(SingletonCliente.Instance.IdPartida);
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
            textBlockPerdisteTurno.Text = Idioma.labelPerdisteTurno;
            labelFinPartida.Content = Idioma.labelFinPartida;
        }

    }
}
