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
