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
using WcfServicioLibreria.Utilidades;
namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class Sala : IObservador
    {
        #region Campos
        private string idCodigoSala;
        private string anfitrion;
        private const int CANTIDAD_MINIMA_JUGADORES = 3;
        private const int CANTIDAD_MAXIMA_JUGADORES = 12;
        private readonly ConcurrentDictionary<string, ISalaJugadorCallback> jugadoresSalaCallbacks = new ConcurrentDictionary<string, ISalaJugadorCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        private ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario> jugadoresInformacion = new ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario>();
        private static ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random());
        public EventHandler salaVaciaManejadorEvento;
        private readonly SemaphoreSlim semaphoreLeerFotoInvitado = new SemaphoreSlim(1, 1);

        #endregion Campos
        #region Propiedades
        public static int CantidadMaximaJugadores => CANTIDAD_MAXIMA_JUGADORES;
        public static int CantidadMinimaJugadores => CANTIDAD_MINIMA_JUGADORES;

        public string IdCodigoSala { get => idCodigoSala; internal set => idCodigoSala = value; }
        #endregion Propiedades

        #region Contructores
        public Sala(string _idCodigoSala, string nombreUsuario)
        {
            this.IdCodigoSala = _idCodigoSala;
            this.anfitrion = nombreUsuario;
            jugadoresSalaCallbacks.TryAdd(nombreUsuario, null);
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

        private IReadOnlyCollection<string> ObtenerNombresJugadoresSala()
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
        void RemoverJugadorSala(string nombreJugador)
        {
            jugadoresSalaCallbacks.TryRemove(nombreJugador, out ISalaJugadorCallback _);
            eventosCommunication.TryRemove(nombreJugador, out DesconectorEventoManejador eventosJugador);
            jugadoresInformacion.TryRemove(nombreJugador, out  _);
            if (eventosJugador != null)
            {
                eventosJugador.Desechar();
            }
            if (ContarJugadores() == 0)
            {
                EliminarSala();
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

        bool DelegarRolAnfitrion(string nuevoAnfitrionNombre)
        {
            bool existeJugador = jugadoresSalaCallbacks.TryGetValue(nuevoAnfitrionNombre, out _);
            if (!existeJugador)
            {
                return false;
            }
            anfitrion = nuevoAnfitrionNombre;
            return anfitrion == nuevoAnfitrionNombre;
        }

        void IObservador.DesconectarUsuario(string nombreJugador)
        {
            AvisarRetiroJugador(nombreJugador);
            RemoverJugadorSala(nombreJugador);
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
                        //TODO: Si se necesita algo mas del jugador nuevo colocar aqui
                    };
                    // Enviar la lista completa de jugadores al nuevo jugador
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
                    // Enviar la información del nuevo jugador a los jugadores existentes
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

        internal void AvisarComienzoPatida(string nombreSolicitante, string idPartida)
        {
            if (nombreSolicitante.Equals(anfitrion, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var nombre in ObtenerNombresJugadoresSala())
                {
                    try
                    {
                        jugadoresSalaCallbacks.TryRemove(nombre, out ISalaJugadorCallback callback);
                        callback.EmpezarPartidaCallBack(idPartida);
                    }
                    catch (Exception)
                    {
                        //TODO. nombre No se puedo coenctar
                    }
                }
            }
        }
        #endregion Metodos
    }
}
