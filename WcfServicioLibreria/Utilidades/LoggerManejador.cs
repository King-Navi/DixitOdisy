using System;
using Serilog;

namespace WcfServicioLibreria.Utilidades
{
    public static class LoggerManejador
    {
        private const string FORMATO_FECHA = "dd-MM-yyyy";
        private const string NOMBRE_LOG = "Log";
        private const string SEPARADOR_NOMBRE_FECHA = "_";
        private const string EXTENSION_LOG = ".txt";
        private const string NOMBRE_CARPETA_INCIDENTES = "Logs";
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

            string rutaBase = AppDomain.CurrentDomain.BaseDirectory;
            string rutaLogs = System.IO.Path.Combine(rutaBase, NOMBRE_CARPETA_INCIDENTES);
            if (!System.IO.Directory.Exists(rutaLogs))
            {
                System.IO.Directory.CreateDirectory(rutaLogs);
            }
            string rutaLogger = System.IO.Path.Combine(rutaLogs, archivoLoggerNombre);

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