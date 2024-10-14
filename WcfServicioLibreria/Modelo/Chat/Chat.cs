using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public abstract class Chat
    {
        #region Atributos
        protected string idChat;
        #endregion Atributos
        #region Propiedades
        public string IdChat { get; private set; }
        #endregion Propiedades
        #region Constructor
        public Chat(string _idChat)
        { 
            IdChat = _idChat;
        }
        #endregion Constructor
        #region Metodos
        protected abstract void EliminarChat();
        #endregion Metodos
    }
}
