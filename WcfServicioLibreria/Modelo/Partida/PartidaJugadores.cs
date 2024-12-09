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
using System.Security.Policy;

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
        private const int CANTIDAD_MINIMA_JUGADORES = 3;
        private const int TIEMPO_ESPERA_UNIRSE_JUGADORES = 25; 
        private const int TIEMPO_ESPERA_NARRADOR = 30;  
        private const int TIEMPO_ESPERA_SELECCION = 25;  
        private const int TIEMPO_ESPERA_PARA_ADIVINAR = 25;
        private const int TIEMPO_ESPERA = 5;
        private const int NUMERO_JUGADOR_PARTIDA_VACIA = 0;
        private const int RONDAS_MINIMA_PARA_PUNTOS = 3;
        private const int NUMERO_MINIMO_RONDAS = 3;
        private const int TIEMPO_MOSTRAR_ESTADISTICAS = 15;
        private const int ID_INVALIDO = 0;
        private const int ID_INVALIDO_ESTADISTICAS = 0;

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
        private static readonly Random aleatorio = new Random();
        private static readonly SemaphoreSlim semaphoreEscogerNarrador = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim semaphoreEmpezarPartida = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim semaphoreAgregarJugador = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim semaphoreRemoverJugador = new SemaphoreSlim(1, 1);
        public EventHandler PartidaVaciaManejadorEvento;
        private event EventHandler TodosListos;
        private readonly IEstadisticasDAO estadisticasDAO;
        public readonly IMediadorImagen mediadorImagen;
        public readonly TematicaPartida tematica;


        #endregion Atributos

        #region Propiedad
        public string IdPartida { get; private set; }
        public string Anfitrion { get; private set; }
        public string NarradorActual { get; private set; }
        public string ClaveImagenCorrectaActual { get; private set; }
        public string PistaActual { get; private set; }
        public int RondaActual { get; private set; }
        public bool SeLlamoEmpezarPartida { get; private set; } = false;
        public bool SeLlamoEmpezarPartidaPrimeraVez { get; private set; } = false;
        public bool SelecionoCartaNarrador { get; private set; } = false;
        public bool TiempoUnirse { get; private set; } = true;
        public bool DebeCancelarRondas { get; set; } = false;
        public ConcurrentDictionary<string, bool> JugadoresPendientes { get; private set; } = new ConcurrentDictionary<string, bool>();
        public ConcurrentDictionary<string, List<string>> ImagenesTablero { get; private set; } = new ConcurrentDictionary<string, List<string>>();
        private ConcurrentDictionary<string, List<string>> ImagenElegidaPorJugador { get; set; } = new ConcurrentDictionary<string, List<string>>();

        #endregion Propiedad

        #region Contructor
        public Partida(
            string _idPartida,
            string _anfitrion,
            ConfiguracionPartida _configuracion,
            IEstadisticasDAO _estadisticasDAO,
            IMediadorImagen _mediadorImagen)
        {
            IdPartida = _idPartida;
            Anfitrion = _anfitrion;
            tematica = _configuracion.Tematica;
            condicionVictoria = CrearCondicionVictoria(_configuracion);
            ImagenElegidaPorJugador = new ConcurrentDictionary<string, List<string>>();
            RondaActual = RONDA_INICIAL;
            estadisticasPartida = new EstadisticasPartida(_configuracion.Tematica);
            TodosListos += (emisor, evento) =>
            {
                TiempoUnirse = false;
                Task.Run(async () => await IniciarPartidaSeguroAsync());
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
                foreach (var callback in jugadoresCallback.Values)
                {
                    try
                    {
                        if (callback is ICommunicationObject communicationObject)
                        {
                            communicationObject.Close();
                        }
                    }
                    catch (Exception excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionError(excepcion);
                    }
                }
                jugadoresCallback.Clear();
                foreach (var evento in eventosCommunication.Values)
                {
                    try
                    {
                        evento.Desechar();
                    }
                    catch (Exception excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
            await semaphoreAgregarJugador.WaitAsync();
            try
            {
                if (!TiempoUnirse)
                {
                    throw new FaultException<PartidaFalla>(new PartidaFalla());
                }
                jugadoresCallback.AddOrUpdate(nombreJugador, nuevoContexto, (key, oldValue) => nuevoContexto);
                if (jugadoresCallback.TryGetValue(nombreJugador, out IPartidaCallback contextoCambiado))
                {
                    eventosCommunication.TryAdd(nombreJugador, new DesconectorEventoManejador((ICommunicationObject)contextoCambiado, this, nombreJugador));
                    await AvisarNuevoJugador(nombreJugador);
                    await ConfirmarInclusionPartidaAsync(nuevoContexto);
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


        internal async Task AvisarNuevoJugador(string nombreJugadorConectandose)
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
                    await EnviarJugadoresExistentesAlNuevoAsync(nuevoCallback);
                    await NotificarSobreNuevoJugadorAsync(nombreJugadorConectandose, usuario);
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
                throw;
            }
        }

        private async Task EnviarJugadoresExistentesAlNuevoAsync(IPartidaCallback nuevoCallback)
        {
            try
            {
                var jugadoresCopia = jugadoresInformacion.Values.ToList();
                foreach (var jugadorExistente in jugadoresCopia)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            Usuario jugador = new Usuario
                            {
                                Nombre = jugadorExistente.gamertag,
                                FotoUsuario = null
                            };

                            using (var fotoUsuarioExistente = new MemoryStream(jugadorExistente.fotoPerfil))
                            {
                                jugador.FotoUsuario = fotoUsuarioExistente;
                                nuevoCallback?.ObtenerJugadorPartidaCallback(jugador);
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task NotificarSobreNuevoJugadorAsync(string nombreJugadorConectandose, Usuario usuario)
        {
            try
            {
                var jugadoresConectados = ObtenerNombresJugadores();
                var tareas = jugadoresConectados
                    .Where(jugadorConectado => jugadorConectado != nombreJugadorConectandose)
                    .Select(jugadorConectado => Task.Run(() =>
                    {
                        try
                        {
                            if (jugadoresCallback.TryGetValue(jugadorConectado, out IPartidaCallback callback))
                            {
                                using (var fotoUsuarioNuevo = new MemoryStream(jugadoresInformacion[nombreJugadorConectandose].fotoPerfil))
                                {
                                    usuario.FotoUsuario = fotoUsuarioNuevo;
                                    callback?.ObtenerJugadorPartidaCallback(usuario);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }))
                    .ToList();
                await Task.WhenAll(tareas);
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal async Task ConfirmarInclusionPartidaAsync(IPartidaCallback contexto)
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        contexto?.IniciarValoresPartidaCallback(true);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                });
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
                throw;
            }
        }

        private async Task AvisarRetiroJugadorAsync(string nombreUsuarioEliminado)
        {
            if (jugadoresInformacion.TryRemove(nombreUsuarioEliminado, out DAOLibreria.ModeloBD.Usuario usuarioEliminado) && usuarioEliminado != null)
            {
                Usuario usuario = new Usuario
                {
                    Nombre = usuarioEliminado.gamertag
                };
                await NotificarRetiroJugadorAsync(usuario);
            }
        }

        private async Task NotificarRetiroJugadorAsync(Usuario usuario)
        {
            var nombresJugadores = ObtenerNombresJugadores();

            foreach (var nombreJugador in nombresJugadores)
            {
                if (jugadoresCallback.TryGetValue(nombreJugador, out IPartidaCallback callback) && callback != null)
                {
                    await EnviarEliminacionJugadorCallbackAsync(callback, usuario);
                }
            }
        }

        private async Task EnviarEliminacionJugadorCallbackAsync(IPartidaCallback callback, Usuario usuario)
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (((ICommunicationObject)callback).State == CommunicationState.Opened)
                        {
                            callback.EliminarJugadorPartidaCallback(usuario);
                        }
                    }
                    catch (CommunicationException)
                    {
                        throw ;
                    }
                    catch (Exception )
                    {
                        throw ;
                    }
                });
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }

        public async Task DesconectarUsuarioAsync(string nombreJugador)
        {
            await AvisarRetiroJugadorAsync(nombreJugador);
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
                if (ContarJugadores() == NUMERO_JUGADOR_PARTIDA_VACIA)
                {
                    DebeCancelarRondas = true;
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


