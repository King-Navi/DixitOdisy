using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class Sala : ISala
    {
        #region Campos
        private string idCodigoSala;
        private string anfitrion;
        private const int cantidadMinimaJugadores = 3;
        private const int cantidadMaximaJugadores = 12;
        private ConcurrentDictionary<string, ISalaJugadorCallback> jugadoresSala = new ConcurrentDictionary<string, ISalaJugadorCallback>();
        #endregion Campos
        #region Propiedades
        public static int CantidadMaximaJugadores => cantidadMaximaJugadores;

        public static int CantidadMinimaJugadores => cantidadMinimaJugadores;

        string ISala.IdCodigoSala => throw new NotImplementedException();

        string ISala.Anfitrion => throw new NotImplementedException();

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

        bool ISala.EsVacia()
        {
            return jugadoresSala.IsEmpty;
        }

        IReadOnlyCollection<string> ISala.ObtenerNombresJugadoresSala()
        {
            return jugadoresSala.Keys.ToList().AsReadOnly();
        }

        bool ISala.AgregarJugadorSala(string nombreJugador, ISalaJugadorCallback nuevoContexto)
        {
            jugadoresSala.AddOrUpdate(nombreJugador, nuevoContexto, (key, oldValue) => nuevoContexto);
            if (jugadoresSala.TryGetValue(nombreJugador, out ISalaJugadorCallback contextoCambiado))
            {
                if (ReferenceEquals(nuevoContexto, contextoCambiado))
                {
                    return true;
                }
            }
            return false;
        }

        bool ISala.RemoverJugadorSala(string nombreJugador)
        {
            throw new NotImplementedException();
        }

        bool ISala.DelegarRolAnfitrion(string nuevoAnfitrionNombre)
        {
            bool existeJugador = jugadoresSala.TryGetValue(nuevoAnfitrionNombre, out _);
            if (!existeJugador)
            {
                return false;
            }
            anfitrion = nuevoAnfitrionNombre;
            return anfitrion == nuevoAnfitrionNombre;
        }
        #endregion Metodos
    }
}
