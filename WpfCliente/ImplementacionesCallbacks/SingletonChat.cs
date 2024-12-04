using System;
using System.ServiceModel;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.ImplementacionesCallbacks
{
    public class SingletonChat :IServicioChatMotorCallback
    {
        private static readonly Lazy<SingletonChat> instancia = new Lazy<SingletonChat>(() => new SingletonChat());
        public static SingletonChat Instancia => instancia.Value;

        public event Action<ChatMensaje> RecibirMensaje;
        private SingletonChat() { }
        public ServicioChatMotorClient ChatMotor { get; private set; }


        public bool AbrirConexionChat()
        {
            try
            {
                var objectoComunicacion = ChatMotor as ICommunicationObject;
                if (objectoComunicacion?.State == CommunicationState.Opened ||
                    objectoComunicacion?.State == CommunicationState.Opening)
                {
                    return true;
                }
                if (objectoComunicacion?.State == CommunicationState.Faulted)
                {
                    CerrarConexionChat();
                }
                ChatMotor = new ServicioChatMotorClient(new System.ServiceModel.InstanceContext(this));
                return true;
            }
            catch (Exception excepcion)
            {
                CerrarConexionChat();
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                return false;
            }

        }


        public bool CerrarConexionChat()
        {
            try
            {
                if (ChatMotor != null)
                {
                    ChatMotor.Close();
                    ChatMotor = null;
                }
            }
            catch (Exception excepcion)
            {
                ChatMotor = null;
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                return false;
            }
            return true;
        }

        public void RecibirMensajeClienteCallback(ChatMensaje mensaje)
        {
            if (mensaje != null)
                RecibirMensaje?.Invoke(mensaje);
        }
    }
}
