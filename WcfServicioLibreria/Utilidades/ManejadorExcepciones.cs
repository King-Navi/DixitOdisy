using Serilog;
using System;

namespace WcfServicioLibreria.Utilidades
{
    public static class ManejadorExcepciones
    {
        private static readonly ILogger logger = ManejadorBitacora.ObtenerBitacora();
        private const string MENSAJE_ERROR_EXCEPTION = "Error exception:\n";
        private const string MENSAJE_FATAL_EXCEPTION = "Fatal exception:\n";
        private const string MENSAJE_TIPO_EXCEPCION = "Tipo de excepción: ";

        public static void ManejarExcepcionError(Exception excepcion)
        {
            logger.Error(MENSAJE_ERROR_EXCEPTION + MENSAJE_TIPO_EXCEPCION + excepcion.GetType() + "\n" + excepcion.Message + "\n" + excepcion.StackTrace + "\n");
        }

        public static void ManejarExcepcionFatal(Exception excepcion)
        {
            logger.Fatal(MENSAJE_FATAL_EXCEPTION + MENSAJE_TIPO_EXCEPCION + excepcion.GetType() + "\n" + excepcion.Message + "\n" + excepcion.StackTrace + "\n");
        }
    }
}
