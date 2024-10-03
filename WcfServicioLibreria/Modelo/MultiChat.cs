using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Modelo
{
    public class MultiChat : Chat
    {
        #region Atributos
        private ConcurrentDictionary<string, IChatCallback> jugadores = new ConcurrentDictionary<string, IChatCallback>();

        #endregion Atributos
        #region Propiedades

        #endregion Propiedades
        #region Contructor
        public MultiChat(string _idChat) : base(_idChat)
        {

        }
        #endregion Constructor
        #region Metodos
        public bool AgregarJugadorChat(string nombreUsuario, IChatCallback contexto)
        {
            return jugadores.TryAdd(nombreUsuario, contexto);
        }
        public override bool EliminarChat()
        {
            throw new NotImplementedException();
        }
        public void EnviarMensajeTodos(ChatMensaje mensaje)
        {
            ICollection<string> nombresUsuarios =  jugadores.Keys;
            foreach (string nombreUsuario in nombresUsuarios)
            {
                jugadores[nombreUsuario].RecibirMensajeCliente(mensaje);
            }
        }
        #endregion Metodos

    }
}
