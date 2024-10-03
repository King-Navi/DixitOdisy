using System.Runtime.Serialization;

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
