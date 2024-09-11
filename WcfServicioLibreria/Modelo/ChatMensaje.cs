using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class ChatMensaje
    {
        private ChatUsuario usuario;
        private string mensaje;
        private DateTime horaFecha;
        [DataMember]
        public ChatUsuario Usuario { get => usuario; set => usuario = value; }
        [DataMember]
        public string Mensaje { get => mensaje; set => mensaje = value; }
        [DataMember]
        public DateTime HoraFecha { get => horaFecha; set => horaFecha = value; }
    }
}
