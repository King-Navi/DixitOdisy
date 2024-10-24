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
        public static ServicioUsuarioSesionClient UsuarioSesion { get; private set; }
        public static ServicioSalaJugadorClient SalaJugador { get; private set; }
        public static ServicioChatMotorClient ChatMotor{ get; private set; }
        public static ServicioAmistadClient Amigos { get; private set; }
        public static Task<bool> AbrirConexionUsuarioSesionCallbackAsync(IServicioUsuarioSesionCallback callback)
        {
            Task<bool> resultado = Task.FromResult(false); 
            if (UsuarioSesion != null)
            {
                resultado = Task.FromResult(true);
            }
            else
            {
                try
                {
                    UsuarioSesion = new ServicioUsuarioSesionClient(new System.ServiceModel.InstanceContext(callback));
                    resultado = Task.FromResult(true);
                }
                catch (Exception)
                {
                    //TODO: Manejar el error
                }
            }
            return resultado;
        }
        public static Task<bool> AbrirConexionSalaJugadorCallbackAsync(IServicioSalaJugadorCallback callback)
        {
            Task<bool> resultado = Task.FromResult(false);
            if (SalaJugador != null)
            {
                resultado = Task.FromResult(true);
            }
            else
            {
                try
                {
                    SalaJugador = new ServicioSalaJugadorClient(new System.ServiceModel.InstanceContext(callback));
                    resultado = Task.FromResult(true);
                }
                catch (Exception)
                {
                    //TODO: Manejar el error
                }
            }
            return resultado;
        }
        public static Task<bool> AbrirConexionChatMotorCallbackAsync(IServicioChatMotorCallback callback)
        {
            Task<bool> resultado = Task.FromResult(false);
            if (ChatMotor != null)
            {
                resultado = Task.FromResult(true);
            }
            else
            {
                try
                {
                    ChatMotor = new ServicioChatMotorClient(new System.ServiceModel.InstanceContext(callback));
                    resultado = Task.FromResult(true);
                }
                catch (Exception)
                {
                    //TODO: Manejar el error
                }
            }
            return resultado;
        }
        public static Task<bool> AbrirConexionAmigosCallbackAsync(IServicioAmistadCallback callback)
        {
            Task<bool> resultado = Task.FromResult(false);
            if (Amigos != null)
            {
                resultado = Task.FromResult(true);
            }
            else
            {
                try
                {
                    Amigos = new ServicioAmistadClient(new System.ServiceModel.InstanceContext(callback));
                    resultado = Task.FromResult(true);
                }
                catch (Exception)
                {
                    //TODO: Manejar el error
                }
            }
            return resultado;
        }
        public static bool CerrarUsuarioSesion()
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
                //TODO Manejar el error
                return false;
            }
            return true;
        } public static bool CerrarSalaJugador()
        {
            try
            {
                if (SalaJugador != null)
                {
                    SalaJugador.Close();
                    SalaJugador = null;
                }

            }
            catch (Exception excepcion)
            {
                //TODO Manejar el error
                return false;
            }
            return true;
        } public static bool CerrarChatMotor()
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
                //TODO Manejar el error
                return false;
            }
            return true;
        } public static bool CerrarAmigos()
        {
            try
            {
                if (Amigos != null)
                {
                    Amigos.Close();
                    Amigos = null;
                }

            }
            catch (Exception excepcion)
            {
                //TODO Manejar el error
                return false;
            }
            return true;
        }
        public static bool CerrarConexionesSalaConChat()
        {
            try
            {
                if (ChatMotor != null)
                {
                    ChatMotor.Close();
                    ChatMotor = null;
                }
                if (SalaJugador != null)
                {
                    SalaJugador.Close();
                    SalaJugador = null;
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
