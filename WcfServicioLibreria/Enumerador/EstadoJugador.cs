using System.Runtime.Serialization;

namespace WcfServicioLibreria.Enumerador
{
    [DataContract]
    public enum EstadoJugador
    {
        [EnumMember]
        Desconectado,
        [EnumMember]
        Conectado,
        [EnumMember]
        Disponible,
        [EnumMember]
        EnSala,
        [EnumMember]
        EnPartida,
    }
    [DataContract]
    public enum EstadoAmigo
    {
        [EnumMember]
        Conectado,
        [EnumMember]
        Desconectado,
        [EnumMember]
        Solicitud,
        [EnumMember]
        ActualizarEstado
    }
}
