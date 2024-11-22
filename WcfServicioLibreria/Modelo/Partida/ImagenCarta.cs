using System.IO;
using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    [KnownType(typeof(Stream))]
    [KnownType(typeof(MemoryStream))]
    public class ImagenCarta
    {
        [DataMember]
        public string IdImagen { get; set; }
        [DataMember]
        public MemoryStream ImagenStream { get; set; }
    }
}
