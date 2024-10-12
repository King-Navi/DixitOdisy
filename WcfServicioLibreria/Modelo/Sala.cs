using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class Sala : IObservadorSala
    {
        #region Campos
        private string idCodigoSala;
        private string anfitrion;
        private const int cantidadMinimaJugadores = 3;
        private const int cantidadMaximaJugadores = 12;
        private readonly ConcurrentDictionary<string, ISalaJugadorCallback> jugadoresSala = new ConcurrentDictionary<string, ISalaJugadorCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        #endregion Campos
        #region Propiedades
        public static int CantidadMaximaJugadores => cantidadMaximaJugadores;
        public static int CantidadMinimaJugadores => cantidadMinimaJugadores;
        #endregion Propiedades

        #region Contructores
        public Sala(string _idCodigoSala, string nombreUsuario)
        {
            this.idCodigoSala = _idCodigoSala;
            this.anfitrion = nombreUsuario;
            jugadoresSala.TryAdd(nombreUsuario, null);
        }

        #endregion Contructores

        #region Metodos
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
            eventosJugador.DesuscribirTodos();
            if (ContarJugadores() == 0)
            {
                EliminarSala();
            }
            return seElimino;
        }
        private void EliminarSala() 
        { 
            
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

        void IObservadorSala.Desconectar(string clave)
        {
            RemoverJugadorSala(clave);
        }
        #endregion Metodos
    }
}
