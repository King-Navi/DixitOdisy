using System.Collections.Concurrent;
using System;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Enumerador;
using System.ServiceModel;
using WcfServicioLibreria.Utilidades;
using System.Collections.Generic;
using System.Linq;

namespace WcfServicioLibreria.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    /// <ref>https://refactoring.guru/es/design-patterns/strategy</ref>
    internal class Partida : IObservador//FIXME: Faltan muchas funcionalidades y pruebas
    {
        #region Atributos
        private ConcurrentDictionary<string, IPartidaCallback> jugadoresCallback = new ConcurrentDictionary<string, IPartidaCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        private ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario> jugadoresInformacion = new ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario>();
        private readonly ICondicionVictoria condicionVictoria;
        public ConfiguracionPartida Configuracion { get; private set; }
        public EventHandler partidaVaciaManejadorEvento;
        #endregion Atributos

        #region Propiedad
        public string IdPartida { get; private set; }
        public string Anfitrion { get; private set; }
        public int RondaActual { get; private set; }
        public int CartasRestantes { get; private set; }
        #endregion Propiedad

        #region
        public Partida(string _idPartida, string _anfitrion, ConfiguracionPartida _configuracion)
        {
            IdPartida = _idPartida;
            Anfitrion = _anfitrion;
            condicionVictoria = CrearCondicionVictoria(_configuracion);
        }
        #endregion
        #region Metodos
        private void EnPartidaVacia()
        {
            partidaVaciaManejadorEvento?.Invoke(this, new PartidaVaciaEventArgs(DateTime.Now, this));
        }
        private ICondicionVictoria CrearCondicionVictoria(ConfiguracionPartida condicionVictoria)
        {
            switch (condicionVictoria.Condicion)
            {
                case CondicionVictoriaPartida.PorCantidadRondas:
                    return new CondicionVictoriaPorRondas(condicionVictoria.NumeroRondas);
                case CondicionVictoriaPartida.PorCartasAgotadas:
                    return new CondicionVictoriaCartasAgotadas();
                default:
                    throw new ArgumentException("Condición de victoria no válida");
            }
        }
        private void CambiarRonda()
        {
            if (VerificarCondicionVictoria())
            {
                TerminarPartida();
            }
            else
            {
                //TODO:Seguir juando
                ++RondaActual;   
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

        internal bool AgregarJugador(string nombreJugador, IPartidaCallback nuevoContexto) //FIXME
        {
            bool resultado = false;
                jugadoresCallback.AddOrUpdate(nombreJugador, nuevoContexto, (key, oldValue) => nuevoContexto);
                if (jugadoresCallback.TryGetValue(nombreJugador, out IPartidaCallback contextoCambiado))
                {
                    if (ReferenceEquals(nuevoContexto, contextoCambiado))
                    {
                        eventosCommunication.TryAdd(nombreJugador, new DesconectorEventoManejador((ICommunicationObject)contextoCambiado, this, nombreJugador));
                        resultado = true;
                    }
                }
            return resultado;
        }

        internal void AvisarNuevoJugador(string gamertag)
        {
            //TODO:
        }
        private void AvisarRetiroJugador(string nombreUsuarioEliminado)
        {
            //TODO:
        }

        public void DesconectarUsuario(string nombreJugador)
        {
            AvisarRetiroJugador(nombreJugador);
            RemoverJugador(nombreJugador);
        }

        private void RemoverJugador(string nombreJugador)
        {
             jugadoresCallback.TryRemove(nombreJugador, out IPartidaCallback _);
            eventosCommunication.TryRemove(nombreJugador, out DesconectorEventoManejador eventosJugador);
            jugadoresInformacion.TryRemove(nombreJugador, out _ );
            eventosJugador.Desechar();
            if (ContarJugadores() == 0)
            {
                EliminarPartida();
            }
        }

        private void EliminarPartida()
        {
            IReadOnlyCollection<string> claveEventos = ObtenerNombresJugadores();
            foreach (var clave in claveEventos)
            {
                if (eventosCommunication.ContainsKey(clave))
                {
                    eventosCommunication[clave].Desechar();
                }
            }
            eventosCommunication.Clear();
            IReadOnlyCollection<string> claveJugadores = ObtenerNombresJugadores();
            foreach (var clave in claveJugadores)
            {
                if (jugadoresCallback.ContainsKey(clave))
                {
                    ((ICommunicationObject)jugadoresCallback[clave]).Close();
                }
            }
            jugadoresCallback.Clear();
            EnPartidaVacia();
        }

        private IReadOnlyCollection<string> ObtenerNombresJugadores()
        {
            return jugadoresCallback.Keys.ToList().AsReadOnly();
        }

        private int ContarJugadores()
        {
            return jugadoresCallback.Count;
        }
        #endregion

    }



}

