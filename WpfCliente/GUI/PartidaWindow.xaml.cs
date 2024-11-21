using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// Multiples hilos pueden acceder a esto ¡Cuidado!
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class PartidaWindow : Window, IActualizacionUI, IHabilitadorBotones, IServicioPartidaSesionCallback, INotifyPropertyChanged
    {
        private const int PANTALLA_INICIO= 1;
        private const int PANTALLA_NARRADOR_SELECION=2 ;
        private const int PANTALLA_JUGADOR_SELECION = 3;
        private const int PANTALLA_TODOS_CARTAS = 4;
        private const int PANTALLA_ESTADISTICAS= 5;
        private const int PANTALLA_FIN_PARTIDA = 6;
        private const int PANTALLA_ESPERA= 7 ;
        //TODO: lo define la partida
        private readonly int seleccionMaximaNarrador = 1; 
        private readonly int seleccionMaximaJugador = 1; 
        private int contadorSeleccion =  0;
        public ICommand ComandoImagenGlobal { get; set; }
        public ICommand ComandoImagenSelecionCorrecta { get; set; }
        private RecursosCompartidos recursosCompartidos;
        public event PropertyChangedEventHandler PropertyChanged;
        SeleccionCartaUsercontrol seleccionCartasUserControl;
        NarradorSeleccionCartaUserControl narradorSeleccionCartasUserControl;
        VerTodasCartasUserControl VerTodasCartasUserControl;
        private readonly SemaphoreSlim semaphoreRecibirImagenCallback = new SemaphoreSlim(1,1);
        private Window ventanaMenu;


        private bool esNarrador;
        public bool EsNarrador
        {
            get => esNarrador;
            set
            {
                esNarrador = value;
                if (value)
                {
                    AvanzarPantalla(PANTALLA_JUGADOR_SELECION);
                }
                else
                {
                    AvanzarPantalla(PANTALLA_NARRADOR_SELECION);
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




        public PartidaWindow(string idPartida)
        {
            ConfigurarVentanaMenu();
            InitializeComponent();
            InicializarComponenetes();
            ActualizarUI();
            DataContext = this;
            UnirsePartida(idPartida);
        }

        private void ConfigurarVentanaMenu()
        {
            // Encontrar la ventana del menú y ocultarla
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MenuWindow ventanaMenu)
                {
                    this.ventanaMenu = ventanaMenu;
                }
            }
        }

        private async Task SolicitarMazoAsync()
        {
            var tareasSolicitudes = new List<Task>();
            for (int i = 0; i < 6; i++)
            {
                tareasSolicitudes.Add(Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida));
            }
            await Task.WhenAll(tareasSolicitudes);
        }

        private void InicializarComponenetes()
        {

            recursosCompartidos = new RecursosCompartidos();
            ComandoImagenGlobal = new ComandoRele<string>(ComandoImagenPorId);
            ComandoImagenSelecionCorrecta = new ComandoRele<string>(ComandoSeleccionCorrecta);
            narradorSeleccionCartasUserControl = new NarradorSeleccionCartaUserControl(recursosCompartidos.Imagenes);
            seleccionCartasUserControl = new SeleccionCartaUsercontrol(recursosCompartidos.Imagenes);
            VerTodasCartasUserControl = new VerTodasCartasUserControl(recursosCompartidos.GruposDeImagenes);
            gridPantalla2.Children.Add(seleccionCartasUserControl);
            gridPantalla3.Children.Add(narradorSeleccionCartasUserControl);
            gridPantalla4.Children.Add(VerTodasCartasUserControl);
            PantallaActual = PANTALLA_INICIO;


        }

        private void ComandoSeleccionCorrecta(string idImagen)
        {
            MessageBox.Show("Escogiste " + idImagen);
            ImagenCarta imagenAEscoger = recursosCompartidos.Imagenes.FirstOrDefault(i => i.IdImagen == idImagen);
            if (imagenAEscoger == null)
                return;
            MostrarCartaModelWindow ventanaModal = new MostrarCartaModelWindow(true, imagenAEscoger.BitmapImagen);
            bool? resultado = ventanaModal.ShowDialog();
            string pista = ventanaModal.Pista;
            if ((bool)resultado)
            {
                contadorSeleccion++;
                recursosCompartidos.Imagenes.Remove(imagenAEscoger);
                Task.Run(async () =>
                {
                    await Conexion.VerificarConexion(HabilitarBotones, this);
                    //TODO: Ver que hacer
                    Conexion.Partida.ConfirmarMovimiento(Singleton.Instance.NombreUsuario,
                                                                            Singleton.Instance.IdPartida,
                                                                            imagenAEscoger.IdImagen,
                                                                            pista);
                    await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
                });
                if (contadorSeleccion >= seleccionMaximaNarrador)
                {
                    AvanzarPantalla(PANTALLA_ESPERA);
                    contadorSeleccion = 0;
                }

            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IServicioPartidaSesionCallback

        public void CambiarPantallaCallback(int numeroPantalla)
        {
            if (numeroPantalla == PANTALLA_INICIO)
            {
                //TODO: Reiniciar pista
                MostrarPistaCallback(""); //FIXME no es la mejor manera
            }
            PantallaActual = numeroPantalla;
        }

        public void ObtenerJugadorPartidaCallback(Usuario jugardoreNuevoEnSala)
        {

        }

        public void EliminarJugadorPartidaCallback(Usuario jugardoreRetiradoDeSala)
        {
            throw new NotImplementedException();
        }

        public void AvanzarRondaCallback(int RondaActual)
        {
        }

        public void TurnoPerdidoCallback()
        {
            throw new NotImplementedException();
        }

        public void RecibirImagenCallback(ImagenCarta imagen)
        {
            //ESTO PARECE SER EL DEADLOCK :   imagenesCompartidas.Imagenes.Add(imagen);
            //-----------------Solucion 1:
            // Verifica si se está ejecutando en el hilo de la UI
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                // Si ya está en el hilo de la UI, agrega directamente
                recursosCompartidos.Imagenes.Add(imagen);
            }
            else
            {
                // Si no está en el hilo de la UI, usa el Dispatcher para invocar en el hilo de la UI
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    recursosCompartidos.Imagenes.Add(imagen);
                });
            }
            
        }

        public void NotificarNarradorCallback(bool esNarrador)
        {
            EsNarrador = esNarrador;
        }

        public void MostrarPistaCallback(string pista)
        {
           
            
            seleccionCartasUserControl.ColocarPista(pista);
            if (EsNarrador)
            {
                AvanzarPantalla(PANTALLA_ESPERA);
            }
            else
            {
                //TODO: AVANZAR A ESCOGER CARTAS Y VER CLUE LES
            }
        }

        public void EnviarEstadisticas(EstadisticasPartida estadisticas)
        {
            List<JugadorEstadisticas> jugadorEstadisticas = new List<JugadorEstadisticas>(estadisticas.Jugadores);

        }

        public async void IniciarValoresPartidaCallback(bool seUnio)
        {
            await SolicitarMazoAsync();
            Conexion.Partida.EmpezarPartida(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
            EsconderVentanaMenu();
        }

        public void RecibirGrupoImagenCallback(ImagenCarta imagen)
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                recursosCompartidos.GruposDeImagenes.Add(imagen);
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    recursosCompartidos.GruposDeImagenes.Add(imagen);
                });
            }
        }

        #endregion IServicioPartidaSesionCallback

        public void AvanzarPantalla(int numeroPantallla)
        {
            PantallaActual = numeroPantallla;
        }

        private async void UnirsePartida(string idPartida)
        {
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }
            if (!Validacion.ExistePartida(idPartida))
            {
                //No existe la partida ¿¿??
                MessageBox.Show("Partida no disponible");
                NoHayConexion();
                return;
            }
            var resultadoTask = Conexion.AbrirConexionPartidaCallbackAsync(this);
            bool resultado = resultadoTask.Result;

            if (!resultado)
            {
                NoHayConexion();
                return;
            }
            Conexion.Partida.UnirsePartida(Singleton.Instance.NombreUsuario, idPartida);
        }

        public void ActualizarUI()
        {
            //TODO: recursos .resx
        }



        public void HabilitarBotones(bool esValido)
        {
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private void NoHayConexion()
        {
            this.Close();
        }


        public void FinalizarPartida()
        {
            AvanzarPantalla(PANTALLA_FIN_PARTIDA);
        }


        private void EsconderVentanaMenu()
        {
            try
            {
                if (ventanaMenu != null)
                {
                    ventanaMenu.Hide();
                }
            }
            catch (Exception)
            {
                Application.Current.Shutdown();
            }
        }
        private void CerrandoVentana(object sender, CancelEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
            try
            {
                Conexion.CerrarChatMotor();
                Conexion.CerrarConexionesPartida();
            }
            catch (Exception)
            {
            }
            try
            {
                if (ventanaMenu != null)
                {
                    ventanaMenu.Show();
                }
            }
            catch (Exception)
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Escoge la imagen segun el rol (narrador o jugador )que le toco en la ronda 
        /// </summary>
        /// <param name="id"></param>
        public void ComandoImagenPorId(string id)
        {
            if (EsNarrador)
            {
                EscogerImagenNarrador(id);
            }
            else
            {
                EscogerImagenPorId(id);
            }
        }
        /// <summary>
        /// Envia la imagen escoigda en base a la pista por el Jugador al servidor
        /// </summary>
        /// <param name="id"></param>
        public void EscogerImagenPorId(string id)
        {
            ImagenCarta imagenEscogida = recursosCompartidos.Imagenes.FirstOrDefault(i => i.IdImagen == id);
            if (imagenEscogida == null)
                return;
            string claveImagen = imagenEscogida.IdImagen;
            MostrarCartaModelWindow ventanaModal = new MostrarCartaModelWindow(false, imagenEscogida.BitmapImagen);
            bool? resultado = ventanaModal.ShowDialog();
            if ((bool)resultado)
            {
                contadorSeleccion++;
                recursosCompartidos.Imagenes.Remove(imagenEscogida);
                Task.Run(async () =>
                {
                    await Conexion.VerificarConexion(HabilitarBotones, this);
                    Conexion.Partida.ConfirmarMovimiento(Singleton.Instance.NombreUsuario,
                                                                            Singleton.Instance.IdPartida,
                                                                            imagenEscogida.IdImagen,
                                                                            null);
                    await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
                });
                if (contadorSeleccion >= seleccionMaximaJugador)
                {
                    AvanzarPantalla(PANTALLA_ESPERA);
                    contadorSeleccion = 0;
                }

            }

        }
        /// <summary>
        /// Envia la imagen y pista escoigda por el narrador al servidor
        /// </summary>
        /// <param name="id"></param>
        public void EscogerImagenNarrador(string id)
        {
            ImagenCarta imagenAEscoger = recursosCompartidos.Imagenes.FirstOrDefault(i => i.IdImagen == id);
            if (imagenAEscoger == null)
                return;
            MostrarCartaModelWindow ventanaModal = new MostrarCartaModelWindow(true, imagenAEscoger.BitmapImagen);
            bool? resultado = ventanaModal.ShowDialog();
            string pista = ventanaModal.Pista;
            if ((bool)resultado)
            {
                contadorSeleccion++;
                recursosCompartidos.Imagenes.Remove(imagenAEscoger);
                Task.Run(async () =>
                {
                    await Conexion.VerificarConexion(HabilitarBotones, this);
                    Conexion.Partida.ConfirmarMovimiento(Singleton.Instance.NombreUsuario,
                                                                            Singleton.Instance.IdPartida,
                                                                            imagenAEscoger.IdImagen,
                                                                            pista);
                    await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
                });
                if (contadorSeleccion >= seleccionMaximaNarrador)
                {
                    AvanzarPantalla(PANTALLA_ESPERA);
                    contadorSeleccion = 0;
                }

            }
        }


        private class RecursosCompartidos
        {
            public ObservableCollection<ImagenCarta> Imagenes { get; } = new ObservableCollection<ImagenCarta>();
            public ObservableCollection<ImagenCarta> GruposDeImagenes { get; } = new ObservableCollection<ImagenCarta>();
            public ObservableCollection<JugadorEstadisticas> JugadorEstadisticas { get; } = new ObservableCollection<JugadorEstadisticas>();
        }

        private void BORRAME_SImulacionCambioRonda(object sender, RoutedEventArgs e)
        {
            CambiarPantallaCallback(PANTALLA_TODOS_CARTAS);
        }
        private void BORRAME_SImulacionCambioRondaSoyJugador(object sender, RoutedEventArgs e)
        {
            NotificarNarradorCallback(false);

        }

        private void BORRAME_SImulacionCambioRondaSoyNarrador(object sender, RoutedEventArgs e)
        {
            NotificarNarradorCallback(true);

        }

        private async void BORRAME_SImulacionSolicitarImagen(object sender, RoutedEventArgs e)
        {
            await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);

        }

        
    }
}
