//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Pruebas.ServidorDescribeloPrueba;

//namespace Pruebas.Cliente
//{
//    //NOTA: Esta clase tiene que ser la misma que la del cliente
//    /// <summary>
//    /// 
//    /// </summary>
//    public static class Conexion
//    {
//        public static ServicioUsuarioSesionClient UsuarioSesion { get; private set; }
//        public static ServicioSalaJugadorClient SalaJugador { get; private set; }
//        public static ServicioChatMotorClient ChatMotor{ get; private set; }
//        public static ServicioAmistadClient Amigos { get; private set; }
//        public static bool AbrirConexionUsuarioSesionCallback(IServicioUsuarioSesionCallback callback)
//        {
//            bool resultado = false;
//            if (UsuarioSesion != null)
//            {
//                resultado = true;
//            }
//            else
//            {
//                try
//                {
//                    UsuarioSesion = new ServicioUsuarioSesionClient(new System.ServiceModel.InstanceContext(callback));
//                    resultado = true;
//                }
//                catch (Exception)
//                {
//                    //TODO: Manejar el error
//                }
//            }
//            return resultado;
//        }
//        public static bool AbrirConexionSalaJugadorCallback(IServicioSalaJugadorCallback callback)
//        {
//            bool resultado = false;
//            if (SalaJugador != null)
//            {
//                resultado = true;
//            }
//            else
//            {
//                try
//                {
//                    SalaJugador = new ServicioSalaJugadorClient(new System.ServiceModel.InstanceContext(callback));
//                    resultado = true;
//                }
//                catch (Exception)
//                {
//                    //TODO: Manejar el error
//                }
//            }
//            return resultado;
//        }
//        public static bool AbrirConexionChatMotorCallback(IServicioChatMotorCallback callback)
//        {
//            bool resultado = false;
//            if (ChatMotor != null)
//            {
//                resultado = true;
//            }
//            else
//            {
//                try
//                {
//                    ChatMotor = new ServicioChatMotorClient(new System.ServiceModel.InstanceContext(callback));
//                    resultado = true;
//                }
//                catch (Exception)
//                {
//                    //TODO: Manejar el error
//                }
//            }
//            return resultado;
//        }
//        public static bool AbrirConexionAmigosCallback(IServicioAmistadCallback callback)
//        {
//            bool resultado = false;
//            if (Amigos != null)
//            {
//                resultado = true;
//            }
//            else
//            {
//                try
//                {
//                    Amigos = new ServicioAmistadClient(new System.ServiceModel.InstanceContext(callback));
//                    resultado = true;
//                }
//                catch (Exception)
//                {
//                    //TODO: Manejar el error
//                }
//            }
//            return resultado;
//        }
//        public static bool CerrarUsuarioSesion()
//        {
//            try
//            {
//                if (UsuarioSesion != null)
//                {
//                    UsuarioSesion.Close();
//                    UsuarioSesion = null;
//                }

//            }
//            catch (Exception excepcion)
//            {
//                //TODO Manejar el error
//                return false;
//            }
//            return true;
//        } public static bool CerrarSalaJugador()
//        {
//            try
//            {
//                if (SalaJugador != null)
//                {
//                    SalaJugador.Close();
//                    SalaJugador = null;
//                }

//            }
//            catch (Exception excepcion)
//            {
//                //TODO Manejar el error
//                return false;
//            }
//            return true;
//        } public static bool CerrarChatMotor()
//        {
//            try
//            {
//                if (ChatMotor != null)
//                {
//                    ChatMotor.Close();
//                    ChatMotor = null;
//                }

//            }
//            catch (Exception excepcion)
//            {
//                //TODO Manejar el error
//                return false;
//            }
//            return true;
//        } public static bool CerrarAmigos()
//        {
//            try
//            {
//                if (Amigos != null)
//                {
//                    Amigos.Close();
//                    Amigos = null;
//                }

//            }
//            catch (Exception excepcion)
//            {
//                //TODO Manejar el error
//                return false;
//            }
//            return true;
//        }
//        public static bool CerrarConexionesSalaConChat()
//        {
//            try
//            {
//                if (ChatMotor != null)
//                {
//                    ChatMotor.Close();
//                    ChatMotor = null;
//                }
//                if (SalaJugador != null)
//                {
//                    SalaJugador.Close();
//                    SalaJugador = null;
//                }
//            }
//            catch (Exception excepcion)
//            {
//                //TODO Manejar el error
//                return false;
//            }
//            return true;
//        }

//        public static bool ValidarConexion()
//        {
//            bool resultado = false;
//            try
//            {
//                ServidorDescribeloPrueba.IServicioUsuario ping = new ServicioUsuarioClient();
//                resultado = ping.Ping();
//            }
//            catch (Exception excepcion)
//            {
//                //TODO: Manejar excepcion
//                return resultado;
//            }
//            return resultado;
//        }
//    }
//}
