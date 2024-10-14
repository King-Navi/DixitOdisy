using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class AmistadFalla
    {
        [DataMember]
        public bool ExisteAmistad { get; set; }
        [DataMember]
        public bool ExistePeticion { get; set; }
        public AmistadFalla(bool existeAmistad, bool existePeticion)
        { 
            ExistePeticion = existePeticion;
            ExisteAmistad = existeAmistad;
        }
    }
}
