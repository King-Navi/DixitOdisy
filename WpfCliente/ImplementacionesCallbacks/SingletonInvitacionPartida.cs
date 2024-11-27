using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;
using WpfCliente.Interfaz;
using System.ServiceModel;

namespace WpfCliente.ImplementacionesCallbacks
{
    public class SingletonInvitacionPartida : IServicioInvitacionPartidaCallback , IImplementacionCallback
    {
        public event Action<InvitacionPartida> InvitacionRecibida;
        private static readonly Lazy<SingletonInvitacionPartida> instancia =
            new Lazy<SingletonInvitacionPartida>(() => new SingletonInvitacionPartida());
        public static SingletonInvitacionPartida Instancia => instancia.Value;
        public ServicioInvitacionPartidaClient InvitacionPartida { get; private set; }

        private SingletonInvitacionPartida() { }
        public void RecibirInvitacionCallback(InvitacionPartida invitacion)
        {
            if (invitacion == null)
                return;
            if (String.IsNullOrEmpty(invitacion.CodigoSala) || String.IsNullOrEmpty(invitacion.GamertagEmisor))
                return;
            InvitacionRecibida?.Invoke(invitacion);
        }

        public bool AbrirConexion()
        {
            try
            {
                var objectoComunicacion = InvitacionPartida as ICommunicationObject;
                if (objectoComunicacion?.State == CommunicationState.Opened ||
                    objectoComunicacion?.State == CommunicationState.Opening)
                {
                    return true;
                }
                if (objectoComunicacion?.State == CommunicationState.Faulted)
                {
                    CerrarConexion();
                }
                InvitacionPartida = new ServicioInvitacionPartidaClient(new System.ServiceModel.InstanceContext(this));
                return true;

            }
            catch (Exception excepcion)
            {
                CerrarConexion();
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
                return false;
            }
        }

        public bool CerrarConexion()
        {
            try
            {
                if (InvitacionPartida != null)
                {
                    InvitacionPartida.Close();
                    InvitacionPartida = null;
                }
            }
            catch (Exception excepcion)
            {
                InvitacionPartida = null;
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
                return false;
            }
            return true;


        }
    }
}
