using Serilog;
using System;

namespace WcfServicioLibreria.Utilidades
{
    public static class ManejadorExcepciones
    {
        private static readonly ILogger bitacora = ManejadorBitacora.ObtenerBitacora();
        private const string MENSAJE_ERROR_EXCEPTION = "Error exception:\n";
        private const string MENSAJE_FATAL_EXCEPTION = "Fatal exception:\n";
        private const string MENSAJE_TIPO_EXCEPCION = "Tipo de excepción: ";

        public static void ManejarExcepcionError(Exception excepcion)
        {
            bitacora.Error("{MENSAJE_ERROR_EXCEPTION} {MENSAJE_TIPO_EXCEPCION} {ExceptionType} \n {Message} \n {StackTrace}",
                MENSAJE_ERROR_EXCEPTION, MENSAJE_TIPO_EXCEPCION, excepcion.GetType(), excepcion.Message, excepcion.StackTrace);
        }

        public static void ManejarExcepcionFatal(Exception excepcion)
        {
            bitacora.Fatal("{MENSAJE_FATAL_EXCEPTION} {MENSAJE_TIPO_EXCEPCION} {ExceptionType} \n {Message} \n {StackTrace}",
                MENSAJE_FATAL_EXCEPTION, MENSAJE_TIPO_EXCEPCION, excepcion.GetType(), excepcion.Message, excepcion.StackTrace);
        }
    }
}
