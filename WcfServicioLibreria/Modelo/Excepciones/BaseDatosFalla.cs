using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo.Excepciones
{
    [DataContract]
    public class BaseDatosFalla
    {
        [DataMember]
        public bool EsGamertagDuplicado { get; set; }
    }
}
