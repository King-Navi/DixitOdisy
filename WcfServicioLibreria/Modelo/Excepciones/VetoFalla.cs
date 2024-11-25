using System.Runtime.Serialization;

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
