using DAOLibreria.DAO;
using DAOLibreria.ModeloBD;
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

        public void CalcularPodio()
        {
            var jugadoresOrdenados = Jugadores.OrderByDescending(j => j.Puntos).ToList();
            PrimerLugar = jugadoresOrdenados.ElementAtOrDefault(0);
            SegundoLugar = jugadoresOrdenados.ElementAtOrDefault(1);
            TercerLugar = jugadoresOrdenados.ElementAtOrDefault(2);
        }

        internal async Task GuardarPuntajeAsync(List<Tuple<String , int >> listaTuplaNombreIdEstadistica)
        {
            var accion = SelecionarAccion(Tematica);
            var tareasSolicitudes = new List<Task>();
            foreach (var tupla in listaTuplaNombreIdEstadistica)
            {
                if (tupla.Item1.Equals(PrimerLugar.Nombre, StringComparison.OrdinalIgnoreCase))
                {
                    tareasSolicitudes.Add(EstadisticasDAO.AgregarEstadiscaPartidaAsync(tupla.Item2, accion, 1));

                }
            }
        }

        private EstadisticasAcciones SelecionarAccion(TematicaPartida tematica)
        {
            switch (tematica)
            {
                case TematicaPartida.Mitologia:
                    return EstadisticasAcciones.IncrementarPartidasMitologia;
                case TematicaPartida.Mixta:
                    return EstadisticasAcciones.IncrementarPartidaMixta;
                case TematicaPartida.Espacio:
                    return EstadisticasAcciones.IncrementarPartidaEspacio;
                case TematicaPartida.Animales:
                    return EstadisticasAcciones.IncrementarPartidaAnimales;
                case TematicaPartida.Paises:
                    return EstadisticasAcciones.IncrementarPartidaPaises;
                default:
                    return EstadisticasAcciones.IncrementarPartidaMixta;
            }
        }
    }
}
