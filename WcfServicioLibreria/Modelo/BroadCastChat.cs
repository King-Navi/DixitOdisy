using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Modelo
{
    public class BroadCastChat : Chat
    {
        #region Atributos
        private ConcurrentDictionary<string, IChatCallback> jugadores = new ConcurrentDictionary<string, IChatCallback>();

        #endregion Atributos
        #region Propiedades

        #endregion Propiedades
        #region Contructor
        public BroadCastChat(string _idChat) : base(_idChat)
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
        #endregion Metodos

    }
}
