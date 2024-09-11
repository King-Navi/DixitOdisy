using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class Usuario
    {
        int idUsuario;
        String nombre;
        SHA256 contraseniaHASH;
        [DataMember]
        public string Nombre { get => nombre; set => nombre = value; }
        [DataMember]
        public int IdUsuario { get => idUsuario; set => idUsuario = value; }
        [DataMember]
        public SHA256 ContraseniaHASH { get => contraseniaHASH; set => contraseniaHASH = value; }
    }
}
