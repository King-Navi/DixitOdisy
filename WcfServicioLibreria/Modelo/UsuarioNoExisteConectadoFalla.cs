using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class UsuarioNoExisteConectadoFalla
    {
        [DataMember]
        public bool Existe { get; set; }
    }
}
