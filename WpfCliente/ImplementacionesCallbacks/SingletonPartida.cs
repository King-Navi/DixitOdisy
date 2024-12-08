using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using WpfCliente.Contexto;
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
        public CollecionObservableSeguraHilos<Usuario> UsuariosEnPartida { get; set; } = new CollecionObservableSeguraHilos<Usuario>();
        public event Action<int> CambiarPantalla;
        public event Action<bool> NotificarEsNarrador;
        public event Action<string> MostrarPista;
        public event Action<List<JugadorTablaPuntaje>> SeActualizoPuntajes;
        public event Action EstadisticasEnviadas;
        public event Action DesbloquearChat;
        public event Action SeTerminoPartida;
        public event Action PerdisteTurno;
        public event Action TeHanExpulsado;
        public event Action PartidaFaltaJugadores;

        private SingletonPartida() { }

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
                    Partida.Abort();
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

        public void CambiarPantallaCallback(int numeroPantalla)
        {
            if (numeroPantalla == PantallasPartida.PANTALLA_INICIO)
            {
                SingletonGestorImagenes.Instancia.imagenesTablero.ImagenesTablero.Clear();
                MostrarPistaCallback(null);
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

        public void EliminarJugadorPartidaCallback(Usuario jugardoreRetiradoDeSala)
        {
            try
            {
                if (jugardoreRetiradoDeSala == null || String.IsNullOrEmpty(jugardoreRetiradoDeSala.Nombre))
                {
                    return;
                }
                UsuariosEnPartida?.EliminarElementoPorCondicion(busqueda => busqueda.Nombre == jugardoreRetiradoDeSala.Nombre);
                
                if (jugardoreRetiradoDeSala.Nombre.Equals(SingletonCliente.Instance.NombreUsuario, StringComparison.OrdinalIgnoreCase))
                {
                    TeHanExpulsado?.Invoke();
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

        public void EnviarEstadisticasCallback(EstadisticasPartida estadisticas, bool esAnfitrion)
        {
            try
            {
                var listaModeloServidor = new List<JugadorPuntaje>(estadisticas.Jugadores);
                var listaModeloCliente = new List<JugadorTablaPuntaje>();
                List<JugadorTablaPuntaje> listaResultado = JugadorPuntajeConvertidor.ConvertirAListaJugadorTablaPuntaje(listaModeloServidor, esAnfitrion);
                PrimerLugar = UsuariosEnPartida.FirstOrDefault(busqueda => busqueda.Nombre == estadisticas.PrimerLugar?.Nombre) ?? new Usuario();
                SegundoLugar = UsuariosEnPartida.FirstOrDefault(busqueda => busqueda.Nombre == estadisticas.SegundoLugar?.Nombre) ?? new Usuario();
                TercerLugar = UsuariosEnPartida.FirstOrDefault(busqueda => busqueda.Nombre == estadisticas.TercerLugar?.Nombre) ?? new Usuario();
                EstadisticasEnviadas?.Invoke();
                SeActualizoPuntajes?.Invoke(listaResultado);
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
                Task.Run(async () =>
                {
                    try
                    {
                        await UnirseChat();
                        await Partida.EmpezarPartidaAsync(SingletonCliente.Instance.IdPartida);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                });
            }
            catch (Exception exccepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(exccepcion);
            }
        }

        private async Task UnirseChat()
        {
            try
            {
                SingletonGestorImagenes.Instancia.PeticionImagenesHilo();
                SingletonChat.Instancia.AbrirConexionChat();
                await SingletonChat.Instancia.ChatMotor.AgregarUsuarioChatAsync(
                    SingletonCliente.Instance.IdChat,
                    SingletonCliente.Instance.NombreUsuario);
                DesbloquearChat?.Invoke();
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
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
            try
            {
                PerdisteTurno?.Invoke();
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }

        public void NoSeEmpezoPartidaCallback()
        {
            try
            {

                    PartidaFaltaJugadores?.Invoke();
                
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }

        public void MostrarTableroCartasCallback()
        {
            try
            {
                Task.Run(async() => 
                {
                    try
                    {
                        await SingletonGestorImagenes.Instancia.imagenesTablero.Imagen.MostrarImagenesTableroAsync(SingletonCliente.Instance.IdPartida);

                    }
                    catch (CommunicationException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                });
                return;
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }
    }
}
