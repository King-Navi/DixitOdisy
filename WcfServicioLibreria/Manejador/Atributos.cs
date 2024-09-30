using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class ManejadorPrincipal
    {
        #region Sala
        /// <summary>
        /// Diccionarion con el idSala, ademas de la sala.
        /// </summary>
        private static readonly ConcurrentDictionary<string, ISala> salasDiccionario = new ConcurrentDictionary<string, ISala>();
        #endregion Sala
        /// <summary>
        /// Diccioanrio de los jugadores conectados con el idJugador y el Callaback (el canal de comunicacion del jugador)
        /// </summary>
        #region JugadorSesion
        private static readonly ConcurrentDictionary<int, UsuarioContexto> jugadoresConetcadosDiccionario = new ConcurrentDictionary<int, UsuarioContexto>();

        #endregion JuagdorSesion
        /// <summary>
        /// 
        /// </summary>
        #region Chat
        private static readonly ConcurrentDictionary<string, Chat> chatDiccionario = new ConcurrentDictionary<string, Chat>();

        #endregion Chat

    }
}
