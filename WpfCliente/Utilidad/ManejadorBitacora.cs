using Serilog;
using System;

namespace WpfCliente.Utilidad
{
    public static class ManejadorBitacora
    {
        private const string FORMATO_FECHA = "dd-MM-yyyy";
        private const string NOMBRE_LOG = "Log";
        private const string SEPARADOR_NOMBRE_FECHA = "_";
        private const string EXTENSION_LOG = ".txt";
        private static ILogger bitacora;

        private static void ConfigurarBitacora(string archivoRuta)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(archivoRuta)
                .CreateLogger();
        }

        private static string ContruirRutaArchivo()
        {
            DateTime fechaActual = DateTime.Today;
            string fecha = fechaActual.ToString(FORMATO_FECHA);
            string archivoBitacoraNombre = NOMBRE_LOG + SEPARADOR_NOMBRE_FECHA + fecha + EXTENSION_LOG;

            string rutaBase = AppDomain.CurrentDomain.BaseDirectory;
            string rutaLogs = System.IO.Path.Combine(rutaBase, "Logs");
            if (!System.IO.Directory.Exists(rutaLogs))
            {
                System.IO.Directory.CreateDirectory(rutaLogs);
            }
            string rutaBitacora = System.IO.Path.Combine(rutaLogs, archivoBitacoraNombre);

            return rutaBitacora;
        }

        public static ILogger ObtenerBitacora()
        {
            if (bitacora == null)
            {
                string rutaBitacora = ContruirRutaArchivo();
                ConfigurarBitacora(rutaBitacora);
            }

            bitacora = Log.Logger;
            return bitacora;
        }

    }
}