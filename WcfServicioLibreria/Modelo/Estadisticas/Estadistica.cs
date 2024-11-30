using DAOLibreria.ModeloBD;
using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class Estadistica
    {
        private const int VALOR_POR_DEFECTO = 0;
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

        public Estadistica() 
        {
            PartidasJugadas = VALOR_POR_DEFECTO;
            PartidasGanadas = VALOR_POR_DEFECTO;
            PartidasMixtaJugadas = VALOR_POR_DEFECTO;
            PartidasEspacioJugadas = VALOR_POR_DEFECTO;
            PartidasMitologiaJugadas = VALOR_POR_DEFECTO;
            PartidasAnimalesJugadas = VALOR_POR_DEFECTO;
            PartidasPaisesJugadas = VALOR_POR_DEFECTO;
        }

        public Estadistica(DAOLibreria.ModeloBD.Estadisticas estadisticas)
        {
            if (estadisticas == null)
            {
                estadisticas = new Estadisticas();
            }
            PartidasJugadas = estadisticas.partidasJugadas ?? VALOR_POR_DEFECTO;
            PartidasGanadas = estadisticas.partidasGanadas ?? VALOR_POR_DEFECTO;
            PartidasMixtaJugadas = estadisticas.vecesTematicaMixto ?? VALOR_POR_DEFECTO;
            PartidasEspacioJugadas = estadisticas.vecesTematicaEspacio ?? VALOR_POR_DEFECTO;
            PartidasMitologiaJugadas = estadisticas.vecesTematicaMitologia ?? VALOR_POR_DEFECTO;
            PartidasAnimalesJugadas = estadisticas.vecesTematicaAnimales ?? VALOR_POR_DEFECTO;
            PartidasPaisesJugadas = estadisticas.vecesTematicaPaises ?? VALOR_POR_DEFECTO;
        }
    }
}
