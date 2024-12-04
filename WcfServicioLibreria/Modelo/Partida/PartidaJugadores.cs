using System.Collections.Concurrent;
using System;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Enumerador;
using System.ServiceModel;
using WcfServicioLibreria.Utilidades;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using DAOLibreria.Interfaces;
using DAOLibreria.DAO;
using WcfServicioLibreria.Modelo.Excepciones;
using WcfServicioLibreria.Modelo.Evento;

namespace WcfServicioLibreria.Modelo
{
    internal partial class Partida : IObservador
    {
        #region Constantes
        #region PantallasCliente
        public const int PANTALLA_INICIO = 1;
        public const int PANTALLA_NARRADOR_SELECION = 2;
        public const int PANTALLA_JUGADOR_SELECION = 3;
        public const int PANTALLA_TODOS_CARTAS = 4;
        public const int PANTALLA_ESTADISTICAS = 5;
        public const int PANTALLA_FIN_PARTIDA = 6;
        public const int PANTALLA_ESPERA = 7;
        #endregion PantallasCliente

        #region NumerosPartida
        private const int CANTIDAD_MINIMA_JUGADORES = 1; // 2
        private const int TIEMPO_ESPERA_UNIRSE_JUGADORES = 20;
        private const int TIEMPO_ESPERA_NARRADOR = 40; // 40
        private const int TIEMPO_ESPERA_SELECCION = 20; //60
        private const int TIEMPO_ESPERA_PARA_ADIVINAR = 10; //60
        private const int TIEMPO_ESPERA = 5;
        private const int TIEMPO_ENVIO_SEGUNDOS = 5;
        private const int NUM_JUGADOR_PARTIDA_VACIA = 0; 
        private const int NUM_JUGADOR_NADIE_ACERTO = 0;
        private const int RONDAS_MINIMA_PARA_PUNTOS = 3;
        private const int NUMERO_MINIMO_RONDAS = 3;
        private const int PUNTOS_RESTADOS_NO_PARTICIPAR = 1;
        private const int PUNTOS_ACIERTO = 1;
        private const int PUNTOS_PENALIZACION_NARRADOR = 2;
        private const int PUNTOS_MAXIMOS_RECIBIDOS_CONFUNDIR = 3;
        private const int TIEMPO_MOSTRAR_ESTADISTICAS = 15;
        private const int ID_INVALIDO = 0;
        private const int TIEMPO_ESPERA_MILISEGUNDOS = 1000;

        private const int RONDA_INICIAL = 0;
        #endregion NumerosPartida
        #endregion Constantes

        #region Atributos
        private readonly ConcurrentDictionary<string, IPartidaCallback> jugadoresCallback = new ConcurrentDictionary<string, IPartidaCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        private readonly ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario> jugadoresInformacion = new ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario>();
        private readonly ICondicionVictoria condicionVictoria;
        public ConfiguracionPartida Configuracion { get; private set; }
        private readonly EstadisticasPartida estadisticasPartida;
        private CancellationTokenSource cancelacionEjecucionRonda;
        private static readonly Random aleatorio = new Random();
        private static readonly SemaphoreSlim semaphoreEscogerNarrador = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim semaphoreEmpezarPartida = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim semaphoreAgregarJugador = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim semaphoreRemoverJugador = new SemaphoreSlim(1, 1);
        public EventHandler PartidaVaciaManejadorEvento;
        private event EventHandler TodosListos;
        private readonly IEstadisticasDAO estadisticasDAO;
        public readonly IMediadorImagen mediadorImagen;
        public event EventHandler<RondaEventArgs> MostrarTodasLasCartas;
        private static int tiempoEspera = 0;
        private static readonly int tiempoMinimo = 16;
        private static readonly object lockTiempoEspera = new object();

        #endregion Atributos

        #region Propiedad
        public string IdPartida { get; private set; }
        public string Anfitrion { get; private set; }
        public string NarradorActual { get; private set; }
        public string ClaveImagenCorrectaActual { get; private set; }
        public string PistaActual { get; private set; }
        public int RondaActual { get; private set; }
        public bool SeLlamoEmpezarPartida { get; private set; } = false;
        public bool SeTerminoEsperaUnirse { get; private set; } = false;
        public bool SelecionoCartaNarrador { get; private set; } = false;
        public ConcurrentBag<string> JugadoresPendientes { get; private set; }
        private ConcurrentDictionary<string, List<string>> ImagenPuestasPisina { get; set; } = new ConcurrentDictionary<string, List<string>>();
        private ConcurrentDictionary<string, List<string>> ImagenElegidaPorJugador { get; set; } = new ConcurrentDictionary<string, List<string>>();

