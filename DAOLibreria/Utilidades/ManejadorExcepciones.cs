using Serilog;
using System;

namespace DAOLibreria.Utilidades
{
    internal class ManejadorExcepciones
    {
        private static readonly ILogger logger = LoggerManejador.ObtenerLogger();
        private const string MENSAJE_ERROR_EXCEPTION = "Error exception:\n";
        private const string MENSAJE_FATAL_EXCEPTION = "Fatal exception:\n";
        private const string MENSAJE_TIPO_EXCEPCION = "Tipo de excepción: ";

        public static void ManejarErrorException(Exception excepcion)
        {
            logger.Error(MENSAJE_ERROR_EXCEPTION + MENSAJE_TIPO_EXCEPCION + excepcion.GetType() + "\n" + excepcion.Message + "\n" + excepcion.StackTrace + "\n");
        }

        public static void ManejarFatalException(Exception excepcion)
        {
            logger.Fatal(MENSAJE_FATAL_EXCEPTION + MENSAJE_TIPO_EXCEPCION + excepcion.GetType() + "\n" + excepcion.Message + "\n" + excepcion.StackTrace + "\n");
        }
    }
}
