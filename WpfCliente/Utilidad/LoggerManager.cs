using Serilog;
using System;

namespace WpfCliente.Utilidad
{
    public static class LoggerManager
    {
        private static ILogger _logger;

        private static void ConfigurarLogger(string logFilePath)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(@logFilePath)
                .CreateLogger();
        }

        private static string BuildLogFilePath()
        {
            string dateFormat = "dd-MM-yyyy";
            string idFileName = "Log";
            string characterSeparation = "_";
            string fileExtension = ".txt";
            string relativeLogFilePath = "../../Logs\\";

            DateTime currentDate = DateTime.Today;
            string date = currentDate.ToString(dateFormat);

            string logFileName = idFileName + characterSeparation + date + fileExtension;
            string absoluteLogFilePath = Otros.ConstruirAbsolutePath(relativeLogFilePath);
            string logPath = absoluteLogFilePath + logFileName;

            return logPath;
        }

        public static ILogger GetLogger()
        {
            if (_logger == null)
            {
                string logPath = BuildLogFilePath();
                ConfigurarLogger(logPath);
            }

            _logger = Log.Logger;
            return _logger;
        }

    }
}