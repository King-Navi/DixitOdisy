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
        private const int SELECCION_MAXIMA_NARRADOR = 1; 
        private const int SELECCION_MAXIMA_JUGADOR = 1; 
        private const int ADIVINAR_MAXIMA_JUGADOR = 1; 
        private int contadorSeleccion =  0;
        private int contadorSeleccionAdivinar =  0;
        private const int INICIALIZAR_CONTADOR =  0;
        public ICommand ComandoImagenGlobal { get; set; }
        public ICommand ComandoImagenSelecionCorrecta { get; set; }
        private bool comandoHabilitado = true;
        public bool ComandoHabilitado
        {
            get => comandoHabilitado;
            set
            {
                comandoHabilitado = value;
                (ComandoImagenSelecionCorrecta as ComandoRele<string>)?.RaiseCanExecuteChanged();
            }
        }
        private RecursosCompartidosPartida recursosCompartidos;
        public event PropertyChangedEventHandler PropertyChanged;
        SeleccionCartaUsercontrol seleccionCartasUserControl;
        NarradorSeleccionCartaUserControl narradorSeleccionCartasUserControl;
        VerTodasCartasUserControl verTodasCartasUserControl;
        ResumenRondaUserControl resumenRondaUserControl;
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

            recursosCompartidos = new RecursosCompartidosPartida();
            ComandoImagenGlobal = new ComandoRele<string>(ComandoImagenPorId);
            ComandoImagenSelecionCorrecta = new ComandoRele<string>(
                ComandoSeleccionCorrecta,
                (param) => ComandoHabilitado // La habilitación depende de la propiedad ComandoHabilitado
            );
            narradorSeleccionCartasUserControl = new NarradorSeleccionCartaUserControl(recursosCompartidos.Imagenes);
            seleccionCartasUserControl = new SeleccionCartaUsercontrol(recursosCompartidos.Imagenes);
            verTodasCartasUserControl = new VerTodasCartasUserControl(recursosCompartidos.GruposDeImagenes);
            resumenRondaUserControl = new ResumenRondaUserControl(recursosCompartidos.UsuarioEnpartida, recursosCompartidos.Podio);
            chatUserControl.IsEnabled = false;
            gridPantalla2.Children.Add(seleccionCartasUserControl);
            gridPantalla3.Children.Add(narradorSeleccionCartasUserControl);
            gridPantalla4.Children.Add(verTodasCartasUserControl);
            gridPantalla5.Children.Add(resumenRondaUserControl);
            PantallaActual = PANTALLA_INICIO;
        }

        private async Task UnirseChat()
        {
            await Conexion.AbrirConexionChatMotorCallbackAsync(chatUserControl);
            await Conexion.ChatMotor.AgregarUsuarioChatAsync(Singleton.Instance.IdChat, Singleton.Instance.NombreUsuario);
            chatUserControl.IsEnabled = true;
        }

        private async void ComandoSeleccionCorrecta(string idImagen)
        {
            ImagenCarta imagenAEscoger = recursosCompartidos.GruposDeImagenes.FirstOrDefault(i => i.IdImagen == idImagen);
            if (imagenAEscoger == null)
                return;
            MostrarCartaModelWindow ventanaModal = new MostrarCartaModelWindow(false, imagenAEscoger.BitmapImagen);
            bool? resultado = ventanaModal.ShowDialog();
            string pista = ventanaModal.Pista;
            if ((bool)resultado)
            {
                contadorSeleccionAdivinar++;
                await Conexion.VerificarConexion(HabilitarBotones, this);
                await Conexion.Partida.TratarAdivinarAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida, idImagen);
                if (contadorSeleccionAdivinar >= ADIVINAR_MAXIMA_JUGADOR)
                {
                    AvanzarPantalla(PANTALLA_ESPERA);
                    contadorSeleccionAdivinar = INICIALIZAR_CONTADOR ;
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
            if (esNarrador && numeroPantalla == PANTALLA_TODOS_CARTAS)
            {
                comandoHabilitado = false;
            }
            else
            {
                comandoHabilitado = true;
            }
            if (numeroPantalla == PANTALLA_INICIO)
            {
                recursosCompartidos.GruposDeImagenes.Clear();
                MostrarPistaCallback(null);
            }
            PantallaActual = numeroPantalla;
        }

        public void ObtenerJugadorPartidaCallback(Usuario jugardoreNuevoEnSala)
        {
            recursosCompartidos.ObtenerUsuarioSala(jugardoreNuevoEnSala);
        }

        public void EliminarJugadorPartidaCallback(Usuario jugardoreRetiradoDeSala)
        {
            recursosCompartidos.ObtenerUsuarioSala(jugardoreRetiradoDeSala);
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
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                recursosCompartidos.Imagenes.Add(imagen);
            }
            else
            {
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
            if (String.IsNullOrEmpty(pista))
            {
                seleccionCartasUserControl.ColocarPista(Properties.Idioma.labelEsperandoPista);
                return;
            }

            seleccionCartasUserControl.ColocarPista(pista);
            if (EsNarrador)
            {
                AvanzarPantalla(PANTALLA_ESPERA);
            }
            else
            {
                //Avanzar con el flujo de jugador no narrador
            }
        }

        public void EnviarEstadisticas(EstadisticasPartida estadisticas)
        {
            recursosCompartidos.JugadorEstadisticas = new ObservableCollection<JugadorEstadisticas>(estadisticas.Jugadores);
            recursosCompartidos.AsignarPodio(estadisticas.PrimerLugar, estadisticas.SegundoLugar, estadisticas.TercerLugar);
            resumenRondaUserControl.MostrarEnPodio(recursosCompartidos.primerLuagr, recursosCompartidos.segundoLugar, recursosCompartidos.tercerLugar);

        }

        public async void IniciarValoresPartidaCallback(bool seUnio)
        {
            await SolicitarMazoAsync();
            Conexion.Partida.EmpezarPartida(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
            EsconderVentanaMenu();
            await UnirseChat();
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
            try
            {
                bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
                if (!conexionExitosa)
                {
                    return;
                }
                if (!ValidacionExistenciaJuego.ExistePartida(idPartida))
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
            catch (Exception)
            {
                NoHayConexion();
            }
        }

        public void ActualizarUI()
        {
            //TODO: recursos .resx
        }



        public void HabilitarBotones(bool esVasible)
        {
            seleccionCartasUserControl.IsEnabled = esVasible;
            resumenRondaUserControl.IsEnabled = esVasible;
            narradorSeleccionCartasUserControl.IsEnabled = esVasible;
            verTodasCartasUserControl.IsEnabled = esVasible;
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
        public async void EscogerImagenPorId(string id)
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

                    await Conexion.VerificarConexion(HabilitarBotones, this);
                    Conexion.Partida.ConfirmarMovimiento(Singleton.Instance.NombreUsuario,
                                                                            Singleton.Instance.IdPartida,
                                                                            imagenEscogida.IdImagen,
                                                                            null);
                    await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
                
                if (contadorSeleccion >= SELECCION_MAXIMA_JUGADOR)
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
        public async void EscogerImagenNarrador(string id)
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
                await Conexion.VerificarConexion(HabilitarBotones, this);
                await Conexion.Partida.ConfirmarMovimientoAsync(Singleton.Instance.NombreUsuario,
                                                                            Singleton.Instance.IdPartida,
                                                                            imagenAEscoger.IdImagen,
                                                                            pista);
                 await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
                
                if (contadorSeleccion >= SELECCION_MAXIMA_NARRADOR)
                {
                    AvanzarPantalla(PANTALLA_ESPERA);
                    contadorSeleccion = 0;
                }

            }
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
