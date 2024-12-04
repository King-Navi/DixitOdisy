using Serilog;
using System;

namespace DAOLibreria.Utilidades
{
    internal class ManejadorExcepciones
    {
        private static readonly ILogger logger = ManejadorBitacora.ObtenerLogger();
        private const string MENSAJE_ERROR_EXCEPTION = "Error exception:";
        private const string MENSAJE_FATAL_EXCEPTION = "Fatal exception:";
        private const string MENSAJE_TIPO_EXCEPCION = "Tipo de excepción:";

        public static void ManejarErrorException(Exception excepcion)
        {
            logger.Error("{MENSAJE_ERROR_EXCEPTION} {MENSAJE_TIPO_EXCEPCION} {ExceptionType} \n {Message} \n {StackTrace}",
                MENSAJE_ERROR_EXCEPTION, MENSAJE_TIPO_EXCEPCION, excepcion.GetType(), excepcion.Message, excepcion.StackTrace);
        }

        public static void ManejarFatalException(Exception excepcion)
        {
            logger.Fatal("{MENSAJE_FATAL_EXCEPTION} {MENSAJE_TIPO_EXCEPCION} {ExceptionType} \n {Message} \n {StackTrace}",
                MENSAJE_FATAL_EXCEPTION, MENSAJE_TIPO_EXCEPCION, excepcion.GetType(), excepcion.Message, excepcion.StackTrace);
        }
    }
}
