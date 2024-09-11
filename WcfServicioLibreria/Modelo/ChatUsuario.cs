using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class ChatUsuario
    {
        private string nombreUsuario, direccionIP, nombreHospedador;
        [DataMember]
        public string NombreUsuario { get => nombreUsuario; set => nombreUsuario = value; }
        [DataMember]
        public string DireccionIP { get => direccionIP; set => direccionIP = value; }
        [DataMember]
        public string NombreHospedador { get => nombreHospedador; set => nombreHospedador = value; }
        public override string ToString()
        {
            return this.NombreUsuario;
        }
    }
}
