using System;
using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class ChatMensaje
    {
        #region Atributos
        private string nombre;
        private string mensaje;
        private DateTime horaFecha;
        #endregion Atributos
        #region Propiedades
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Mensaje { get; set; }
        [DataMember]
        public DateTime HoraFecha { get; set; }
        #endregion Propiedades
        #region Constructores
        public ChatMensaje(string _nombre, string _mensaje, DateTime _horaFecha)
        {
            nombre = _nombre;
            mensaje = _mensaje;
            horaFecha= _horaFecha;
        }
        #endregion Constructores
        #region Metodos
        public override string ToString()
        {
            return $"{HoraFecha.ToLocalTime()} {Nombre} : {Mensaje}";
        }
        #endregion Metodos
    }
}
