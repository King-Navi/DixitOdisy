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
    public partial class SingletonCanles: IServicioUsuarioSesionCallback, IImplementacionCallback
    {
        public ServicioUsuarioSesionClient UsuarioSesion { get; private set; }
        public EventHandler InicioSesionEvento;

        public void ObtenerSessionJugadorCallback()
        {
            InicioSesionEvento?.Invoke(this, EventArgs.Empty);
        }
        public bool AbrirConexion()
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
                    CerrarConexion();
                }
                UsuarioSesion = new ServicioUsuarioSesionClient(new System.ServiceModel.InstanceContext(this));
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
