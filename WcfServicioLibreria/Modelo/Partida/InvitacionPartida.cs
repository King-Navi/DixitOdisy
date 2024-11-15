using System.IO;
using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class InvitacionPartida
    {
        [DataMember]
        public string GamertagEmisor { get; set; }

        [DataMember]
        public string CodigoSala { get; set; }
        [DataMember]
        public string GamertagReceptor { get; set; }

        public InvitacionPartida() { }
        public InvitacionPartida(string gamertagEmisor, string codigoSala, string gamertagReceptor)
        {
            GamertagEmisor = gamertagEmisor;
            CodigoSala = codigoSala;
            GamertagReceptor = gamertagReceptor;
        }
    }
}