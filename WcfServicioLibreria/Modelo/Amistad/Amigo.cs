using System.IO;
using System.Runtime.Serialization;
namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class Amigo
    {
        [DataMember]
        public Stream Foto { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Estado { get; set; }
    }
}
