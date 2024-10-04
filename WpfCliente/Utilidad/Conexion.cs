using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.Utilidad
{
    public static class Conexion
    {
        public static ServicioUsuarioSesionClient UsuarioSesionCliente { get; internal set; }
        public static ServicioSalaJugadorClient SalaJugadorCliente { get; internal set; }
        public static ServicioChatMotorClient ChatMotorCliente { get; internal set; }
        public static bool AbrirConexionUsuarioSesionClienteCallback(IServicioUsuarioSesionCallback callback)
        {
            bool resultado = false;
            if (UsuarioSesionCliente != null)
            {
                resultado = true;
            }
            else
            {
                try
                {
                    UsuarioSesionCliente = new ServicioUsuarioSesionClient(new System.ServiceModel.InstanceContext(callback));
                    resultado = true;
                }
                catch (Exception)
                {
                    //TODO: Manejar el error
                }
            }
            return resultado;
        }
        public static bool AbrirConexionSalaJugadorClienteCallback(IServicioSalaJugadorCallback callback)
        {
            bool resultado = false;
            if (SalaJugadorCliente != null)
            {
                resultado = true;
            }
            else
            {
                try
                {
                    SalaJugadorCliente = new ServicioSalaJugadorClient(new System.ServiceModel.InstanceContext(callback));
                    resultado = true;
                }
                catch (Exception)
                {
                    //TODO: Manejar el error
                }
            }
            return resultado;
        }
        public static bool AbrirConexionChatMotorCallback(IServicioChatMotorCallback callback)
        {
            bool resultado = false;
            if (ChatMotorCliente != null)
            {
                resultado = true;
            }
            else
            {
                try
                {
                    ChatMotorCliente = new ServicioChatMotorClient(new System.ServiceModel.InstanceContext(callback));
                    resultado = true;
                }
                catch (Exception)
                {
                    //TODO: Manejar el error
                }
            }
            return resultado;
        }
        public static bool CerrarConexionesServiciosSalaCallback()
        {
            try
            {
                if (ChatMotorCliente != null)
                {
                    ChatMotorCliente.Close();
                    ChatMotorCliente = null;
                }
                if (SalaJugadorCliente != null)
                {
                    SalaJugadorCliente.Close();
                    SalaJugadorCliente = null;
                }
            }
            catch (Exception excepcion)
            {
                //TODO Manejar el error
                return false;
            }
            return true;
        }
    }
}
