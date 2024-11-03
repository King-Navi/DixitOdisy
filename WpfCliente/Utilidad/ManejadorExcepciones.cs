using System;
using Serilog;
using System.Windows.Navigation;
using WpfCliente.GUI;
using System.Windows;

namespace WpfCliente.Utilidad
{
    public static class ManejadorExcepciones
    {
        private static readonly ILogger _logger = LoggerManager.GetLogger();

        public static void ManejarErrorException(Exception ex, Window window)
        {
            _logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");

            if (window != null)
            {
                window.Close();
                IniciarSesion iniciarSesion = new IniciarSesion();
                iniciarSesion.Show();
            }
        }

        public static void ManejarFatalException(Exception ex, Window window)
        {
            _logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");

            if (window != null)
            {
                window.Close();
                IniciarSesion iniciarSesion = new IniciarSesion();
                iniciarSesion.Show();
            }
        }

        public static void ManejarComponentErrorException(Exception ex)
        {
            _logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");
        }

        public static void ManejarComponentFatalException(Exception ex)
        {
            _logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");
        }
    }
}
