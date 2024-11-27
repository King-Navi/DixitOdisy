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
        private static readonly ConcurrentDictionary<string, Sala> salasDiccionario = new ConcurrentDictionary<string, Sala>();
        #endregion Sala

        #region JugadorSesion
        private static readonly ConcurrentDictionary<int, UsuarioContexto> jugadoresConectadosDiccionario = new ConcurrentDictionary<int, UsuarioContexto>();
        #endregion JuagdorSesion

        #region Chat
        private static readonly ConcurrentDictionary<string, Chat> chatDiccionario = new ConcurrentDictionary<string, Chat>();
        #endregion Chat

        #region Partida
        private static readonly ConcurrentDictionary<string, Partida> partidasdDiccionario = new ConcurrentDictionary<string, Partida>();
        #endregion Partida

        #region Correo
        private static readonly string CORREO_DESCRIBELO = "describeloproyecto@gmail.com";
        private static readonly string CONTRASENIA_CORREO_DESCRIBELO = ManejadorPrincipal.ObtenerContraseniaCorreo();
        #endregion Correo

        #region Inyeccion de dependencias
        private readonly IContextoOperacion contextoOperacion;

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

    }
}
