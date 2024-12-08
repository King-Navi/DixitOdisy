using System.IO;
using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    [KnownType(typeof(byte[]))]
    public class ImagenCarta
    {
        [DataMember]
        public string IdImagen { get; set; }
        [DataMember]
        public byte[] ImagenStream { get; set; }
    }
}
