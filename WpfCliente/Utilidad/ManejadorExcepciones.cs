using System;
using Serilog;
using WpfCliente.GUI;
using System.Windows;
using Serilog.Core;
using WpfCliente.Contexto;

namespace WpfCliente.Utilidad
{
    public static class ManejadorExcepciones
    {
        private static readonly ILogger logger = ManejadorLogger.ObtenerLogger();
        private const string MENSAJE_ERROR_EXCEPTION = "Error exception:\n";
        private const string MENSAJE_FATAL_EXCEPTION = "Fatal exception:\n";
        private const string MENSAJE_COMPONENT_ERROR_EXCEPTION = "Component error exception:\n";
        private const string MENSAJE_COMPONENT_FATAL_EXCEPTION = "Component fatal exception:\n";
        private const string MENSAJE_TIPO_EXCEPCION = "Tipo de excepción: ";

        public static void ManejarErrorExcepcion(Exception excepcion, Window ventana)
        {
            logger.Error(MENSAJE_ERROR_EXCEPTION + MENSAJE_TIPO_EXCEPCION +  excepcion.GetType() + "\n" + excepcion.Message + "\n" + excepcion.StackTrace + "\n");

            if (ventana != null)
            {
                ventana.Close();
                SingletonGestorVentana.Instancia.AbrirNuevaVentanaPrincipal(new PrincipalWindow());
            }
        }

        public static void ManejarFatalExcepcion(Exception excepcion, Window ventana)
        {
            logger.Fatal(MENSAJE_FATAL_EXCEPTION + excepcion.Message + "\n" + excepcion.StackTrace + "\n");

            if (ventana != null)
            {
                ventana.Close();
                SingletonGestorVentana.Instancia.AbrirNuevaVentanaPrincipal(new PrincipalWindow());
            }
        }

        public static void ManejarComponenteErrorExcepcion(Exception excepcion)
        {
            logger.Error(MENSAJE_COMPONENT_ERROR_EXCEPTION + MENSAJE_TIPO_EXCEPCION + excepcion.GetType() + "\n" + excepcion.Message + "\n" + excepcion.StackTrace + "\n");
        }

        public static void ManejarComponenteFatalExcepcion(Exception excepcion)
        {
            logger.Fatal(MENSAJE_COMPONENT_FATAL_EXCEPTION + MENSAJE_TIPO_EXCEPCION + excepcion.GetType() + "\n" + excepcion.Message + "\n" + excepcion.StackTrace + "\n");
        }
    }
}
