using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WpfCliente.Contexto;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.ImplementacionesCallbacks
{
    public class SingletonSalaJugador : IServicioSalaJugadorCallback, IImplementacionCallback
    {
        public event Action<bool> DelegacionRolAnfitrion;
        public event Action<string> EmepzarPartida;
        private static readonly Lazy<SingletonSalaJugador> instancia =
            new Lazy<SingletonSalaJugador>(() => new SingletonSalaJugador());
        public static SingletonSalaJugador Instancia => instancia.Value;
        public ServicioSalaJugadorClient Sala { get; private set; }
        public ObservableCollection<Usuario> JugadoresSala { get; private set; } = new ObservableCollection<Usuario>();

        private SingletonSalaJugador() { }
        public bool AbrirConexionInvitacionPartida()
        {
            try
            {
                var objectoComunicacion = Sala as ICommunicationObject;
                if (objectoComunicacion?.State == CommunicationState.Opened ||
                    objectoComunicacion?.State == CommunicationState.Opening)
                {
                    return true;
                }
                if (objectoComunicacion?.State == CommunicationState.Faulted)
                {
                    CerrarConexion();
                }
                Sala = new ServicioSalaJugadorClient(new System.ServiceModel.InstanceContext(this));
                LimpiarRecursos();
                return true;
            }
            catch (Exception excepcion)
            {
                CerrarConexion();
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
                return false;
            }

        }

        private void LimpiarRecursos()
        {
            JugadoresSala.Clear();
        }

        public bool CerrarConexion()
        {
            try
            {
                if (Sala != null)
                {
                    Sala.Close();
                    Sala = null;
                }
            }
            catch (Exception excepcion)
            {
                Sala = null;
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
                return false;
            }
            return true;
        }

        public void ObtenerJugadorSalaCallback(Usuario jugardoreNuevoEnSala)
        {
            if (jugardoreNuevoEnSala == null || 
                String.IsNullOrEmpty(jugardoreNuevoEnSala.Nombre) ||
                String.IsNullOrEmpty(SingletonCliente.Instance.NombreUsuario))
            {
                return;
            }
            JugadoresSala?.Add(jugardoreNuevoEnSala);
        }

        public void EliminarJugadorSalaCallback(Usuario jugardoreRetiradoDeSala)
        {
            if (jugardoreRetiradoDeSala == null || String.IsNullOrEmpty(jugardoreRetiradoDeSala.Nombre))
            {
                return;
            }
            var jugadorARemover = JugadoresSala?.FirstOrDefault(jugador => jugador.Nombre == jugardoreRetiradoDeSala.Nombre);
            if (jugadorARemover != null)
            {
                JugadoresSala.Remove(jugadorARemover);
            }
            if (jugardoreRetiradoDeSala.Nombre.Equals(SingletonCliente.Instance.NombreUsuario, StringComparison.OrdinalIgnoreCase))
            {
                SingletonGestorVentana.Instancia.Regresar();
            }
        }

        public void EmpezarPartidaCallback(string idPartida)
        {
            if (String.IsNullOrEmpty(idPartida))
                return;
            EmepzarPartida?.Invoke(idPartida);
        }

        public void DelegacionRolCallback(bool esAnfitrion)
        {
            if (esAnfitrion)
                DelegacionRolAnfitrion?.Invoke(esAnfitrion);
        }
    }
}
