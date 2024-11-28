using ChatGPTLibreria;
using DAOLibreria;
using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading;
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
        private static readonly ConcurrentDictionary<int, UsuarioContexto> jugadoresConectadosDiccionario = 
            new ConcurrentDictionary<int, UsuarioContexto>();
        #endregion JuagdorSesion
        #region Chat
        private static readonly ConcurrentDictionary<string, Chat> chatDiccionario = new ConcurrentDictionary<string, Chat>();
        #endregion Chat
        #region Partida
        private static readonly ConcurrentDictionary<string, Partida> partidasdDiccionario = new ConcurrentDictionary<string, Partida>();
        #endregion Partida

        #region Correo
        private readonly static ConcurrentDictionary<string, (string Codigo, DateTime Creacion)> codigosVerificacion = 
            new ConcurrentDictionary<string, (string Codigo, DateTime Creacion)>();
        private const int TIEMPO_EXPIRACION_CODIGO_SEGUNDOS = 1;
        private Timer eliminadorCodigos = new Timer(EliminarCodigosExpirados, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        private const string SERVIDOR_SMTP = "smtp.gmail.com";
        private const int PUERTO_SMTP = 587;
        private static readonly string CORREO = "describeloproyecto@gmail.com";
        private static readonly string CONTRASENIA = "rbyu noyd vebq adwe";
        #endregion Correo

        #region ContextoOperacion
        private readonly IContextoOperacion contextoOperacion;
        #endregion

        #region Escritura en disco
        public IEscribirDisco Escritor { get; private set; }


        #endregion

        #region Amistad
        private readonly SemaphoreSlim semaphorAbrirSesionAmigo= new SemaphoreSlim(1, 1);

        #endregion

        public ManejadorPrincipal(IContextoOperacion _contextoOperacion, IEscribirDisco _escribirDisco)
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
        public void CerrarAplicacion()
        {
            Console.WriteLine("Guardando las ultimas imagenes...");
            Task.Run(async () => await Escritor.DetenerAsync());
            EliminadorCadena.EliminarConnectionStringDelArchivo();
            eliminadorCodigos.Dispose();
        }


    }
}
