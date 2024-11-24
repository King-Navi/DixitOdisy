using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo.Excepciones
{
    [DataContract]
    public class VetoFalla
    {
        [DataMember]
        public bool EsPermanete { get; set; } = false;
        [DataMember]
        public bool EnProgreso { get; set; } = false;
    }
}
