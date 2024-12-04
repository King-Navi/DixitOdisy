using System;
using Serilog;
using WpfCliente.GUI;
using System.Windows;
using WpfCliente.Contexto;

namespace WpfCliente.Utilidad
{
    public static class ManejadorExcepciones
    {
        private static readonly ILogger bitacora = ManejadorBitacora.ObtenerBitacora();
        private const string MENSAJE_ERROR_EXCEPTION = "Error exception:";
        private const string MENSAJE_FATAL_EXCEPTION = "Fatal exception:";
        private const string MENSAJE_COMPONENT_ERROR_EXCEPTION = "Component error exception:";
        private const string MENSAJE_COMPONENT_FATAL_EXCEPTION = "Component fatal exception:";
        private const string MENSAJE_EXCEPTION_TYPE = "Exception type:";

        public static void ManejarExcepcionError(Exception excepcion, Window ventana)
        {
            bitacora.Error("{MENSAJE_ERROR_EXCEPTION} {MENSAJE_EXCEPTION_TYPE} {ExceptionType} \n {Message} \n {StackTrace}",
                MENSAJE_ERROR_EXCEPTION, MENSAJE_EXCEPTION_TYPE, excepcion.GetType(), excepcion.Message, excepcion.StackTrace);

            if (ventana != null)
            {
                ventana.Close();
                SingletonGestorVentana.Instancia.AbrirNuevaVentanaPrincipal(new PrincipalWindow());
            }
        }

        public static void ManejarExcepcionFatal(Exception excepcion, Window ventana)
        {
            bitacora.Fatal("{MENSAJE_FATAL_EXCEPTION} \n {Message} \n {StackTrace}",
                MENSAJE_FATAL_EXCEPTION, excepcion.Message, excepcion.StackTrace);

            if (ventana != null)
            {
                ventana.Close();
                SingletonGestorVentana.Instancia.AbrirNuevaVentanaPrincipal(new PrincipalWindow());
            }
        }

        public static void ManejarExcepcionErrorComponente(Exception excepcion)
        {
            bitacora.Error("{MENSAJE_COMPONENT_ERROR_EXCEPTION} {MENSAJE_EXCEPTION_TYPE} {ExceptionType} \n {Message} \n {StackTrace}",
                MENSAJE_COMPONENT_ERROR_EXCEPTION, MENSAJE_EXCEPTION_TYPE, excepcion.GetType(), excepcion.Message, excepcion.StackTrace);
        }

        public static void ManejarExcepcionFatalComponente(Exception excepcion)
        {
            bitacora.Fatal("{MENSAJE_COMPONENT_FATAL_EXCEPTION} {MENSAJE_EXCEPTION_TYPE} {ExceptionType} \n {Message} \n {StackTrace}",
                MENSAJE_COMPONENT_FATAL_EXCEPTION, MENSAJE_EXCEPTION_TYPE, excepcion.GetType(), excepcion.Message, excepcion.StackTrace);
        }
    }
}
