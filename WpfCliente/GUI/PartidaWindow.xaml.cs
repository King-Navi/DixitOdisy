using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class PartidaWindow : Window, IHabilitadorBotones, IServicioPartidaSesionCallback, INotifyPropertyChanged
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
        private const int CONTADOR_SELECCION_CERO = 0; 
        private const int MAXIMO_IMAGENES_MAZO = 6; 
        private const int CERO_IMAGENES_MAZO = 0; 
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
        SeleccionCartaUserControl seleccionCartasUserControl;
        NarradorSeleccionCartaUserControl narradorSeleccionCartasUserControl;
        VerTodasCartasUserControl verTodasCartasUserControl;
        ResumenRondaUserControl resumenRondaUserControl;
        private readonly SemaphoreSlim semaphoreRecibirImagenCallback = new SemaphoreSlim(1,1);
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
            InitializeComponent();
            InicializarComponenetes();
            DataContext = this;
            UnirsePartida(idPartida);
        }



        private async Task SolicitarMazoAsync()
        {
            var tareasSolicitudes = new List<Task>();
            for (int i = CERO_IMAGENES_MAZO; i < MAXIMO_IMAGENES_MAZO; i++)
            {
                tareasSolicitudes.Add(Conexion.Partida.SolicitarImagenCartaAsync(
                    SingletonCliente.Instance.NombreUsuario, 
                    SingletonCliente.Instance.IdPartida));
            }
            await Task.WhenAll(tareasSolicitudes);
        }

        private void InicializarComponenetes()
        {
            recursosCompartidos = new RecursosCompartidosPartida();
            ComandoImagenGlobal = new ComandoRele<string>(ComandoImagenPorId);
            ComandoImagenSelecionCorrecta = new ComandoRele<string>(
                ComandoSeleccionCorrecta,
                (param) => ComandoHabilitado
            );
            narradorSeleccionCartasUserControl = new NarradorSeleccionCartaUserControl(recursosCompartidos.Imagenes);
            seleccionCartasUserControl = new SeleccionCartaUserControl(recursosCompartidos.Imagenes);
            verTodasCartasUserControl = new VerTodasCartasUserControl(recursosCompartidos.GruposDeImagenes);
            resumenRondaUserControl = new ResumenRondaUserControl(recursosCompartidos.UsuarioEnpartida, 
                recursosCompartidos.Podio);
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
            await Conexion.ChatMotor.AgregarUsuarioChatAsync(SingletonCliente.Instance.IdChat, 
                SingletonCliente.Instance.NombreUsuario);
            chatUserControl.IsEnabled = true;
        }

        private async void ComandoSeleccionCorrecta(string idImagen)
        {
            ImagenCarta imagenAEscoger = recursosCompartidos.GruposDeImagenes.FirstOrDefault(busqueda => busqueda.IdImagen == idImagen);
            if (imagenAEscoger == null)
                return;
            MostrarCartaModalWindow ventanaModal = new MostrarCartaModalWindow(false, imagenAEscoger.BitmapImagen);
            bool? resultado = ventanaModal.ShowDialog();
            string pista = ventanaModal.Pista;
            if ((bool)resultado)
            {
                contadorSeleccionAdivinar++;
                await Conexion.VerificarConexion(HabilitarBotones, this);
                await Conexion.Partida.TratarAdivinarAsync(SingletonCliente.Instance.NombreUsuario, SingletonCliente.Instance.IdPartida, idImagen);
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
            recursosCompartidos.EliminarUsuarioSala(jugardoreRetiradoDeSala);
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
        }

        public void EnviarEstadisticas(EstadisticasPartida estadisticas)
        {
            recursosCompartidos.JugadorEstadisticas = new ObservableCollection<JugadorEstadisticas>(estadisticas.Jugadores);
            recursosCompartidos.AsignarPodio(estadisticas.PrimerLugar, estadisticas.SegundoLugar, estadisticas.TercerLugar);
            resumenRondaUserControl.MostrarEnPodio(recursosCompartidos.primerLuagr, recursosCompartidos.segundoLugar, recursosCompartidos.tercerLugar);

        }

        public async void IniciarValoresPartidaCallback(bool seUnio)
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
             {
                await SolicitarMazoAsync();
                Conexion.Partida.EmpezarPartida(SingletonCliente.Instance.NombreUsuario, SingletonCliente.Instance.IdPartida);
                await UnirseChat();
             });
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
                    VentanasEmergentes.CrearVentanaEmergenteConCierre(Properties.Idioma.tituloErrorInesperado, 
                        Properties.Idioma.mensajeErrorInesperado,
                        this);
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
                Conexion.Partida.UnirsePartida(SingletonCliente.Instance.NombreUsuario, idPartida);
            }
            catch (Exception)
            {
                NoHayConexion();
            }
        }



        public void HabilitarBotones(bool esHabilitado)
        {
            seleccionCartasUserControl.IsEnabled = esHabilitado;
            resumenRondaUserControl.IsEnabled = esHabilitado;
            narradorSeleccionCartasUserControl.IsEnabled = esHabilitado;
            verTodasCartasUserControl.IsEnabled = esHabilitado;
        }

        private void NoHayConexion()
        {
            this.Close();
        }


        public void FinalizarPartida()
        {
            AvanzarPantalla(PANTALLA_FIN_PARTIDA);
        }

        private void CerrandoVentana(object sender, CancelEventArgs e)
        {
            try
            {
                Conexion.CerrarChatMotor();
                Conexion.CerrarConexionesPartida();
            }
            catch (Exception)
            {
            }
        }

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

        public async void EscogerImagenPorId(string id)
        {
            ImagenCarta imagenEscogida = recursosCompartidos.Imagenes.FirstOrDefault(i => i.IdImagen == id);
            if (imagenEscogida == null)
                return;
            string claveImagen = imagenEscogida.IdImagen;
            MostrarCartaModalWindow ventanaModal = new MostrarCartaModalWindow(false, imagenEscogida.BitmapImagen);
            bool? resultado = ventanaModal.ShowDialog();
            if ((bool)resultado)
            {
                contadorSeleccion++;
                recursosCompartidos.Imagenes.Remove(imagenEscogida);

                    await Conexion.VerificarConexion(HabilitarBotones, this);
                    Conexion.Partida.ConfirmarMovimiento(SingletonCliente.Instance.NombreUsuario,
                                                                            SingletonCliente.Instance.IdPartida,
                                                                            imagenEscogida.IdImagen,
                                                                            null);
                    await Conexion.Partida.SolicitarImagenCartaAsync(SingletonCliente.Instance.NombreUsuario, SingletonCliente.Instance.IdPartida);
                
                if (contadorSeleccion >= SELECCION_MAXIMA_JUGADOR)
                {
                    AvanzarPantalla(PANTALLA_ESPERA);
                    contadorSeleccion = CONTADOR_SELECCION_CERO;
                }

            }

        }

        public async void EscogerImagenNarrador(string id)
        {
            
            ImagenCarta imagenAEscoger = recursosCompartidos.Imagenes.FirstOrDefault(i => i.IdImagen == id);
            if (imagenAEscoger == null)
                return;
            MostrarCartaModalWindow ventanaModal = new MostrarCartaModalWindow(true, imagenAEscoger.BitmapImagen);
            bool? resultado = ventanaModal.ShowDialog();
            string pista = ventanaModal.Pista;
            if ((bool)resultado)
            {
                contadorSeleccion++;
                recursosCompartidos.Imagenes.Remove(imagenAEscoger);
                await Conexion.VerificarConexion(HabilitarBotones, this);
                await Conexion.Partida.ConfirmarMovimientoAsync(SingletonCliente.Instance.NombreUsuario,
                                                                            SingletonCliente.Instance.IdPartida,
                                                                            imagenAEscoger.IdImagen,
                                                                            pista);
                 await Conexion.Partida.SolicitarImagenCartaAsync(SingletonCliente.Instance.NombreUsuario, SingletonCliente.Instance.IdPartida);
                if (contadorSeleccion >= SELECCION_MAXIMA_NARRADOR)
                {
                    AvanzarPantalla(PANTALLA_ESPERA);
                    contadorSeleccion = CONTADOR_SELECCION_CERO;
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
            await Conexion.Partida.SolicitarImagenCartaAsync(SingletonCliente.Instance.NombreUsuario, SingletonCliente.Instance.IdPartida);

        }

        
    }
}
