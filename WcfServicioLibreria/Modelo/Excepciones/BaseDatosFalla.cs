using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo.Excepciones
{
    [DataContract]
    public class BaseDatosFalla
    {
        [DataMember]
        public bool EsGamertagDuplicado { get; set; }
    }
}
