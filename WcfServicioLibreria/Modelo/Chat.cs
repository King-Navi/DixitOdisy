using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public abstract class Chat
    {
        #region Atributos
        private string idChat;
        #endregion Atributos
        #region Propiedades
        #endregion Propiedades
        #region Constructor
        public Chat(string _idChat)
        { 
            this.idChat = _idChat;
        }
        #endregion Constructor
        #region Metodos
        public abstract bool EliminarChat();
        #endregion Metodos
    }
}
