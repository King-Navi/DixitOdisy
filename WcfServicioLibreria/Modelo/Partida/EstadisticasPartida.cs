using DAOLibreria.DAO;
using DAOLibreria.Excepciones;
using DAOLibreria.Interfaces;
using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Modelo.Vetos;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    [KnownType(typeof(TematicaPartida))]
    [KnownType(typeof(List<JugadorPuntaje>))]
    public class EstadisticasPartida
    {
        private const int VICTORIA = 1;
        private const int DERROTA = 0;
        private const int CERO_RONDAS = 0;
        [DataMember]
        public TematicaPartida Tematica { get; private set; }
        [DataMember]
        public List<JugadorPuntaje> Jugadores { get; set; } 
        [DataMember]
        public int TotalRondas { get; set; } 
        [DataMember]
        public JugadorPuntaje PrimerLugar { get; set; }
        [DataMember]
        public JugadorPuntaje SegundoLugar { get; set; }
        [DataMember]
        public JugadorPuntaje TercerLugar { get; set; }
        private ManejadorDeVetos ManejadorVetos { get; set; } = new ManejadorDeVetos();
        private IEstadisticasDAO estadisticasDAO;
        public EstadisticasPartida(TematicaPartida tematica)
        {
            Tematica = tematica;
            Jugadores = new List<JugadorPuntaje>();
            TotalRondas = CERO_RONDAS;
            PrimerLugar = null;
            SegundoLugar = null;
            TercerLugar = null;
            estadisticasDAO = new EstadisticasDAO();
        }
        public EstadisticasPartida(TematicaPartida tematica, IEstadisticasDAO _estadisticasDAO)
        {
            Tematica = tematica;
            Jugadores = new List<JugadorPuntaje>();
            TotalRondas = CERO_RONDAS;
            PrimerLugar = null;
            SegundoLugar = null;
            TercerLugar = null;
            estadisticasDAO = _estadisticasDAO;
        }
        public void CalcularPodio()
        {
            var jugadoresOrdenados = Jugadores
                .OrderByDescending(jugador => jugador.Puntos)
                .ToList();

            PrimerLugar = jugadoresOrdenados.ElementAtOrDefault(0);
            SegundoLugar = jugadoresOrdenados.ElementAtOrDefault(1);
            TercerLugar = jugadoresOrdenados.ElementAtOrDefault(2);
        }

        public async Task GuardarPuntajeAsync(List<Tuple<String , int >> listaTuplaNombreIdEstadistica)
        {
            if (listaTuplaNombreIdEstadistica == null)
            {
                return;
            }
            CalcularPodio();
            var accion = SelecionarAccion(Tematica);
            var tareasSolicitudes = new List<Task>();
            foreach (var tupla in listaTuplaNombreIdEstadistica)
            {
                try
                {
                    if (tupla.Item1.Equals(PrimerLugar.Nombre, StringComparison.OrdinalIgnoreCase))
                    {
                        tareasSolicitudes.Add(estadisticasDAO.AgregarEstadiscaPartidaAsync(tupla.Item2, accion, VICTORIA));

                    }
                    else
                    {
                        tareasSolicitudes.Add(estadisticasDAO.AgregarEstadiscaPartidaAsync(tupla.Item2, accion, DERROTA));
                    }
                }
                catch (ActividadSospechosaExcepcion excepcion)
                {
                    await ManejadorVetos.VetaJugadorAsync(tupla.Item1);
                    ManejadorExcepciones.ManejarExcepcionError(excepcion);

                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
                throw new ArgumentException(nameof(AgregarDesdeOtraLista));
            }

            foreach (var usuario in usuariosEnPartida)
            {
                var jugadorExistente = Jugadores.FirstOrDefault(busqueda => busqueda.Nombre == usuario);
                if (jugadorExistente == null)
                {
                    Jugadores.Add(new JugadorPuntaje(usuario));
                }
            }
        }
    }
}
