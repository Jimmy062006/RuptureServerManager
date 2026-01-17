using System;
using System.IO;

namespace RuptureServerManager
{
    public static class Logger
    {
        private static string _logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        private static string _logFileName = "server.txt";

        public static void Log(string message)
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }
                var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                File.AppendAllText(Path.Combine(_logDirectory, _logFileName), line + Environment.NewLine);
            }
            catch
            {
                // Intentionally swallow logging errors
            }
        }
    }
}
