using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.ImplementacionesCallbacks
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public class SingletonPartida : IServicioPartidaSesionCallback
    {
        
        private static readonly Lazy<SingletonPartida> instancia = new Lazy<SingletonPartida>(() => new SingletonPartida());
        public static SingletonPartida Instancia => instancia.Value;
        public ServicioPartidaSesionClient Partida { get; set; }
        public Usuario PrimerLugar { get; private set; } = new Usuario();
        public Usuario SegundoLugar { get; private set; } = new Usuario();
        public Usuario TercerLugar { get; private set; } = new Usuario();
        public CollecionObservableSeguraHilos<JugadorPuntaje> JugadoresEstadisticas { get; set; } = new CollecionObservableSeguraHilos<JugadorPuntaje>();
        public CollecionObservableSeguraHilos<Usuario> UsuariosEnPartida { get; set; } = new CollecionObservableSeguraHilos<Usuario>();

        public event Action<int> CambiarPantalla;
        public event Action<bool> NotificarEsNarrador;
        public event Action<string> MostrarPista;
        public event Action EstadisticasEnviadas;
        public event Action DesbloquearChat;
        public event Action SeTerminoPartida;
        public event Action InicioPartida;


        private SingletonPartida() 
        {
            InicioPartida += PrepararseParaRonda;
        }



        public bool AbrirConexionPartida()
        {
            try
            {
                var objectoComunicacion = Partida as ICommunicationObject;
                if (objectoComunicacion?.State == CommunicationState.Opened ||
                    objectoComunicacion?.State == CommunicationState.Opening)
                {
                    return true;
                }
                if (objectoComunicacion?.State == CommunicationState.Faulted)
                {
                    CerrarConexionPartida();
                }
                SingletonGestorImagenes.Instancia.AbrirConexionImagenes();
                Partida = new ServicioPartidaSesionClient(new System.ServiceModel.InstanceContext(this));
                return true;
            }
            catch (Exception excepcion)
            {
                CerrarConexionPartida();
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                return false;
            }

        }

        public bool CerrarConexionPartida()
        {
            try
            {
                if (Partida != null)
                {
                    Partida.Close();
                    Partida = null;
                    return true;
                }
            }
            catch (CommunicationException excepcion)
            {
                Partida?.Abort();
                Partida = null;
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (TimeoutException excepcion)
            {
                Partida?.Abort();
                Partida = null;
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                Partida = null;
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            SingletonGestorImagenes.Instancia.CerrarConexionImagenes();
            return false;
        }

        public void NotificarNarradorCallback(bool esNarrador)
        {
            if (NotificarEsNarrador != null)
            {
                foreach (var solicitante in NotificarEsNarrador.GetInvocationList())
                {
                    try
                    {
                        ((Action<bool>)solicitante).Invoke(esNarrador);
                    }
                    catch (Exception excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                    }
                }
            }
        }

        public async void CambiarPantallaCallback(int numeroPantalla)
        {
            if (numeroPantalla == PantallasPartida.PANTALLA_INICIO)
            {
                SingletonGestorImagenes.Instancia.imagenesDeTodos.ImagenCartasTodos.Clear();
                MostrarPistaCallback(null);
            }
            if (numeroPantalla == PantallasPartida.PANTALLA_TODOS_CARTAS)
            {
                await SingletonGestorImagenes.Instancia.imagenesDeTodos.Imagen
                    .MostrarTodasImagenesAsync(SingletonCliente.Instance.IdPartida);
            }
            if (CambiarPantalla != null)
            {
                foreach (var solicitante in CambiarPantalla.GetInvocationList())
                {
                    try
                    {
                        ((Action<int>)solicitante).Invoke(numeroPantalla);
                    }
                    catch (Exception excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                    }
                }
            }
        }
        public async void PrepararseParaRonda()
        {
            await UnirseChat();
            await Partida.EmpezarPartidaAsync(SingletonCliente.Instance.NombreUsuario, 
                SingletonCliente.Instance.IdPartida);
        }

        public void EliminarJugadorPartidaCallback(Usuario jugardoreRetiradoDeSala)
        {
            try
            {
                var usuarioAEliminar = UsuariosEnPartida?.FirstOrDefault(busqueda => busqueda.Nombre == jugardoreRetiradoDeSala.Nombre);
                if (usuarioAEliminar != null)
                {
                    UsuariosEnPartida?.Remove(usuarioAEliminar);
                }
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }

        public void ObtenerJugadorPartidaCallback(Usuario jugardoreNuevoEnSala)
        {
            try
            {
                if (jugardoreNuevoEnSala == null)
                {
                    return;
                }
                else
                {
                    UsuariosEnPartida?.Add(jugardoreNuevoEnSala);
                }
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }

        public void EnviarEstadisticasCallback(EstadisticasPartida estadisticas)
        {
            Usuario usuarioPorDefecto = new Usuario()
            {
                Nombre = "",
                FotoUsuario = null
            };
            JugadoresEstadisticas = new CollecionObservableSeguraHilos<JugadorPuntaje>(estadisticas.Jugadores);
            PrimerLugar = UsuariosEnPartida.FirstOrDefault(busqueda => busqueda.Nombre == estadisticas.PrimerLugar?.Nombre) ?? new Usuario();
            SegundoLugar = UsuariosEnPartida.FirstOrDefault(busqueda => busqueda.Nombre == estadisticas.SegundoLugar?.Nombre) ?? new Usuario();
            TercerLugar = UsuariosEnPartida.FirstOrDefault(busqueda => busqueda.Nombre == estadisticas.TercerLugar?.Nombre) ?? new Usuario();
            EstadisticasEnviadas?.Invoke();
        }

        public void FinalizarPartidaCallback()
        {
            if (SeTerminoPartida != null)
            {
                foreach (var solicitante in SeTerminoPartida.GetInvocationList())
                {
                    try
                    {
                        ((Action)solicitante)?.Invoke();
                    }
                    catch (Exception excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                    }
                }
            }
        }

        public void IniciarValoresPartidaCallback(bool seUnio)
        {
            try
            {
                InicioPartida?.Invoke();
            }
            catch (Exception exccepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(exccepcion);
            }
        }
        private async Task UnirseChat()
        {
            SingletonGestorImagenes.Instancia.PeticionImagenesHilo();
            SingletonChat.Instancia.AbrirConexionChat();
            await SingletonChat.Instancia.ChatMotor.AgregarUsuarioChatAsync(SingletonCliente.Instance.IdChat,
                SingletonCliente.Instance.NombreUsuario);
            DesbloquearChat?.Invoke();
        }

        public void MostrarPistaCallback(string pista)
        {
            if (String.IsNullOrEmpty(pista))
            {
                pista = Properties.Idioma.labelEsperandoPista;
            }
            if (MostrarPista != null)
            {
                foreach (var solicitante in MostrarPista.GetInvocationList())
                {
                    try
                    {
                        ((Action<string>)solicitante)?.Invoke(pista);
                    }
                    catch (Exception excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                    }
                }
            }
        }
        public void TurnoPerdidoCallback()
        {
            throw new NotImplementedException();
        }
    }
}
