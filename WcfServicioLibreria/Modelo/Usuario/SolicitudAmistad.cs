using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class SolicitudAmistad
    {
        [DataMember]
        public Usuario Remitente { get; set; }
    }
}
