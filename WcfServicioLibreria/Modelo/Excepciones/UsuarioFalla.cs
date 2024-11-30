using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class UsuarioFalla
    {
        [DataMember]
        public bool EstaConectado { get; set; } = false;
        [DataMember]
        public bool ExisteUsuario { get; set; } = false;

    }
}
