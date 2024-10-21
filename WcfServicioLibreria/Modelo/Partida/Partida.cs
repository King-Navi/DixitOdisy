using System.Collections.Concurrent;
using System;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Enumerador;

namespace WcfServicioLibreria.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    /// <ref>https://refactoring.guru/es/design-patterns/strategy</ref>
    public class Partida //FIXME: Faltan muchas funcionalidades y pruebas
    {
        #region Atributos
        private const int cantidadMaximaJugadores = 12;
        private ConcurrentDictionary<string, IPartidaCallabck> jugadores = new ConcurrentDictionary<string, IPartidaCallabck>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        private readonly ICondicionVictoria condicionVictoria;
        public EventHandler EliminarChatManejadorEvento;
        #endregion Atributos
        public ConfiguracionPartida Configuracion { get; private set; }

        public string IdPartida { get; private set; }
        public string Anfitrion { get; private set; }
        public int Ronda { get; private set; }
        public int CartasRestantes { get; private set; }
        #region
        public Partida(string _idPartida, string _anfitrion, ConfiguracionPartida _configuracion)
        {
            IdPartida = _idPartida;
            Anfitrion = _anfitrion;
            condicionVictoria = CrearCondicionVictoria(_configuracion.Condicion);
        }
        #endregion
        #region Metodos
        private ICondicionVictoria CrearCondicionVictoria(CondicionVictoriaPartida condicionVictoria)
        {
            switch (condicionVictoria)
            {
                case CondicionVictoriaPartida.PorCantidadRondas:
                    return new CondicionVictoriaPorRondas(Configuracion.NumeroRondas);
                case CondicionVictoriaPartida.PorCartasAgotadas:
                    return new CondicionVictoriaCartasAgotadas();
                default:
                    throw new ArgumentException("Condición de victoria no válida");
            }
        }
        private void CambiarRonda()
        {
            if (!VerificarCondicionVictoria())
            {
                TerminarPartida();
            }
            else
            {
                ++Ronda;   
            }
        }

        private void TerminarPartida()
        {
            throw new NotImplementedException();
        }

        private bool VerificarCondicionVictoria()
        {
            return condicionVictoria.Verificar(this);
        }
        #endregion

        #region
        #endregion

    }



}

