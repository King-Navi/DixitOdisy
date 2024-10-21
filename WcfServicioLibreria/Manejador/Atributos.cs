using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class ManejadorPrincipal
    {
        #region Sala
        /// <summary>
        /// Diccionarion con el idSala, ademas de la sala.
        /// </summary>
        private static readonly ConcurrentDictionary<string, Sala> salasDiccionario = new ConcurrentDictionary<string, Sala>();
        #endregion Sala

        #region JugadorSesion
        /// <summary>
        /// Diccionario de los jugadores conectados con el idJugador y la clase abstracta del 
        /// modelo del usuario (en ella esta el canal de comunicacion de la sala de espera del jugador)
        /// </summary>
        private static readonly ConcurrentDictionary<int, UsuarioContexto> jugadoresConectadosDiccionario = new ConcurrentDictionary<int, UsuarioContexto>();

        #endregion JuagdorSesion
        #region Chat
        /// <summary>
        ///  Diccionario que guarda la clave idChat (equivalentes al idSala) y el modelo del chat con sus jugadores.
        /// </summary>
        private static readonly ConcurrentDictionary<string, Chat> chatDiccionario = new ConcurrentDictionary<string, Chat>();



        #endregion Chat

        #region Inyeccion de depdendencias
        private readonly IContextoOperacion contextoOperacion;

        // Inyección de dependencias 
        public ManejadorPrincipal(IContextoOperacion _contextoOperacion)
        {
            this.contextoOperacion = _contextoOperacion;
        }   
        public ManejadorPrincipal()
        {
            contextoOperacion = new ContextoOperacion();
        }
        #endregion
    }
}
