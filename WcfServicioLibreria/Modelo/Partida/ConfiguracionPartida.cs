using System.Runtime.Serialization;
using System.ServiceModel;
using WcfServicioLibreria.Enumerador;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    [KnownType(typeof(CondicionVictoriaPartida))]
    [KnownType(typeof(TematicaPartida))]
    public class ConfiguracionPartida
    {
        [DataMember]
        public TematicaPartida Tematica { get; set; }
        [DataMember]
        public CondicionVictoriaPartida Condicion { get; set; }
        [DataMember]
        public int NumeroRondas { get; set; }

        public ConfiguracionPartida(TematicaPartida _tematica,CondicionVictoriaPartida _condicion, int _numeroRondas ) 
        {
            Tematica = _tematica;
            Condicion = _condicion;
            NumeroRondas = _numeroRondas;
        }
    }
}