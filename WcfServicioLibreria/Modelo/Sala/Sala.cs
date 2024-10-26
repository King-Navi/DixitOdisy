using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
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
        private const int cantidadMinimaJugadores = 3;
        private const int cantidadMaximaJugadores = 12;
        private readonly ConcurrentDictionary<string, ISalaJugadorCallback> jugadoresSalaCallbacks = new ConcurrentDictionary<string, ISalaJugadorCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        private ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario> jugadoresInformacion = new ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario>();

        public EventHandler salaVaciaManejadorEvento;

        #endregion Campos
        #region Propiedades
        public static int CantidadMaximaJugadores => cantidadMaximaJugadores;
        public static int CantidadMinimaJugadores => cantidadMinimaJugadores;

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
            if (ContarJugadores() < cantidadMaximaJugadores)
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
            eventosJugador.Desechar();
            if (ContarJugadores() == 0)
            {
                EliminarSala();
            }
        }
        private void EliminarSala() 
        {
            IReadOnlyCollection<string> claveEventos = ObtenerNombresJugadoresSala();
            foreach (var clave in claveEventos)
            {
                if (eventosCommunication.ContainsKey(clave))
                {
                    eventosCommunication[clave].Desechar();
                }
            }
            eventosCommunication.Clear();
            IReadOnlyCollection<string> claveJugadores = ObtenerNombresJugadoresSala();
            foreach (var clave in claveJugadores)
            {
                if (jugadoresSalaCallbacks.ContainsKey(clave))
                {
                    ((ICommunicationObject)jugadoresSalaCallbacks[clave]).Close();
                }
            }
            jugadoresSalaCallbacks.Clear();
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

        public void AvisarNuevoJugador(string nombreJugador)
        {
            DAOLibreria.ModeloBD.Usuario informacionUsuario = DAOLibreria.DAO.UsuarioDAO.ObtenerUsuarioPorNombre(nombreJugador);
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
                                FotoUsuario = new MemoryStream(jugadorExistente.fotoPerfil)
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
