using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo.Excepciones
{
    [DataContract]
    public class SalaFalla
    {
        [DataMember]
        public bool EstaLlena { get; set; } = false;

        public SalaFalla() { }
    }
}
