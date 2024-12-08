﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Modelo
{
    public class MultiChat : Chat, IObservador
    {
        #region Atributos
        private const int CANTIDAD_MAXIMA_USUARIOS = 12;
        private readonly ConcurrentDictionary<string, IChatCallback> jugadores = new ConcurrentDictionary<string, IChatCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        public EventHandler EliminarChatManejadorEvento;
        private static readonly SemaphoreSlim semaphoreAgrearUsuario = new SemaphoreSlim(1, 1);

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

        public async Task<bool> AgregarJugadorChatAsync(string nombreUsuario, IChatCallback contexto)
        {
            bool resultado = false;

            await semaphoreAgrearUsuario.WaitAsync();
            try
            {
                foreach (var nombreJugadorEnSala in ObtenerNombresJugadoresChat())
                {
                    if (nombreJugadorEnSala.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
                if (ContarJugadores() < CANTIDAD_MAXIMA_USUARIOS)
                {
                    jugadores.AddOrUpdate(nombreUsuario, contexto, (key, oldValue) => contexto);
                    eventosCommunication.TryAdd(nombreUsuario, new DesconectorEventoManejador((ICommunicationObject)contexto, this, nombreUsuario));
                    resultado = true;
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            finally
            {
                semaphoreAgrearUsuario.Release();
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
            ICollection<string> nombresUsuarios = jugadores.Keys;
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
