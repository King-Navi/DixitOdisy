using System.Runtime.Serialization;

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
