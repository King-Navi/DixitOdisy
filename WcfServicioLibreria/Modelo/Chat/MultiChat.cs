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
        private ConcurrentDictionary<string, IChatCallback> jugadores = new ConcurrentDictionary<string, IChatCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        public EventHandler EliminarChatManejadorEvento;
        #endregion Atributos
        #region Propiedades

        #endregion Propiedades
        #region Contructor
        public MultiChat(string _idChat) : base(_idChat)
        {

        }
        #endregion Constructor
        #region Metodos
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
            return resultado;
        }
        bool RemoverJugadorChat(string nombreJugador)
        {
            bool seElimino = jugadores.TryRemove(nombreJugador, out IChatCallback jugadorEliminado);
            eventosCommunication.TryGetValue(nombreJugador, out DesconectorEventoManejador eventosJugador);
            eventosJugador.Desechar();
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
                jugadores[nombreUsuario].RecibirMensajeCliente(mensaje);
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
