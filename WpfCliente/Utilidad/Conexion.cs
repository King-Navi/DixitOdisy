using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.Utilidad
{
    public static class Conexion
    {
        public static ServicioSalaJugadorClient SalaJugador { get; private set; }
        public static ServicioChatMotorClient ChatMotor { get; private set; }
        public static ServicioAmistadClient Amigos { get; private set; }
        public static ServicioPartidaSesionClient Partida { get; private set; }
        public static ServicioInvitacionPartidaClient InvitacionPartida { get; private set; }
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

        public static void CerrarSalaJugador()
        {
            try
            {
                if (SalaJugador != null)
                {
                    SalaJugador.Close();
                }

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, null);
            }
            finally
            {
                SalaJugador = null;
            }
        } 
    
        public static void CerrarChatMotor()
        {
            try
            {
                if (ChatMotor != null)
                {
                    ChatMotor.Close();
                }

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            finally
            {
                ChatMotor = null;
            }
        } 
        
        public static void CerrarAmigos()
        {
            try
            {
                if (Amigos != null)
                {
                    Amigos.Close();
                }

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            finally
            {
                Amigos = null;

            }
        }
      
        public static void CerrarConexionesSalaConChat()
        {
            try
            {
                if (ChatMotor != null)
                {
                    ChatMotor.Close();
                }
                if (SalaJugador != null)
                {
                    SalaJugador.Close();
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(excepcion, null);
            }
            finally
            {
                ChatMotor = null;
                SalaJugador = null;
            }
        }
      
        public static bool CerrarPartida()
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
    
        private static async Task<bool> HacerPingAsync()
        {
            bool resultado = false;
            try
            {
                ServidorDescribelo.IServicioUsuario ping = new ServicioUsuarioClient();
                resultado = await ping.PingAsync();
            }
            catch (EndpointNotFoundException enndpointException)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(enndpointException);
                return resultado;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
                return resultado;
            }
            return resultado;
        }

        public static async Task<bool> VerificarConexionAsync(Action<bool> habilitarAcciones, Window ventana)
        {
            habilitarAcciones?.Invoke(false);
            DeshabilitarVentana(ventana, false);
            Task<bool> verificarConexion = HacerPingAsync();

            if (!await verificarConexion)
            {
                VentanasEmergentes.CrearVentanaEmergenteConCierre(Properties.Idioma.tituloErrorServidor, Properties.Idioma.mensajeErrorServidor, ventana);
                habilitarAcciones?.Invoke(true);
                DeshabilitarVentana(ventana, true);
                return false;
            }
            Task<bool> verificarConexionBD = HacerPingBaseDatosAsync();

            if (!await verificarConexionBD)
            {
                VentanasEmergentes.CrearVentanaEmergenteConCierre(Properties.Idioma.tituloErrorBaseDatos, Properties.Idioma.mensajeErrorBaseDatos, ventana);
                habilitarAcciones?.Invoke(true);
                DeshabilitarVentana(ventana, true);
                return false;
            }

            habilitarAcciones?.Invoke(true);
            DeshabilitarVentana(ventana, true);
            return true;
        }

        public static async Task<bool> VerificarConexionSinBaseDatosAsync()
        {
            Task<bool> verificarConexion = HacerPingAsync();
            if (!await verificarConexion)
            {
                return false;
            }
            return true;
        }


        private static void DeshabilitarVentana(Window ventana, bool estado)
        {
            if (ventana != null)
            {
                ventana.IsEnabled = estado;
            }
        }

        private static async Task<bool> HacerPingBaseDatosAsync()
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
