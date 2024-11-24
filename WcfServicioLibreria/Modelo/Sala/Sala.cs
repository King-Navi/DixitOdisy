using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Modelo.Vetos;
using WcfServicioLibreria.Utilidades;
namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class Sala : IObservador
    {
        #region Constantes
        private const string MOTIVO_EXPULSION_SALA="Expulsion de sala";
        #endregion
        #region Campos
        private const int SALA_VACIA = 0;
        private string idCodigoSala;
        private const int CANTIDAD_MINIMA_JUGADORES = 3;
        private const int CANTIDAD_MAXIMA_JUGADORES = 6;
        private readonly ConcurrentDictionary<string, ISalaJugadorCallback> jugadoresSalaCallbacks = new ConcurrentDictionary<string, ISalaJugadorCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        private ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario> jugadoresInformacion = new ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario>();
        private static ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random());
        public EventHandler salaVaciaManejadorEvento;
        private readonly SemaphoreSlim semaphoreLeerFotoInvitado = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim semaphoreRemoverJugador = new SemaphoreSlim(1, 1);

        private IManejadorVeto manejadorVeto;
        #endregion Campos
        #region Propiedades
public static int CantidadMaximaJugadores => CANTIDAD_MAXIMA_JUGADORES;
        public static int CantidadMinimaJugadores => CANTIDAD_MINIMA_JUGADORES;
        public string IdCodigoSala { get => idCodigoSala; internal set => idCodigoSala = value; }
        public string Anfitrion { get; private set; }

        #endregion Propiedades

        #region Contructores
        public Sala(string _idCodigoSala, string nombreUsuario)
        {
            this.IdCodigoSala = _idCodigoSala;
            this.Anfitrion = nombreUsuario;
            manejadorVeto = new ManejadorDeVetos();
        }

        #endregion Contructores

        #region Metodos
        private void EnSalaVacia()
        {
            salaVaciaManejadorEvento?.Invoke(this, new SalaVaciaEventArgs(DateTime.Now, this));
        }
        private int ContarJugadores()
        {
            return jugadoresSalaCallbacks.Count;
        }

        bool EsVacia()
        {
            return jugadoresSalaCallbacks.IsEmpty;
        }

        internal IReadOnlyCollection<string> ObtenerNombresJugadoresSala()
        {
            return jugadoresSalaCallbacks.Keys.ToList().AsReadOnly();
        }

        public bool AgregarJugadorSala(string nombreJugador, ISalaJugadorCallback nuevoContexto)
        {
            bool resultado = false;
            if (ContarJugadores() < CANTIDAD_MAXIMA_JUGADORES)
            {
                jugadoresSalaCallbacks.AddOrUpdate(nombreJugador, nuevoContexto, (key, oldValue) => nuevoContexto);
                if (jugadoresSalaCallbacks.TryGetValue(nombreJugador, out ISalaJugadorCallback contextoCambiado))
                {
                    if (ReferenceEquals(nuevoContexto, contextoCambiado))
                    {
                        eventosCommunication.TryAdd(nombreJugador, new DesconectorEventoManejador((ICommunicationObject)contextoCambiado, this, nombreJugador));
                        resultado = true;
                    }
                }
            }
            return resultado;
        }
        /// <summary>
        /// Despues de este metodo cualquier referencia al jugador en sala se pierde
        /// </summary>
        /// <param name="nombreJugador"></param>
        async Task RemoverJugadorSalaAsync(string nombreJugador)
        {
            await semaphoreRemoverJugador.WaitAsync();
            try
            {
                jugadoresSalaCallbacks.TryRemove(nombreJugador, out ISalaJugadorCallback _);
                eventosCommunication.TryRemove(nombreJugador, out DesconectorEventoManejador eventosJugador);
                jugadoresInformacion.TryRemove(nombreJugador, out _);

                if (nombreJugador.Equals(Anfitrion, StringComparison.OrdinalIgnoreCase) && !(SALA_VACIA == ObtenerNombresJugadoresSala().Count))
                {
                    DelegarRolAnfitrion();
                }

                if (eventosJugador != null)
                {
                    eventosJugador.Desechar();
                    await manejadorVeto.RegistrarExpulsionJugadorAsync(nombreJugador, MOTIVO_EXPULSION_SALA, false);
                }

                if (ContarJugadores() == SALA_VACIA)
                {
                    EliminarSala();
                }
            }
            finally
            {
                semaphoreRemoverJugador.Release();
            }
        }


        private void EliminarSala()
        {
            IReadOnlyCollection<string> claveJugadores = ObtenerNombresJugadoresSala();
            foreach (var clave in claveJugadores)
            {
                if (jugadoresSalaCallbacks.ContainsKey(clave))
                {
                    ((ICommunicationObject)jugadoresSalaCallbacks[clave]).Close();
                }
            }
            jugadoresSalaCallbacks.Clear();
            IReadOnlyCollection<string> claveEventos = ObtenerNombresJugadoresSala();
            foreach (var clave in claveEventos)
            {
                if (eventosCommunication.ContainsKey(clave))
                {
                    eventosCommunication[clave].Desechar();
                }
            }
            eventosCommunication.Clear();

            EnSalaVacia();

        }

        void DelegarRolAnfitrion()
        {
            if (jugadoresSalaCallbacks == null || !jugadoresSalaCallbacks.Any())
            {
                return;
            }
            var jugadoresKeys = jugadoresSalaCallbacks.Keys.ToList();
            Random random = new Random();
            int indiceAleatorio = random.Next(jugadoresKeys.Count);
            string jugadorClave = jugadoresKeys[indiceAleatorio];
            Anfitrion = jugadorClave;
            jugadoresSalaCallbacks.TryGetValue(jugadorClave, out ISalaJugadorCallback callback);
            try
            {
                callback?.DelegacionRolCallback(true);
            }
            catch (Exception)
            {
                if (this is IObservador observador)
                {
                    observador.DesconectarUsuario(jugadorClave);
                }
            }
        }

        public async void DesconectarUsuario(string nombreJugador)
        {
            AvisarRetiroJugador(nombreJugador);
            await RemoverJugadorSalaAsync(nombreJugador);
        }

        public async Task AvisarNuevoJugador(string nombreJugador)
        {
            DAOLibreria.ModeloBD.Usuario informacionUsuario = DAOLibreria.DAO.UsuarioDAO.ObtenerUsuarioPorNombre(nombreJugador);
            bool esInvitado = false;
            if (informacionUsuario == null)
            {
                await semaphoreLeerFotoInvitado.WaitAsync();
                try
                {
                    //TODO: Hacer algo si no hay imagenes
                    string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recursos", "FotosInvitados");
                    var archivos = Directory.GetFiles(ruta, "*.png");
                    string archivoAleatorio = archivos[random.Value.Next(archivos.Length)];
                    string nombreSinExtension = Path.GetFileNameWithoutExtension(archivoAleatorio);
                    esInvitado = true;
                    informacionUsuario = new DAOLibreria.ModeloBD.Usuario
                    {
                        gamertag = nombreJugador,
                        fotoPerfil = File.ReadAllBytes(archivoAleatorio)
                    };
                }
                catch (Exception)
                {
                }
                finally
                {
                    semaphoreLeerFotoInvitado.Release();
                }
            }
            lock (jugadoresSalaCallbacks)
            {
                lock (jugadoresInformacion)
                {
                    jugadoresInformacion.TryAdd(nombreJugador, informacionUsuario);
                    Usuario usuario = new Usuario
                    {
                        Nombre = informacionUsuario.gamertag,
                        FotoUsuario = new MemoryStream(informacionUsuario.fotoPerfil)
                    };
                    if (jugadoresSalaCallbacks.TryGetValue(nombreJugador, out ISalaJugadorCallback nuevoCallback))
                    {
                        foreach (var jugadorExistente in jugadoresInformacion.Values)
                        {
                            Usuario jugador = new Usuario
                            {
                                Nombre = jugadorExistente.gamertag,
                                FotoUsuario = new MemoryStream(jugadorExistente.fotoPerfil),
                                EsInvitado = esInvitado
                            };
                            nuevoCallback.ObtenerJugadorSalaCallback(jugador);
                        }
                    }
                    foreach (var jugadorConectado in ObtenerNombresJugadoresSala())
                    {
                        if (jugadoresSalaCallbacks.TryGetValue(jugadorConectado, out ISalaJugadorCallback callback))
                        {
                            if (jugadorConectado != nombreJugador)
                            {
                                callback.ObtenerJugadorSalaCallback(usuario);
                            }
                        }
                    }

                }
            }
        }

        private void AvisarRetiroJugador(string nombreUsuarioEliminado)
        {
            lock (jugadoresSalaCallbacks)
            {
                lock (jugadoresInformacion)
                {
                    jugadoresInformacion.TryRemove(nombreUsuarioEliminado, out DAOLibreria.ModeloBD.Usuario usuarioEliminado);
                    if (usuarioEliminado != null)
                    {
                        Usuario usuario = new Usuario
                        {
                            Nombre = usuarioEliminado.gamertag
                        };
                        foreach (var nombreJugador in ObtenerNombresJugadoresSala())
                        {
                            jugadoresSalaCallbacks.TryGetValue(nombreJugador, out ISalaJugadorCallback callback);
                            if (callback != null)
                            {
                                try
                                {
                                    callback.EliminarJugadorSalaCallback(usuario);

                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// El 
        /// </summary>
        /// <param name="nombreSolicitante"></param>
        /// <param name="idPartida"></param>
        internal bool AvisarComienzoPatida(string nombreSolicitante, string idPartida)
        {
            if (nombreSolicitante.Equals(Anfitrion, StringComparison.OrdinalIgnoreCase))
            {

                try
                {
                    foreach (var nombre in ObtenerNombresJugadoresSala())
                    {
                        if (!nombre.Equals(Anfitrion, StringComparison.OrdinalIgnoreCase))
                        {
                            jugadoresSalaCallbacks.TryGetValue(nombre, out ISalaJugadorCallback callback);
                            try
                            {
                                callback.EmpezarPartidaCallBack(idPartida);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex}");
                            }
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                }
            }
            return false;

        }
        #endregion Metodos
    }
}
