using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class Estadistica
    {
        [DataMember]
        public int PartidasJugadas { get; set; }
        [DataMember]
        public int PartidasGanadas { get; set; }
        [DataMember]
        public int PartidasMixtaJugadas { get; set; }
        [DataMember]
        public int PartidasEspacioJugadas { get; set; }
        [DataMember]
        public int PartidasMitologiaJugadas { get; set; }
        [DataMember]
        public int PartidasAnimalesJugadas { get; set; }
        [DataMember]
        public int PartidasPaisesJugadas { get; set; }

        public Estadistica() { }

        public Estadistica(DAOLibreria.ModeloBD.Estadisticas estadisticas)
        {
            PartidasJugadas = estadisticas.partidasJugadas ?? 0;
            PartidasGanadas = estadisticas.partidasGanadas ?? 0;
            PartidasMixtaJugadas = estadisticas.vecesTematicaMixto ?? 0;
            PartidasEspacioJugadas = estadisticas.vecesTematicaEspacio ?? 0;
            PartidasMitologiaJugadas = estadisticas.vecesTematicaMitologia ?? 0;
            PartidasAnimalesJugadas = estadisticas.vecesTematicaAnimales ?? 0;
            PartidasPaisesJugadas = estadisticas.vecesTematicaPaises ?? 0;
        }
    }
}
