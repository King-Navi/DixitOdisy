using System;
using Serilog;
using System.Windows.Navigation;
using WpfCliente.GUI;

namespace WpfCliente.Utilidad
{
    public static class ManejadorExcepciones
    {
        private static readonly ILogger _logger = LoggerManager.GetLogger();

        public static void HandleErrorException(Exception ex, NavigationService navigationService)
        {
            _logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");

            if (navigationService != null)
            {
                navigationService.Navigate(new IniciarSesion());
            }
        }

        public static void HandleFatalException(Exception ex, NavigationService navigationService)
        {
            _logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");

            if (navigationService != null)
            {
                navigationService.Navigate(new IniciarSesion());
            }
        }

        public static void HandleComponentErrorException(Exception ex)
        {
            _logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");
        }

        public static void HandleComponentFatalException(Exception ex)
        {
            _logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");
        }
    }
}
