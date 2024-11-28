using System;
using Serilog;
using WpfCliente.GUI;
using System.Windows;
using WpfCliente.Contexto;

namespace WpfCliente.Utilidad
{
    public static class ManejadorExcepciones
    {
        private static readonly ILogger logger = LoggerManagador .ObtenerLogger();

        public static void ManejarErrorExcepcion(Exception excepcion, Window ventana)
        {
            logger.Error(excepcion.Message + "\n" + excepcion.StackTrace + "\n");

            if (ventana != null)
            {
                ventana.Close();
                SingletonGestorVentana.Instancia.AbrirNuevaVentanaPrincipal(new PrincipalWindow());
            }
        }

        public static void ManejarFatalExcepcion(Exception excepcion, Window ventana)
        {
            logger.Fatal(excepcion.Message + "\n" + excepcion.StackTrace + "\n");

            if (ventana != null)
            {
                ventana.Close();
                SingletonGestorVentana.Instancia.AbrirNuevaVentanaPrincipal(new PrincipalWindow());
            }
        }

        public static void ManejarComponenteErrorExcepcion(Exception excepcion)
        {
            logger.Error(excepcion.Message + "\n" + excepcion.StackTrace + "\n");
        }

        public static void ManejarComponenteFatalExcepcion(Exception excepcion)
        {
            logger.Fatal(excepcion.Message + "\n" + excepcion.StackTrace + "\n");
        }
    }
}
