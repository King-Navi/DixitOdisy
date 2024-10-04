using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class ManejadorPrincipal
    {
        #region Sala
        /// <summary>
        /// Diccionarion con el idSala, ademas de la sala.
        /// </summary>
        private static readonly ConcurrentDictionary<string, ISala> salasDiccionario = new ConcurrentDictionary<string, ISala>();
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


    }
}
