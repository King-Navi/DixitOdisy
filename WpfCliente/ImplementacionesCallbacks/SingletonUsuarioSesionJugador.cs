using System;
using System.ServiceModel;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.ImplementacionesCallbacks
{
    public partial class SingletonCanal: IServicioUsuarioSesionCallback
    {
        public ServicioUsuarioSesionClient UsuarioSesion { get; private set; }
        public EventHandler InicioSesionEvento;

        public void ObtenerSesionJugadorCallback()
        {
            InicioSesionEvento?.Invoke(this, EventArgs.Empty);
        }
        public bool AbrirConexionUsuarioSesion()
        {
            try
            {
                var objectoComunicacion = UsuarioSesion as ICommunicationObject;
                if (objectoComunicacion?.State == CommunicationState.Opened ||
                    objectoComunicacion?.State == CommunicationState.Opening)
                {
                    return true;
                }
                if (objectoComunicacion?.State == CommunicationState.Faulted)
                {
                    CerrarConexionUsuarioSesion();
                }
                UsuarioSesion = new ServicioUsuarioSesionClient(new System.ServiceModel.InstanceContext(this));
                LimpiarRecursos();
                return true;
            }
            catch (CommunicationException excepcion)
            {
                CerrarConexionUsuarioSesion();
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                CerrarConexionUsuarioSesion();
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            return false;
        }

        public bool CerrarConexionUsuarioSesion()
        {
            try
            {
                if (UsuarioSesion != null)
                {
                    UsuarioSesion.Close();
                    UsuarioSesion = null;
                }
            }
            catch (Exception excepcion)
            {
                UsuarioSesion = null;
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion, null);
                return false;
            }
            return true;
        }

    }
}
