using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Utilidades;
namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class Sala : IObservador
    {
        #region Campos
        private string idCodigoSala;
        private string anfitrion;
        private const int cantidadMinimaJugadores = 3;
        private const int cantidadMaximaJugadores = 12;
        private readonly ConcurrentDictionary<string, ISalaJugadorCallback> jugadoresSala = new ConcurrentDictionary<string, ISalaJugadorCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        public EventHandler salaVaciaManejadorEvento;
        #endregion Campos
        #region Propiedades
        public static int CantidadMaximaJugadores => cantidadMaximaJugadores;
        public static int CantidadMinimaJugadores => cantidadMinimaJugadores;

        public string IdCodigoSala { get => idCodigoSala; internal set => idCodigoSala = value; }
        #endregion Propiedades

        #region Contructores
        public Sala(string _idCodigoSala, string nombreUsuario)
        {
            this.IdCodigoSala = _idCodigoSala;
            this.anfitrion = nombreUsuario;
            jugadoresSala.TryAdd(nombreUsuario, null);
        }

        #endregion Contructores

        #region Metodos
        private void EnSalaVacia()
        {
            salaVaciaManejadorEvento?.Invoke(this, new SalaVaciaEventArgs(DateTime.Now, this));
        }
        private int ContarJugadores()
        {
            return jugadoresSala.Count;
        }

        bool EsVacia()
        {
            return jugadoresSala.IsEmpty;
        }

        IReadOnlyCollection<string> ObtenerNombresJugadoresSala()
        {
            return jugadoresSala.Keys.ToList().AsReadOnly();
        }

        public bool AgregarJugadorSala(string nombreJugador, ISalaJugadorCallback nuevoContexto)
        {
            bool resultado = false;
            if (ContarJugadores() < cantidadMaximaJugadores)
            {
                jugadoresSala.AddOrUpdate(nombreJugador, nuevoContexto, (key, oldValue) => nuevoContexto);
                if (jugadoresSala.TryGetValue(nombreJugador, out ISalaJugadorCallback contextoCambiado))
                {
                    if (ReferenceEquals(nuevoContexto, contextoCambiado))
                    {
                        eventosCommunication.TryAdd(nombreJugador, new DesconectorEventoManejador((ICommunicationObject)contextoCambiado, this, nombreJugador));
                        resultado = true;
                    }
                }
            }
            return resultado;
        }

        bool RemoverJugadorSala(string nombreJugador)
        {
            bool seElimino = jugadoresSala.TryRemove(nombreJugador, out ISalaJugadorCallback jugadorEliminado);
            eventosCommunication.TryGetValue(nombreJugador, out DesconectorEventoManejador eventosJugador);
            eventosJugador.Desechar();
            if (ContarJugadores() == 0)
            {
                EliminarSala();
            }
            return seElimino;
        }
        private void EliminarSala() 
        {
            IReadOnlyCollection<string> claveEventos = ObtenerNombresJugadoresSala();
            foreach (var clave in claveEventos)
            {
                if (eventosCommunication.ContainsKey(clave))
                {
                    eventosCommunication[clave].Desechar();
                }
            }
            eventosCommunication.Clear();
            IReadOnlyCollection<string> claveJugadores = ObtenerNombresJugadoresSala();
            foreach (var clave in claveJugadores)
            {
                if (jugadoresSala.ContainsKey(clave))
                {
                    ((ICommunicationObject)jugadoresSala[clave]).Close();
                }
            }
            jugadoresSala.Clear();
            EnSalaVacia();

        }

        bool DelegarRolAnfitrion(string nuevoAnfitrionNombre)
        {
            bool existeJugador = jugadoresSala.TryGetValue(nuevoAnfitrionNombre, out _);
            if (!existeJugador)
            {
                return false;
            }
            anfitrion = nuevoAnfitrionNombre;
            return anfitrion == nuevoAnfitrionNombre;
        }

        void IObservador.DesconectarUsuario(string clave)
        {
            RemoverJugadorSala(clave);
        }
        #endregion Metodos
    }
}
