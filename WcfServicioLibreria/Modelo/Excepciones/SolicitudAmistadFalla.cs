using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo 
{
    [DataContract]
    public class SolicitudAmistadFalla
    {
        [DataMember]
        public bool ExisteAmistad { get; set; }

        [DataMember]
        public bool ExistePeticion { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        public SolicitudAmistadFalla(bool existeAmistad, bool existePeticion)
        {
            ExisteAmistad = existeAmistad;
            ExistePeticion = existePeticion;

            Mensaje = existeAmistad
                ? "No puedes enviar solicitud de amistad a alguien que ya es tu amigo dentro del juego."
                : "Ya le enviaste una solicitud de amistad a ese usuario.";
        }
    }
}
