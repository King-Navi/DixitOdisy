using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class UsuarioFalla
    {
        [DataMember]
        public bool EstaConectado { get; set; }
        [DataMember]
        public bool ExisteUsuario { get; set; }
    }
}
