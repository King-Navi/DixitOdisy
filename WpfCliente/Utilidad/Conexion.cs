using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.Utilidad
{
    public static class Conexion
<<<<<<< HEAD
    {   
=======
    {
        public static ServicioChatMotorClient ChatMotor { get; private set; }
        public static ServicioPartidaSesionClient Partida { get; private set; }
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
                    ManejadorExcepciones.ManejarExcepcionFatal(excepcion, null);
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
                    ManejadorExcepciones.ManejarExcepcionFatal(excepcion, null);
                }
            }
            return resultado;
        }
    
        public static void CerrarChatMotor()
        {
            try
            {
                if (ChatMotor != null)
                {
                    ChatMotor?.Close();
                }

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            finally
            {
                ChatMotor = null;
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
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion, null);
                return false;
            }
            finally
            {
                Partida = null;
            }
            return true;
        }

    
>>>>>>> 04/12/2024
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
                ManejadorExcepciones.ManejarExcepcionFatalComponente(enndpointException);
                return resultado;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                return resultado;
            }
            return resultado;
        }
        public static bool HacerPing()
        {
            bool resultado = false;
            try
            {
                ServidorDescribelo.IServicioUsuario ping = new ServicioUsuarioClient();
                resultado = ping.Ping() && ping.PingBD();

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
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
<<<<<<< HEAD
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
=======
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                return resultado;
>>>>>>> 04/12/2024
            }
            return resultado;
        }
    }
}
