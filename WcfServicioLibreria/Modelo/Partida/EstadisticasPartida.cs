using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Enumerador;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    [KnownType(typeof(TematicaPartida))]
    [KnownType(typeof(List<JugadorEstadisticas>))]
    public class EstadisticasPartida
    {
        [DataMember]
        public TematicaPartida Tematica { get; private set; }
        
        [DataMember]
        public List<JugadorEstadisticas> Jugadores { get; set; } 

        [DataMember]
        public int TotalRondas { get; set; } 
        
        [DataMember]
        public JugadorEstadisticas PrimerLugar { get; set; }
        
        [DataMember]
        public JugadorEstadisticas SegundoLugar { get; set; }
        
        [DataMember]
        public JugadorEstadisticas TercerLugar { get; set; }

        public EstadisticasPartida(TematicaPartida tematica)
        {
            Tematica = tematica;
            Jugadores = new List<JugadorEstadisticas>();
            TotalRondas = 0;
            PrimerLugar = null;
            SegundoLugar = null;
            TercerLugar = null;
        }
    }

    [DataContract]
    public class JugadorEstadisticas
    {
        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public int Puntos { get; set; }
        public JugadorEstadisticas(string nombre)
        {
            Nombre = nombre;
            Puntos = 0;
        }

    }
}
