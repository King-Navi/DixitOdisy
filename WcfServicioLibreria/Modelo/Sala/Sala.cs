﻿using DAOLibreria.Interfaces;
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
        private const string TODOS_ARCHIVOS_EXTENSION_PNG = "*.png";
        private const string RUTA_RECURSOS = "Recursos";
        private const string CARPETA_FOTOS_INVITADOS = "FotosInvitados";
        #endregion

        #region Campos
        public const int NO_UNISER = 0;
        public const int UNISER = 1;
        private const int SALA_VACIA = 0;
        private const int TIEMPO_DESECHO_SEGUNDOS = 10;
        private string idCodigoSala;
        private const int CANTIDAD_MINIMA_JUGADORES = 1;
        public const int CANTIDAD_MAXIMA_JUGADORES = 4;
        private readonly ConcurrentDictionary<string, ISalaJugadorCallback> jugadoresSalaCallbacks = new ConcurrentDictionary<string, ISalaJugadorCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        private ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario> jugadoresInformacion = new ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario>();
        private static ThreadLocal<Random> aleatorio = new ThreadLocal<Random>(() => new Random());
        public EventHandler salaVaciaManejadorEvento;
        private readonly SemaphoreSlim semaphoreLeerFotoInvitado = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim semaphoreRemoverJugador = new SemaphoreSlim(1, 1);
        private IUsuarioDAO usuarioDAO;
        public int sePuedeUnir = UNISER;
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

        internal IReadOnlyCollection<string> ObtenerNombresJugadoresSala()
        {
            return jugadoresSalaCallbacks.Keys.ToList().AsReadOnly();
        }

        public bool AgregarInformacionJugadorSala(string nombreJugador, ISalaJugadorCallback nuevoContexto)
        {
            bool resultado = false;
            try
            {
                if (ContarJugadores() <= CANTIDAD_MAXIMA_JUGADORES)
                {
                    jugadoresSalaCallbacks.AddOrUpdate(nombreJugador, nuevoContexto, (key, oldValue) => nuevoContexto);
                    if (jugadoresSalaCallbacks.TryGetValue(nombreJugador, out ISalaJugadorCallback contextoCambiado)
                        && ReferenceEquals(nuevoContexto, contextoCambiado))
                    {

                        eventosCommunication.TryAdd(nombreJugador, new DesconectorEventoManejador((ICommunicationObject)contextoCambiado, this, nombreJugador));
                        resultado = true;

                    }
                }
            }
            catch (Exception excepcion)
            {
                try
                {
                    ((ICommunicationObject)nuevoContexto).Abort();
                }
                catch (Exception excepcionComunicacion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcionComunicacion);
                }
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
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

                if (nombreJugador.Equals(Anfitrion, StringComparison.OrdinalIgnoreCase) && (ObtenerNombresJugadoresSala().Count !=  SALA_VACIA))
                {
                    await DelegarRolAnfitrionAsync();
                }

                if (eventosJugador != null)
                {
                    eventosJugador.Desechar();
                }

                if (ContarJugadores() == SALA_VACIA)
                {
                    ProgramarEliminacionSala();
                }
            }
            finally
            {
                semaphoreRemoverJugador.Release();
            }
        }

        private void ProgramarEliminacionSala()
        {
            if (NO_UNISER == sePuedeUnir)
            {
                return;
            }

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(TIEMPO_DESECHO_SEGUNDOS));
                    EliminarSala();
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
                }
            });
        }

        private void EliminarSala()
        {
            try
            {
                IReadOnlyCollection<string> claveJugadores = ObtenerNombresJugadoresSala();
                foreach (var clave in claveJugadores)
                {
                    if (jugadoresSalaCallbacks.ContainsKey(clave))
                    {
                        CerrarCallbackSiAbierto(jugadoresSalaCallbacks[clave]);
                    }
                }
                jugadoresSalaCallbacks.Clear();

                IReadOnlyCollection<string> claveEventos = ObtenerNombresJugadoresSala();
                foreach (var clave in claveEventos)
                {
                    if (eventosCommunication.ContainsKey(clave))
                    {
                        CerrarCallbackSiAbierto(eventosCommunication[clave]);
                    }
                }
                eventosCommunication.Clear();
                sePuedeUnir = NO_UNISER;

                EnSalaVacia();
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
        }

        private void CerrarCallbackSiAbierto(object callbackObj)
        {
            var callback = (ICommunicationObject)callbackObj;
            if (callback.State == CommunicationState.Opening
                || callback.State == CommunicationState.Opened)
            {
                callback.Close();
            }
        }


        private async Task DelegarRolAnfitrionAsync()
        {
            if (jugadoresSalaCallbacks == null || !jugadoresSalaCallbacks.Any())
            {
                return;
            }
            var jugadoresKeys = jugadoresSalaCallbacks.Keys.ToList();
            Random _aleatorio = new Random();
            int indiceAleatorio = _aleatorio.Next(jugadoresKeys.Count);
            string jugadorClave = jugadoresKeys[indiceAleatorio];
            Anfitrion = jugadorClave;
            jugadoresSalaCallbacks.TryGetValue(jugadorClave, out ISalaJugadorCallback callback);
            try
            {
                callback?.DelegacionRolCallback(true);
            }
            catch (Exception)
            {
                await DesconectarUsuarioAsync(jugadorClave);
            }
        }

        public async Task DesconectarUsuarioAsync(string nombreJugador)
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
                    ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
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
            string archivoAleatorio = archivos[aleatorio.Value.Next(archivos.Length)];
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
            if (jugadoresSalaCallbacks.TryGetValue(nombreJugador, out ISalaJugadorCallback nuevoCallback))
            {
                NotificarJugadorExistente(nuevoCallback, esInvitado);
            }
            NotificarJugadoresConectados(nombreJugador, usuario);
        }

        private void NotificarJugadorExistente(ISalaJugadorCallback nuevoCallback, bool esInvitado)
        {
            try
            {
                foreach (var jugadorExistente in jugadoresInformacion.Values)
                {
                    Usuario jugador = new Usuario
                    {
                        Nombre = jugadorExistente.gamertag,
                        FotoUsuario = new MemoryStream(jugadorExistente.fotoPerfil),
                        EsInvitado = esInvitado
                    };
                    Task.Run(() =>
                    {
                        try
                        {
                            nuevoCallback?.ObtenerJugadorSalaCallback(jugador);
                        }
                        catch (Exception excepcion)
                        {
                            ManejadorExcepciones.ManejarExcepcionError(excepcion);
                        }
                    });
                }
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }

        private void NotificarJugadoresConectados(string nombreJugador, Usuario usuario)
        {
            try
            {
                foreach (var jugadorConectado in ObtenerNombresJugadoresSala())
                {
                    if (jugadoresSalaCallbacks.TryGetValue(jugadorConectado, out ISalaJugadorCallback callback)
                        && jugadorConectado != nombreJugador)
                    {

                        Task.Run(() =>
                        {
                            try
                            {
                                callback?.ObtenerJugadorSalaCallback(usuario);
                            }
                            catch (Exception excepcion)
                            {
                                ManejadorExcepciones.ManejarExcepcionError(excepcion);
                            }
                        });
                    }
                }
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
                                    ManejadorExcepciones.ManejarExcepcionError(excepcion);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal bool AvisarComienzoPatida(string nombreSolicitante, string idPartida)
        {
            sePuedeUnir = NO_UNISER;
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
                                ManejadorExcepciones.ManejarExcepcionError(excepcion);
                            }
                        }
                    }
                    return true;
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
                }
            }
            return false;

        }
        #endregion Metodos
    }
}
