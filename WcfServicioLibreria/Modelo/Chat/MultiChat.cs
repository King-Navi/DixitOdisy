using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Modelo
{
    public class MultiChat : Chat , IObservador
    {
        #region Atributos
        private const int cantidadMaximaJugadores = 12;
        private readonly ConcurrentDictionary<string, IChatCallback> jugadores = new ConcurrentDictionary<string, IChatCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        public EventHandler EliminarChatManejadorEvento;
        #endregion Atributos

        #region Contructor
        public MultiChat(string _idChat) : base(_idChat) { }
        #endregion Constructor

        #region Metodos
        internal IReadOnlyCollection<string> ObtenerNombresJugadoresChat()
        {
            return jugadores.Keys.ToList().AsReadOnly();
        }

        private int ContarJugadores()
        {
            return jugadores.Count;
        }
        private void BorrarChatEvent()
        {
            EliminarChatManejadorEvento?.Invoke(this, new MultiChatVacioEventArgs(DateTime.Now, this));
        }

        public bool AgregarJugadorChat(string nombreUsuario, IChatCallback contexto)
        {
            bool resultado = false;
            try
            {
                if (ContarJugadores() < cantidadMaximaJugadores)
                {
                    jugadores.AddOrUpdate(nombreUsuario, contexto, (key, oldValue) => contexto);
                    if (jugadores.TryGetValue(nombreUsuario, out IChatCallback contextoCambiado))
                    {
                        if (ReferenceEquals(contexto, contextoCambiado))
                        {
                            eventosCommunication.TryAdd(nombreUsuario, new DesconectorEventoManejador((ICommunicationObject)contextoCambiado, this, nombreUsuario));
                            resultado = true;
                        }
                    }
                }
            }
            catch (Exception excepcion)
            {
                try
                {
                    ((ICommunicationObject)contexto).Abort();
                }
                catch (Exception excepcionComunicacion)
                {
                    ManejadorExcepciones.ManejarErrorException(excepcionComunicacion);

                }
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return resultado;
        }
        bool RemoverJugadorChat(string nombreJugador)
        {
            bool seElimino = jugadores.TryRemove(nombreJugador, out IChatCallback jugadorEliminado);
            eventosCommunication.TryRemove(nombreJugador, out DesconectorEventoManejador eventosJugador);
            eventosJugador?.Desechar();
            if (ContarJugadores() == 0)
            {
                EliminarChat();
            }
            return seElimino;
        }
        protected override void EliminarChat()
        {
            IReadOnlyCollection<string> claveEventos = ObtenerLlaves();
            foreach (var clave in claveEventos)
            {
                if (jugadores.ContainsKey(clave))
                {
                    ((ICommunicationObject)jugadores[clave]).Close();
                }
            };
            jugadores.Clear();
            BorrarChatEvent();
        }
        public void EnviarMensajeTodos(ChatMensaje mensaje)
        {
            ICollection<string> nombresUsuarios =  jugadores.Keys;
            foreach (string nombreUsuario in nombresUsuarios)
            {
                jugadores[nombreUsuario].RecibirMensajeClienteCallback(mensaje);
            }
        }
        IReadOnlyCollection<string> ObtenerLlaves()
        {
            return jugadores.Keys.ToList().AsReadOnly();
        }

        void IObservador.DesconectarUsuario(string clave)
        {
            RemoverJugadorChat(clave);
        }
        #endregion Metodos

    }
}