        #endregion Propiedad

        #region Contructor
        public Partida(string _idPartida, string _anfitrion, ConfiguracionPartida _configuracion, IEstadisticasDAO _estadisticasDAO, IMediadorImagen _mediadorImagen)
        {
            IdPartida = _idPartida;
            Anfitrion = _anfitrion;
            condicionVictoria = CrearCondicionVictoria(_configuracion);
            JugadoresPendientes = new ConcurrentBag<string>();
            ImagenElegidaPorJugador = new ConcurrentDictionary<string, List<string>>();
            RondaActual = RONDA_INICIAL;
            estadisticasPartida = new EstadisticasPartida(_configuracion.Tematica);
            TodosListos += (emisor, evento) =>
            {
                cancelacionEjecucionRonda = new CancellationTokenSource();
                Task.Run(async () => await IniciarPartidaSeguroAsync(cancelacionEjecucionRonda.Token));
            };
            estadisticasDAO = _estadisticasDAO;
            mediadorImagen = _mediadorImagen;
        }
        #endregion Contructor

        #region Metodos

        #region ManejarEstadoPartida
        private void EnPartidaVacia()
        {
            PartidaVaciaManejadorEvento?.Invoke(this, new PartidaVaciaEventArgs(DateTime.Now, this));
        }

        private void EliminarPartida()
        {
            try
            {
                IReadOnlyCollection<string> claveJugadores = ObtenerNombresJugadores();
                foreach (var clave in claveJugadores)
                {
                    if (jugadoresCallback.ContainsKey(clave))
                    {
                        ((ICommunicationObject)jugadoresCallback[clave]).Close();
                    }
                }
                jugadoresCallback.Clear();
                IReadOnlyCollection<string> claveEventos = ObtenerNombresJugadores();
                foreach (var clave in claveEventos)
                {
                    if (eventosCommunication.ContainsKey(clave))
                    {
                        eventosCommunication[clave].Desechar();
                    }
                }
                eventosCommunication.Clear();
                EnPartidaVacia();
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }

        #endregion ManejarEstadoPartida

        #region InicializarPartida
        private ICondicionVictoria CrearCondicionVictoria(ConfiguracionPartida condicionVictoria)
        {
            switch (condicionVictoria.Condicion)
            {
                case CondicionVictoriaPartida.PorCantidadRondas:
                    if (condicionVictoria.NumeroRondas < NUMERO_MINIMO_RONDAS)
                    {
                        return new CondicionVictoriaPorRondas(NUMERO_MINIMO_RONDAS);
                    }
                    else
                    {
                        return new CondicionVictoriaPorRondas(condicionVictoria.NumeroRondas);
                    }
                case CondicionVictoriaPartida.PorCartasAgotadas:
                    return new CondicionVictoriaCartasAgotadas();
                default:
                    return new CondicionVictoriaCartasAgotadas();
            }
        }


        #endregion InicializarPartida

        #region ManejoJugadores

        public async Task<bool> AgregarJugadorAsync(string nombreJugador, IPartidaCallback nuevoContexto)
        {
            int esperaActual;
            lock (lockTiempoEspera)
            {
                esperaActual = tiempoEspera;
                if (tiempoEspera > tiempoMinimo)
                {
                    tiempoEspera += 4;
                }
            }
            await Task.Delay(esperaActual * TIEMPO_ESPERA_MILISEGUNDOS);

            await semaphoreAgregarJugador.WaitAsync();
            try
            {
                jugadoresCallback.AddOrUpdate(nombreJugador, nuevoContexto, (key, oldValue) => nuevoContexto);

                if (jugadoresCallback.TryGetValue(nombreJugador, out IPartidaCallback contextoCambiado) &&
                    ReferenceEquals(nuevoContexto, contextoCambiado))
                {
                    eventosCommunication.TryAdd(nombreJugador, new DesconectorEventoManejador((ICommunicationObject)contextoCambiado, this, nombreJugador));
                    AvisarNuevoJugador(nombreJugador);
                    ConfirmarInclusionPartida(nuevoContexto);
                    return true;
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
                throw new FaultException<PartidaFalla>(new PartidaFalla()
                {
                    ErrorAlUnirse = true
                });
            }
            finally
            {
                semaphoreAgregarJugador.Release();
            }

            return false;
        }


        internal void AvisarNuevoJugador(string nombreJugadorConectandose)
        {


            try
            {
                UsuarioDAO consulta = new UsuarioDAO();
                DAOLibreria.ModeloBD.Usuario informacionUsuarioNuevo = consulta.ObtenerUsuarioPorNombre(nombreJugadorConectandose);
                bool esInvitado = false;

                if (informacionUsuarioNuevo == null)
                {
                    string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Rutas.RUTA_RECURSOS, Rutas.CARPETA_FOTOS_INVITADOS);
                    var archivos = Directory.GetFiles(ruta, Rutas.EXTENSION_TODO_ARCHIVO_PNG);
                    string archivoAleatorio = archivos[aleatorio.Next(archivos.Length)];
                    string nombreSinExtension = Path.GetFileNameWithoutExtension(archivoAleatorio);
                    esInvitado = true;
                    informacionUsuarioNuevo = new DAOLibreria.ModeloBD.Usuario
                    {
                        gamertag = nombreJugadorConectandose,
                        fotoPerfil = File.ReadAllBytes(archivoAleatorio)
                    };
                }

                jugadoresInformacion.TryAdd(nombreJugadorConectandose, informacionUsuarioNuevo);
                Usuario usuario = new Usuario
                {
                    Nombre = informacionUsuarioNuevo.gamertag,
                    FotoUsuario = null,
                    EsInvitado = esInvitado
                };

                if (jugadoresCallback.TryGetValue(nombreJugadorConectandose, out IPartidaCallback nuevoCallback))
                {
                    var jugadoresCopia = jugadoresInformacion.Values.ToList();
                    foreach (var jugadorExistente in jugadoresCopia)
                    {
                        Usuario jugador = new Usuario
                        {
                            Nombre = jugadorExistente.gamertag,
                            FotoUsuario = null
                        };

                        using (var fotoUsuarioExistente = new MemoryStream(jugadorExistente.fotoPerfil))
                        {
                            jugador.FotoUsuario = fotoUsuarioExistente;
                            nuevoCallback.ObtenerJugadorPartidaCallback(jugador);
                        }
                    }

                    foreach (var jugadorConectado in ObtenerNombresJugadores())
                    {
                        if (jugadoresCallback.TryGetValue(jugadorConectado, out IPartidaCallback callback))
                        {
                            if (jugadorConectado != nombreJugadorConectandose)
                            {
                                using (var fotoUsuarioNuevo = new MemoryStream(informacionUsuarioNuevo.fotoPerfil))
                                {
                                    usuario.FotoUsuario = fotoUsuarioNuevo;
                                    callback.ObtenerJugadorPartidaCallback(usuario);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
                throw;
            }
        }

        internal void ConfirmarInclusionPartida(IPartidaCallback contexto)
        {
            try
            {
                contexto.IniciarValoresPartidaCallback(true);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
                throw;
            }
        }



        private void AvisarRetiroJugador(string nombreUsuarioEliminado)
        {

            jugadoresInformacion.TryRemove(nombreUsuarioEliminado, out DAOLibreria.ModeloBD.Usuario usuarioEliminado);
            if (usuarioEliminado != null)
            {
                Usuario usuario = new Usuario
                {
                    Nombre = usuarioEliminado.gamertag
                };
                foreach (var nombreJugador in ObtenerNombresJugadores())
                {
                    jugadoresCallback.TryGetValue(nombreJugador, out IPartidaCallback callback);
                    if (callback != null)
                    {
                        try
                        {
                            callback.EliminarJugadorPartidaCallback(usuario);

                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        public async void DesconectarUsuario(string nombreJugador)
        {
            AvisarRetiroJugador(nombreJugador);
            await RemoverJugadorAsync(nombreJugador);
        }

        private async Task RemoverJugadorAsync(string nombreJugador)
        {
            await semaphoreRemoverJugador.WaitAsync();
            try
            {
                jugadoresCallback.TryRemove(nombreJugador, out IPartidaCallback _);
                eventosCommunication.TryRemove(nombreJugador, out DesconectorEventoManejador eventosJugador);
                jugadoresInformacion.TryRemove(nombreJugador, out _);
                eventosJugador?.Desechar();
                if (ContarJugadores() == NUM_JUGADOR_PARTIDA_VACIA)
                {
                    try
                    {
                        cancelacionEjecucionRonda?.Cancel();
                    }
                    catch (Exception excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionError(excepcion);
                    }
                    EliminarPartida();
                }
            }
            finally
            {
                semaphoreRemoverJugador.Release();
            }
        }
        private IReadOnlyCollection<string> ObtenerNombresJugadores()
        {
            return jugadoresCallback.Keys.ToList().AsReadOnly();
        }

        private int ContarJugadores()
        {
            return jugadoresCallback.Count;
        }

        
        #endregion ManejoJugadores

        #endregion Metodos
    }
}


