using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.ImplementacionesCallbacks
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SingletonPartida : IServicioPartidaSesionCallback
    {
        private static readonly Lazy<SingletonPartida> instancia = new Lazy<SingletonPartida>(() => new SingletonPartida());
        public static SingletonPartida Instancia => instancia.Value;
        public static ServicioPartidaSesionClient Partida { get; private set; }

        private SingletonPartida() { }


        public bool AbrirConexionChat()
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
                Partida = new ServicioPartidaSesionClient(new System.ServiceModel.InstanceContext(this));
                return true;
            }
            catch (Exception excepcion)
            {
                CerrarConexionPartida();
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
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
                }
            }
            catch (Exception excepcion)
            {
                Partida = null;
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
                return false;
            }
            return true;
        }
        public void CambiarPantallaCallback(int numeroPantalla)
        {
            throw new NotImplementedException();
        }

        public void EliminarJugadorPartidaCallback(Usuario jugardoreRetiradoDeSala)
        {
            throw new NotImplementedException();
        }

        public void EnviarEstadisticas(EstadisticasPartida estadisticas)
        {
            throw new NotImplementedException();
        }

        public void FinalizarPartida()
        {
            throw new NotImplementedException();
        }

        public void IniciarValoresPartidaCallback(bool seUnio)
        {
            throw new NotImplementedException();
        }

        public void MostrarPistaCallback(string pista)
        {
            throw new NotImplementedException();
        }

        public void NotificarNarradorCallback(bool esNarrador)
        {
            throw new NotImplementedException();
        }

        public void ObtenerJugadorPartidaCallback(Usuario jugardoreNuevoEnSala)
        {
            throw new NotImplementedException();
        }

        public void RecibirGrupoImagenCallback(ImagenCarta imagen)
        {
            throw new NotImplementedException();
        }

        public void RecibirImagenCallback(ImagenCarta imagen)
        {
            throw new NotImplementedException();
        }

        public void TurnoPerdidoCallback()
        {
            throw new NotImplementedException();
        }
    }
}
