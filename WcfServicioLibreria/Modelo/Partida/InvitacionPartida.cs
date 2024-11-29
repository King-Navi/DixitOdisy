using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class InvitacionPartida
    {
        [DataMember]
        public string NombreEmisor { get; set; }

        [DataMember]
        public string CodigoSala { get; set; }
        [DataMember]
        public string NombreReceptor { get; set; }

        public InvitacionPartida() { }
        public InvitacionPartida(string _nombreEmisor, string _codigoSala, string _nombreReceptor)
        {
            NombreEmisor = _nombreEmisor;
            CodigoSala = _codigoSala;
            NombreReceptor = _nombreReceptor;
        }
    }
}