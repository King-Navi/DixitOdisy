using Serilog;
using System;

namespace WcfServicioLibreria.Utilidades
{
    public static class ManejadorExcepciones
    {
        private static readonly ILogger _logger = LoggerManager.ObtenerLogger();

        public static void ManejarErrorException(Exception ex)
        {
            _logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");
        }

        public static void ManejarFatalException(Exception ex)
        {
            _logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");
        }
    }
}
