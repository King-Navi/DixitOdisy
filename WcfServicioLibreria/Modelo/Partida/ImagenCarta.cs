using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
