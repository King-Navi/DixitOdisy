using Serilog;
using System;

namespace WpfCliente.Utilidad
{
    public static class LoggerManager
    {
        private const string FORMATO_FECHA = "dd-MM-yyyy";
        private const string NOMBRE_LOG = "Log";
        private const string SEPARADOR_NOMBRE_FECHA = "_";
        private const string EXTENSION_LOG = ".txt";
        private const string RUTA_RELATIVA_LOGS = "../../Logs\\";
        private static ILogger logger;

        private static void ConfigurarLogger(string logFilePath)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(@logFilePath)
                .CreateLogger();
        }

        private static string ContruirRutaArchivo()
        {
            DateTime fechaActual = DateTime.Today;
            string fecha = fechaActual.ToString(FORMATO_FECHA);

            string archivoLoggerNombre = NOMBRE_LOG + SEPARADOR_NOMBRE_FECHA + fecha + EXTENSION_LOG;
            string rutaAbsolutaArchivo = Utilidades.ConstruirRutaAbsoluta(RUTA_RELATIVA_LOGS);
            string rutaLogger = rutaAbsolutaArchivo + archivoLoggerNombre;

            return rutaLogger;
        }

        public static ILogger ObtenerLogger()
        {
            if (logger == null)
            {
                string rutaLogger = ContruirRutaArchivo();
                ConfigurarLogger(rutaLogger);
            }

            logger = Log.Logger;
            return logger;
        }

    }
}