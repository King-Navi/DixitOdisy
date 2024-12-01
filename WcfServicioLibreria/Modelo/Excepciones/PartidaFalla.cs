using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo.Excepciones
{
    [DataContract]
    public class PartidaFalla
    {
        [DataMember]
        public bool PartidaInvalida { get; set; } = false;
        [DataMember]
        public bool ErrorAlUnirse { get; set; } = false;
    }
}
