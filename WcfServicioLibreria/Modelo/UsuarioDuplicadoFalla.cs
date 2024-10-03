using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class UsuarioDuplicadoFalla
    {
            [DataMember]
            public string Motivo { get; set; }
            
     }
    
}
