using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        public ICommand ComandoImagenGlobal { get; set; }
        private RecursosCompartidos imagenesCompartidas;
        public event PropertyChangedEventHandler PropertyChanged;
        SeleccionCartaUsercontrol seleccionCartasUserControl;
        NarradorSeleccionCartaUserControl narradorSeleccionCartasUserControl;
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
                    AvanzarPantalla(3);
                }
                else
                {
                    AvanzarPantalla(2);
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
            ActualizarUI();
            DataContext = this;
            UnirsePartida(idPartida);

        }

        private async Task SolicitarMazoAsync()
        {
            await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
            //await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
            //await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
            //await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
            //await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
            //await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
            //var tareasSolicitudes = new List<Task>();
            //for (int i = 0; i < 6; i++)
            //{
            //    tareasSolicitudes.Add(Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida));
            //}
            //await Task.WhenAll(tareasSolicitudes);
        }

        private void InicializarComponenetes()
        {

            imagenesCompartidas = new RecursosCompartidos();
            ComandoImagenGlobal = new ComandoRele<string>(ComandoImagenPorId);
            narradorSeleccionCartasUserControl = new NarradorSeleccionCartaUserControl(imagenesCompartidas.Imagenes);
            seleccionCartasUserControl = new SeleccionCartaUsercontrol(imagenesCompartidas.Imagenes);
            gridPantalla2.Children.Add(seleccionCartasUserControl);
            gridPantalla3.Children.Add(narradorSeleccionCartasUserControl);
            PantallaActual = 1;


        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IServicioPartidaSesionCallback

        public void CambiarPantallaCallback(int numeroPantalla)
        {
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
                imagenesCompartidas.Imagenes.Add(imagen);
            }
            else
            {
                // Si no está en el hilo de la UI, usa el Dispatcher para invocar en el hilo de la UI
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    imagenesCompartidas.Imagenes.Add(imagen);
                });
            }
            //
        }

        /// <summary>
        /// Notifica si eres el narrador
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void NotificarNarradorCallback(bool esNarrador)
        {
            EsNarrador = esNarrador;
        }

        public void MostrarPistaCallback(string pista)
        {
           
            seleccionCartasUserControl.ColocarPista(pista);
            if (EsNarrador)
            {
                AvanzarPantalla(6);
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
        }

        #endregion IServicioPartidaSesionCallback

        /// <summary>
        /// InicioRonda = 1
        /// 
        /// 
        /// Escoger carta jugador = 2
        /// EscogerCataNarrador = 3
        /// Fin partida = 5
        /// </summary>
        /// <param name="numeroPantallla"></param>
        public void AvanzarPantalla(int numeroPantallla) //Fixme o algo por el estilo
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
            AvanzarPantalla(5);
        }



        private void CerrandoVentana(object sender, CancelEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
            try
            {
                Conexion.CerrarChatMotor();
                Conexion.CerrarConexionesPartida();
            }
            catch (Exception excepcion)
            {
                //TODO Manejar excepcion
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
            AvanzarPantalla(6);
        }
        /// <summary>
        /// Envia la imagen escoigda en base a la pista por el Jugador al servidor
        /// </summary>
        /// <param name="id"></param>
        public void EscogerImagenPorId(string id)
        {
            ImagenCarta imagenEscogida = imagenesCompartidas.Imagenes.FirstOrDefault(i => i.IdImagen == id);
            if (imagenEscogida == null)
                return;
            string claveImagen = imagenEscogida.IdImagen;
            MostrarCartaModelWindow ventanaModal = new MostrarCartaModelWindow(false, imagenEscogida.BitmapImagen);
            bool? resultado = ventanaModal.ShowDialog();
            if ((bool)resultado)
            {

                imagenesCompartidas.Imagenes.Remove(imagenEscogida);
                Task.Run(async () =>
                {
                    await Conexion.VerificarConexion(HabilitarBotones, this);
                    Conexion.Partida.ConfirmarMovimiento(Singleton.Instance.NombreUsuario,
                                                                            Singleton.Instance.IdPartida,
                                                                            imagenEscogida.IdImagen,
                                                                            null);
                    await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
                });
            }
            else
            {
                MessageBox.Show("Debes selecionar una imagen");
            }
        }
        /// <summary>
        /// Envia la imagen y pista escoigda por el narrador al servidor
        /// </summary>
        /// <param name="id"></param>
        public void EscogerImagenNarrador(string id)
        {
            ImagenCarta imagenAEscoger = imagenesCompartidas.Imagenes.FirstOrDefault(i => i.IdImagen == id);
            if (imagenAEscoger == null)
                return;
            MostrarCartaModelWindow ventanaModal = new MostrarCartaModelWindow(true, imagenAEscoger.BitmapImagen);
            bool? resultado = ventanaModal.ShowDialog();
            string pista = ventanaModal.Pista;
            if ((bool)resultado)
            {
                imagenesCompartidas.Imagenes.Remove(imagenAEscoger);
                Task.Run(async () =>
                {
                    await Conexion.VerificarConexion(HabilitarBotones, this);
                    Conexion.Partida.ConfirmarMovimiento(Singleton.Instance.NombreUsuario,
                                                                            Singleton.Instance.IdPartida,
                                                                            imagenAEscoger.IdImagen,
                                                                            pista);
                    await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
                });
            }
            else
            {
                MessageBox.Show("Debes selecionar una imagen");
            }
        }


        private class RecursosCompartidos
        {
            public ObservableCollection<ImagenCarta> Imagenes { get; } = new ObservableCollection<ImagenCarta>();
            public ObservableCollection<JugadorEstadisticas> JugadorEstadisticas { get; } = new ObservableCollection<JugadorEstadisticas>();

        }

        private void BORRAME_SImulacionCambioRonda(object sender, RoutedEventArgs e)
        {
            AvanzarRondaCallback(pantallaActual + 1);
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
