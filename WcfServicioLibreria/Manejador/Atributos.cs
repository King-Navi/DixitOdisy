using ChatGPTLibreria;
using DAOLibreria;
using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
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

        #region Partida
        private static readonly ConcurrentDictionary<string, Partida> partidasdDiccionario = new ConcurrentDictionary<string, Partida>();

        #endregion Partida

        #region Correo
        private static readonly string correo = "describeloproyecto@gmail.com";
        private static readonly string contrasenia = "rbyu noyd vebq adwe";

        #endregion Correo

        #region Inyeccion de depdendencias
        private readonly IContextoOperacion contextoOperacion;

        // Inyección de dependencias 
        public ManejadorPrincipal(IContextoOperacion _contextoOperacion , IEscribirDisco _escribirDisco)
        {
            this.contextoOperacion = _contextoOperacion;
            Escritor = _escribirDisco; 

        }
        public ManejadorPrincipal(IContextoOperacion _contextoOperacion)
        {
            this.contextoOperacion = _contextoOperacion;
            Escritor = new EscritorDisco();

        }
        public ManejadorPrincipal()
        {
            contextoOperacion = new ContextoOperacion();
            Escritor = new EscritorDisco(); 

        }


        #endregion
        #region Escritura en disco
        public IEscribirDisco Escritor { get; private set; }

        public void CerrarAplicacion()
        {
            Console.WriteLine("Guardando las ultimas imagenes...");
            Task.Run(async()=> await Escritor.DetenerAsync());
            EliminadorCadena.EliminarConnectionStringDelArchivo();
            
        }
        #endregion
        #region PartidaSesion

        #endregion 
    }
}
