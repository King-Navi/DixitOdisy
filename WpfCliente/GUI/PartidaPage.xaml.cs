using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
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
        private const int TIEMPO_CARTA_SEGUNDOS = 3;
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

        public PartidaPage(string idPartida)
        {
            KeepAlive = false;
            SingletonPartida.Instancia.CambiarPantalla += CambiarPantalla;
            SingletonPartida.Instancia.NotificarEsNarrador += NotificarNarrador;
            SingletonPartida.Instancia.MostrarPista += this.MostrarPista;
            SingletonPartida.Instancia.DesbloquearChat += DesbloqueoChat;
            InitializeComponent();
            InicializarComponenetes();
            DataContext = this;
            UnirsePartidaAsync(idPartida);
            ActualizarUI();

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
                ComandoImagenSelecionCorrecta = new ComandoRele<string>(ComandoSeleccionCorrectaAsync,
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
        private async void ComandoSeleccionCorrectaAsync(string idImagen)
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
                    SalirDePartida();
                    return;
                }
                var resultado = SingletonPartida.Instancia.AbrirConexionPartida();
                if (!resultado)
                {
                    SalirDePartida();
                    return;
                }
                await SingletonPartida.Instancia.Partida.UnirsePartidaAsync(SingletonCliente.Instance.NombreUsuario, idPartida);
            }
            catch (FaultException<PartidaFalla> excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergenteConCierre(Properties.Idioma.tituloErrorUnirsePartida,
                        Properties.Idioma.mensajeErrorUnirsePartida,
                        Window.GetWindow(this));
                SalirDePartida();
                ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
            }
            catch (Exception excepcion)
            {
                SalirDePartida();
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

        private void SalirDePartida()
        {
            BindingOperations.ClearAllBindings(this);
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
                    SingletonGestorVentana.Instancia.NavegarA(new MenuPage());
                    SingletonGestorVentana.Instancia.LimpiarHistorial();
                }
                SingletonPartida.Instancia.CerrarConexionPartida();
                SingletonChat.Instancia.CerrarConexionChat();
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloErrorInesperado,
                   Idioma.mensajeErrorInesperado,
                   Window.GetWindow(this));
            }
        }


        public void ComandoImagenPorId(string id)
        {
            if (EsNarrador)
            {
                EscogerImagenNarradorAsync(id);
            }
            else
            {
                EscogerImagenPorIdAsync(id);
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
                await SingletonPartida.Instancia.Partida.ConfirmarMovimientoAsync(SingletonCliente.Instance.NombreUsuario, 
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

        private void BORRAME_SImulacionCambioRonda(object sender, RoutedEventArgs e)
        {
            CambiarPantalla(PantallasPartida.PANTALLA_TODOS_CARTAS);
        }
        public void ClicImagenFlechaAtras(object sender, RoutedEventArgs e)
        {
            SalirDePartida();
        }
        private void BORRAME_SImulacionCambioRondaSoyJugador(object sender, RoutedEventArgs e)
        {
            NotificarNarrador(false);

        }

        private void BORRAME_SImulacionCambioRondaSoyNarrador(object sender, RoutedEventArgs e)
        {
            NotificarNarrador(true);

        }

        private void BORRAME_SImulacionCambioRondaStats(object sender, RoutedEventArgs e)
        {
            CambiarPantalla(PantallasPartida.PANTALLA_ESTADISTICAS);

        }

        private async void ClicButtonSolicitarImagenAsync(object sender, RoutedEventArgs e)
        {
            
            await SingletonGestorImagenes.Instancia.imagnesMazo.Imagen.SolicitarImagenCartaAsync(SingletonCliente.Instance.IdPartida);
            await Task.Delay(TimeSpan.FromSeconds(TIEMPO_CARTA_SEGUNDOS));
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            buttonSolicitarImagen.Content = Idioma.buttonSolicitarImagen;
        }

    }
}
