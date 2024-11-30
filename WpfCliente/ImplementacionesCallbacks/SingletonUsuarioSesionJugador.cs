using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;
using WpfCliente.Interfaz;

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
            catch (Exception excepcion)
            {
                CerrarConexionUsuarioSesion();
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
                return false;
            }

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
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, null);
                return false;
            }
            return true;
        }

    }
}
