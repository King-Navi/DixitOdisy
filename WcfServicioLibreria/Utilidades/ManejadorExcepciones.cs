using Serilog;
using System;

namespace WcfServicioLibreria.Utilidades
{
    public static class ManejadorExcepciones
    {
        private static readonly ILogger _logger = LoggerManajador.ObtenerLogger();

        public static void ManejarErrorException(Exception excepcion)
        {
            _logger.Error(excepcion.Message + "\n" + excepcion.StackTrace + "\n");
        }

        public static void ManejarFatalException(Exception excepcion)
        {
            _logger.Fatal(excepcion.Message + "\n" + excepcion.StackTrace + "\n");
        }
    }
}
