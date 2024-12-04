using System;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;
using System.ServiceModel;

namespace WpfCliente.ImplementacionesCallbacks
{
    public partial class SingletonCanal : IServicioInvitacionPartidaCallback 
    {
        public event Action<InvitacionPartida> InvitacionRecibida;
        public ServicioInvitacionPartidaClient InvitacionPartida { get; private set; }
        public void RecibirInvitacionCallback(InvitacionPartida invitacion)
        {
            if (invitacion == null)
                return;
            if (String.IsNullOrEmpty(invitacion.CodigoSala) || String.IsNullOrEmpty(invitacion.NombreEmisor))
                return;
            InvitacionRecibida?.Invoke(invitacion);
        }

        public bool AbrirConexionInvitacionParitda()
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
                    CerrarConexionInvitacion();
                }
                InvitacionPartida = new ServicioInvitacionPartidaClient(new System.ServiceModel.InstanceContext(this));
                return true;
            }
            catch (Exception excepcion)
            {
                CerrarConexionInvitacion();
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                return false;
            }

        }

        public bool CerrarConexionInvitacion()
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
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                return false;
            }
            return true;


        }
    }
}
