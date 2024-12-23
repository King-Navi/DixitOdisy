﻿using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.Utilidad
{
    public static class Conexion
    {
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

        public static async Task<bool> VerificarConexionSinBaseDatosAsync(Action<bool> habilitarAcciones, Window ventana)
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
            habilitarAcciones?.Invoke(true);
            DeshabilitarVentana(ventana, true);
            return true;
        }

        public static async Task<bool> VerificarConexionConBaseDatosSinCierreAsync(Action<bool> habilitarAcciones, Window ventana)
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
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloErrorBaseDatos, Properties.Idioma.mensajeErrorBaseDatos, ventana);
                habilitarAcciones?.Invoke(true);
                DeshabilitarVentana(ventana, true);
                return false;
            }

            habilitarAcciones?.Invoke(true);
            DeshabilitarVentana(ventana, true);
            return true;
        }

        public static async Task<bool> VerificarExistenciaPartida()
        {
            Task<bool> verificarConexion = HacerPingAsync();
            if (!await verificarConexion)
            {
                throw new CommunicationException();
            }
            var servicio = new ServicioManejador<ServicioPartidaClient>();
            var resultado = servicio.EjecutarServicio(llamada =>
            {
                return llamada.ValidarPartida(SingletonCliente.Instance.IdPartida);
            });
            if (!resultado)
            {
                throw new ArgumentException();
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
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            return resultado;
        }
    }
}