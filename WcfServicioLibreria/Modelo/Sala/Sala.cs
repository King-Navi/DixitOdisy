using DAOLibreria.DAO;
using DAOLibreria.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        private const string TODOS_ARCHIVOS_EXTENSION_PNG = "*.png";
        private const string RUTA_RECURSOS = "Recursos";
        private const string CARPETA_FOTOS_INVITADOS = "FotosInvitados";
        #endregion
        #region Campos
        private const int SALA_VACIA = 0;
        private string idCodigoSala;
        private const int CANTIDAD_MINIMA_JUGADORES = 1;
        private const int CANTIDAD_MAXIMA_JUGADORES = 6;
        private readonly ConcurrentDictionary<string, ISalaJugadorCallback> jugadoresSalaCallbacks = new ConcurrentDictionary<string, ISalaJugadorCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        private ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario> jugadoresInformacion = new ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario>();
        private static ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random());
        public EventHandler salaVaciaManejadorEvento;
        private readonly SemaphoreSlim semaphoreLeerFotoInvitado = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim semaphoreRemoverJugador = new SemaphoreSlim(1, 1);
        private IUsuarioDAO usuarioDAO;

        private IManejadorVeto manejadorVeto;
        #endregion Campos
        #region Propiedades
        public string IdCodigoSala { get => idCodigoSala; internal set => idCodigoSala = value; }
        public string Anfitrion { get; private set; }

        #endregion Propiedades

        #region Contructores
        public Sala(string _idCodigoSala, string nombreUsuario, IUsuarioDAO _usuarioDAO)
        {
            this.IdCodigoSala = _idCodigoSala;
            this.Anfitrion = nombreUsuario;
            manejadorVeto = new ManejadorDeVetos();
            usuarioDAO = _usuarioDAO;
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
            var informacionUsuario = await ObtenerInformacionUsuarioAsync(nombreJugador);
            var usuario = CrearUsuario(informacionUsuario);
            bool esInvitado = informacionUsuario == null;
            ActualizarJugadoresSala(nombreJugador, informacionUsuario);
            NotificarNuevoJugador(nombreJugador, usuario, esInvitado);
        }

        private async Task<DAOLibreria.ModeloBD.Usuario> ObtenerInformacionUsuarioAsync(string nombreJugador)
        {
            DAOLibreria.ModeloBD.Usuario informacionUsuario = usuarioDAO.ObtenerUsuarioPorNombre(nombreJugador);
            if (informacionUsuario == null)
            {
                await semaphoreLeerFotoInvitado.WaitAsync();
                try
                {
                    informacionUsuario = CrearUsuarioInvitado(nombreJugador);
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarFatalException(excepcion);
                }
                finally
                {
                    semaphoreLeerFotoInvitado.Release();
                }
            }
            return informacionUsuario;
        }

        private DAOLibreria.ModeloBD.Usuario CrearUsuarioInvitado(string nombreJugador)
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RUTA_RECURSOS, CARPETA_FOTOS_INVITADOS);
            var archivos = Directory.GetFiles(ruta, TODOS_ARCHIVOS_EXTENSION_PNG);
            string archivoAleatorio = archivos[random.Value.Next(archivos.Length)];
            return new DAOLibreria.ModeloBD.Usuario
            {
                gamertag = nombreJugador,
                fotoPerfil = File.ReadAllBytes(archivoAleatorio)
            };
        }

        private Usuario CrearUsuario(DAOLibreria.ModeloBD.Usuario informacionUsuario)
        {
            return new Usuario
            {
                Nombre = informacionUsuario.gamertag,
                FotoUsuario = new MemoryStream(informacionUsuario.fotoPerfil)
            };
        }

        private void ActualizarJugadoresSala(string nombreJugador, DAOLibreria.ModeloBD.Usuario informacionUsuario)
        {
            jugadoresInformacion.AddOrUpdate(
                nombreJugador,
                informacionUsuario,
                (key, oldValue) => informacionUsuario
            );
        }

        private void NotificarNuevoJugador(string nombreJugador, Usuario usuario, bool esInvitado)
        {
            lock (jugadoresSalaCallbacks)
            {
                if (jugadoresSalaCallbacks.TryGetValue(nombreJugador, out ISalaJugadorCallback nuevoCallback))
                {
                    NotificarJugadorExistente(nuevoCallback, esInvitado);
                }

                NotificarJugadoresConectados(nombreJugador, usuario);
            }
        }

        private void NotificarJugadorExistente(ISalaJugadorCallback nuevoCallback, bool esInvitado)
        {
            foreach (var jugadorExistente in jugadoresInformacion.Values)
            {
                Usuario jugador = new Usuario
                {
                    Nombre = jugadorExistente.gamertag,
                    FotoUsuario = new MemoryStream(jugadorExistente.fotoPerfil),
                    EsInvitado = esInvitado
                };
                try
                {
                    nuevoCallback?.ObtenerJugadorSalaCallback(jugador);
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarErrorException(excepcion);
                }
            }
        }

        private void NotificarJugadoresConectados(string nombreJugador, Usuario usuario)
        {
            foreach (var jugadorConectado in ObtenerNombresJugadoresSala())
            {
                if (jugadoresSalaCallbacks.TryGetValue(jugadorConectado, out ISalaJugadorCallback callback))
                {
                    if (jugadorConectado != nombreJugador)
                    {
                        try
                        {
                            callback?.ObtenerJugadorSalaCallback(usuario);
                        }
                        catch (Exception excepcion)
                        {
                            ManejadorExcepciones.ManejarErrorException(excepcion);
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
                                catch (Exception excepcion)
                                {
                                    ManejadorExcepciones.ManejarErrorException(excepcion);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal bool AvisarComienzoPatida(string nombreSolicitante, string idPartida)
        {
            if (ContarJugadores() < CANTIDAD_MINIMA_JUGADORES)
            {
                return false;
            }
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
                                callback.EmpezarPartidaCallback(idPartida);
                            }
                            catch (Exception excepcion)
                            {
                                ManejadorExcepciones.ManejarErrorException(excepcion);
                            }
                        }
                    }
                    return true;
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarFatalException(excepcion);
                }
            }
            return false;

        }
        #endregion Metodos
    }
}
