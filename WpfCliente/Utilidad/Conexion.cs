using System;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.Utilidad
{
    public static class Conexion
    {
        public static ServicioUsuarioSesionClient UsuarioSesion { get; private set; }
        public static ServicioSalaJugadorClient SalaJugador { get; private set; }
        public static ServicioChatMotorClient ChatMotor { get; private set; }
        public static ServicioAmistadClient Amigos { get; private set; }
        public static ServicioPartidaSesionClient Partida { get; private set; }
        public static ServicioInvitacionPartidaClient InvitacionPartida { get; private set; }
        public static Task<bool> AbrirConexionUsuarioSesionCallbackAsync(IServicioUsuarioSesionCallback callback)
        {
            UsuarioSesion = null;
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
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarFatalExcepcion(excepcion, null);
                }
            }
            return resultado;
        }

        public static Task<bool> AbrirConexionSalaJugadorCallbackAsync(IServicioSalaJugadorCallback callback)
        {
            SalaJugador = null;
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
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarFatalExcepcion(excepcion, null);
                }
            }
            return resultado;
        }

        public static Task<bool> AbrirConexionChatMotorCallbackAsync(IServicioChatMotorCallback callback)
        {
            ChatMotor = null;
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
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarFatalExcepcion(excepcion, null);
                }
            }
            return resultado;
        }

        public static Task<bool> AbrirConexionAmigosCallbackAsync(IServicioAmistadCallback callback)
        {
            Amigos = null;
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
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarFatalExcepcion(excepcion, null);
                }
            }
            return resultado;
        }

        public static Task<bool> AbrirConexionPartidaCallbackAsync(IServicioPartidaSesionCallback callback)
        {
            Partida = null;
            Task<bool> resultado = Task.FromResult(false);
            if (Partida != null)
            {
                resultado = Task.FromResult(true);
            }
            else
            {
                try
                {
                    Partida = new ServicioPartidaSesionClient(new System.ServiceModel.InstanceContext(callback));
                    resultado = Task.FromResult(true);
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarFatalExcepcion(excepcion, null);
                }
            }
            return resultado;
        }

        public static Task<bool> AbrirConexionInvitacionPartidaCallbackAsync(IServicioInvitacionPartidaCallback callback)
        {
            InvitacionPartida = null;
            Task<bool> resultado = Task.FromResult(false);
            if (InvitacionPartida != null)
            {
                resultado = Task.FromResult(true);
            }
            else
            {
                try
                {
                    InvitacionPartida = new ServicioInvitacionPartidaClient(new System.ServiceModel.InstanceContext(callback));
                    resultado = Task.FromResult(true);
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarFatalExcepcion(excepcion, null);
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
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, null);
                return false;
            }
            return true;
        } 
      
        public static bool CerrarSalaJugador()
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
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, null);
                return false;
            }
            return true;
        } 
    
        public static bool CerrarChatMotor()
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
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                return false;
            }
            return true;
        } 
        
        public static bool CerrarAmigos()
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
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
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
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, null);
                return false;
            }
            return true;
        }
      
        public static bool CerrarConexionesPartida()
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
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, null);
                return false;
            }
            finally
            {
                Partida = null;
            }
            return true;
        }

        public static bool CerrarConexionInvitacionesPartida()
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
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, null);
                return false;
            }
            finally
            {
                InvitacionPartida = null;
            }
            return true;
        }
    
        private static async Task<bool> HacerPing()
        {
            bool resultado = false;
            try
            {
                ServidorDescribelo.IServicioUsuario ping = new ServicioUsuarioClient();
                resultado = await ping.PingAsync();
            }
            catch (EndpointNotFoundException enndpointException)
            {
                Window window = null;
                ManejadorExcepciones.ManejarFatalExcepcion(enndpointException, window);
                return resultado;
            }
            catch (Exception excepcion)
            {
                Window window = null;
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion,window);
                return resultado;
            }
            return resultado;
        }

        public static async Task<bool> VerificarConexion(Action<bool> habilitarAcciones, Window ventana)
        {
            habilitarAcciones(false);
            DeshabilitarVentana(ventana, false);
            Task<bool> verificarConexion = HacerPing();

            if (!await verificarConexion)
            {
                VentanasEmergentes.CrearVentanaEmergenteConCierre(Properties.Idioma.tituloErrorServidor, Properties.Idioma.mensajeErrorServidor, ventana);
                habilitarAcciones(true);
                DeshabilitarVentana(ventana, true);
                return false;
            }
            Task<bool> verificarConexionBD = HacerPingBD();

            if (!await verificarConexionBD)
            {
                VentanasEmergentes.CrearVentanaEmergenteConCierre(Properties.Idioma.tituloErrorBaseDatos, Properties.Idioma.mensajeErrorBaseDatos, ventana);
                habilitarAcciones(true);
                DeshabilitarVentana(ventana, true);
                return false;
            }

            habilitarAcciones(true);
            DeshabilitarVentana(ventana, true);
            return true;
        }
      
        private static void DeshabilitarVentana(Window ventana, bool estado)
        {
            if (ventana != null)
            {
                ventana.IsEnabled = estado;
            }
        }

        private static async Task<bool> HacerPingBD()
        {
            bool resultado = false;
            try
            {
                ServidorDescribelo.IServicioUsuario ping = new ServicioUsuarioClient();
                resultado = await ping.PingBDAsync();
            }
            catch (Exception)
            {
            }
            return resultado;
        }
    }
}
