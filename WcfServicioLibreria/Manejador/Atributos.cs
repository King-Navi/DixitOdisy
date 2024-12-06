using ChatGPTLibreria;
using DAOLibreria;
using DAOLibreria.DAO;
using DAOLibreria.Interfaces;
using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class ManejadorPrincipal
    {
        #region Sala
        private static readonly ConcurrentDictionary<string, Sala> salasDiccionario = new ConcurrentDictionary<string, Sala>();
        private const string MOTIVO_EXPULSION_SALA = "Expulsion de sala";
        #endregion Sala

        #region JugadorSesion
        private static readonly ConcurrentDictionary<int, UsuarioContexto> jugadoresConectadosDiccionario = 
            new ConcurrentDictionary<int, UsuarioContexto>();
        #endregion JuagdorSesion

        #region Chat
        private static readonly ConcurrentDictionary<string, Chat> chatDiccionario = new ConcurrentDictionary<string, Chat>();
        #endregion Chat

        #region Partida
        private static readonly ConcurrentDictionary<string, ManejadorImagen> manejadoresImagenes= new ConcurrentDictionary<string, ManejadorImagen>();
        private static readonly ConcurrentDictionary<string, Partida> partidasDiccionario = new ConcurrentDictionary<string, Partida>();
        #endregion Partida

        #region Correo
        private readonly static ConcurrentDictionary<string, (string Codigo, DateTime Creacion)> codigosVerificacion = 
            new ConcurrentDictionary<string, (string Codigo, DateTime Creacion)>();
        private const int TIEMPO_EXPIRACION_CODIGO_SEGUNDOS = 1;
        private Timer eliminadorCodigos = new Timer(EliminarCodigosExpirados, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        private const string SERVIDOR_SMTP = "smtp.gmail.com";
        private const int PUERTO_SMTP = 587;
        private static readonly string CORREO_DESCRIBELO = "describeloproyecto@gmail.com";
        private static readonly string CONTRASENIA_CORREO_DESCRIBELO = ManejadorPrincipal.ObtenerContraseniaCorreo();

        private static string ObtenerContraseniaCorreo()
        {
            string contrasenia = Environment.GetEnvironmentVariable("CONTRASENIA_CORREO");
            if (string.IsNullOrEmpty(contrasenia))
            {
                throw new InvalidOperationException("La variable de entorno 'CONTRASENIA_CORREO' no está definida.");
            }
            return contrasenia;
        }
        #endregion Correo

        #region ContextoOperacion
        private readonly IContextoOperacion contextoOperacion;
        #endregion

        #region Escritura en disco
        public IEscribirDisco Escritor { get; private set; }


        #endregion

        #region DAO
        private readonly IVetoDAO vetoDAO;
        private readonly IUsuarioDAO usuarioDAO;
        private readonly IUsuarioCuentaDAO usuarioCuentaDAO;
        private readonly ISolicitudAmistadDAO peticionAmistadDAO;
        private readonly IExpulsionDAO expulsionDAO;
        private readonly IEstadisticasDAO estadisticasDAO;
        private readonly IAmistadDAO amistadDAO;
        #endregion DAO

        #region
        private const int ID_INVALIDO = 0;

        #endregion


        public ManejadorPrincipal(IContextoOperacion contextoOperacion, IVetoDAO vetoDAO, IUsuarioDAO usuarioDAO, IUsuarioCuentaDAO usuarioCuentaDAO, 
            ISolicitudAmistadDAO peticionAmistadDAO, IExpulsionDAO expulsionDAO, IEstadisticasDAO estadisticasDAO, IAmistadDAO amistadDAO)
        {
            this.contextoOperacion = contextoOperacion;
            this.vetoDAO = vetoDAO;
            this.usuarioDAO = usuarioDAO;
            this.usuarioCuentaDAO = usuarioCuentaDAO;
            this.peticionAmistadDAO = peticionAmistadDAO;
            this.expulsionDAO = expulsionDAO;
            this.estadisticasDAO = estadisticasDAO;
            this.amistadDAO = amistadDAO;
        }

        public ManejadorPrincipal()
        {
            contextoOperacion = new ContextoOperacion();
            Escritor = new EscritorDisco();
            vetoDAO = new VetoDAO();
            usuarioDAO = new UsuarioDAO();
            usuarioCuentaDAO = new UsuarioCuentaDAO();
            peticionAmistadDAO = new SolicitudAmistadDAO();
            expulsionDAO = new ExpulsionDAO();
            estadisticasDAO = new EstadisticasDAO();
            amistadDAO = new AmistadDAO();
        }
        public void CerrarAplicacion()
        {
            Task.Run(async () => await Escritor.DetenerAsync());
            EliminadorCadena.EliminarConnectionStringDelArchivo();
            eliminadorCodigos.Dispose();
        }


    }
}
