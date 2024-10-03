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
        public string Nombre { get => nombre; set => nombre = value; }
        [DataMember]
        public string Mensaje { get => mensaje; set => mensaje = value; }
        [DataMember]
        public DateTime HoraFecha { get => horaFecha; set => horaFecha = value; }
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
