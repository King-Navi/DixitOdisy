using DAOLibreria.DAO;
using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private const int VICTORIA = 1;
        private const int DERROTA = 0;
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
            // Ordenar los jugadores por puntos de forma descendente
            var jugadoresOrdenados = Jugadores.OrderByDescending(jugador => jugador.Puntos).ToList();

            // Asignar primer lugar si hay al menos un jugador
            PrimerLugar = jugadoresOrdenados.FirstOrDefault();

            // Encontrar y asignar el segundo lugar (distinto del primer lugar en caso de empate)
            SegundoLugar = jugadoresOrdenados.Skip(1).FirstOrDefault(jugador => jugador.Puntos != PrimerLugar?.Puntos);

            // Encontrar y asignar el tercer lugar (distinto del primer y segundo lugar en caso de empate)
            TercerLugar = jugadoresOrdenados.Skip(2).FirstOrDefault(jugador =>
                jugador.Puntos != PrimerLugar?.Puntos && jugador.Puntos != SegundoLugar?.Puntos);
        }

        internal async Task GuardarPuntajeAsync(List<Tuple<String , int >> listaTuplaNombreIdEstadistica)
        {
            var accion = SelecionarAccion(Tematica);
            var tareasSolicitudes = new List<Task>();
            foreach (var tupla in listaTuplaNombreIdEstadistica)
            {
                if (tupla.Item1.Equals(PrimerLugar.Nombre, StringComparison.OrdinalIgnoreCase))
                {
                    tareasSolicitudes.Add(EstadisticasDAO.AgregarEstadiscaPartidaAsync(tupla.Item2, accion, VICTORIA));

                }
                else
                {
                    tareasSolicitudes.Add(EstadisticasDAO.AgregarEstadiscaPartidaAsync(tupla.Item2, accion, DERROTA));
                }
            }
            await Task.WhenAll(tareasSolicitudes);

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

        public void AgregarDesdeOtraLista(IReadOnlyCollection<string> usuariosEnPartida)
        {
            if (usuariosEnPartida == null || usuariosEnPartida.Count == 0)
            {
                throw new ArgumentException("La lista de usuarios en partida está vacía o es nula.");
            }

            foreach (var usuario in usuariosEnPartida)
            {
                var jugadorExistente = Jugadores.FirstOrDefault(j => j.Nombre == usuario);
                if (jugadorExistente == null)
                {
                    Jugadores.Add(new JugadorEstadisticas(usuario));
                }
            }
        }
    }
}
