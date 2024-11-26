using System.Runtime.Serialization;

namespace WcfServicioLibreria.Enumerador
{
    [DataContract]
    public enum EstadoUsuario
    {
        [EnumMember]
        Desconectado,
        [EnumMember]
        Conectado
    }
    [DataContract]
    public enum EstadoAmigo
    {
        [EnumMember]
        Conectado,
        [EnumMember]
        Desconectado
    }
    [DataContract]
    public enum CondicionVictoriaPartida
    {
        [EnumMember]
        PorCartasAgotadas,
        [EnumMember]
        PorCantidadRondas
    }
    [DataContract]
    public enum TematicaPartida
    {
        [EnumMember]
        Mixta,
        [EnumMember]
        Animales,
        [EnumMember]
        Paises,
        [EnumMember]
        Mitologia,
        [EnumMember]
        Espacio
    }
}
