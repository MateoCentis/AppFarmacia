using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AppFarmacia.Services
{
    public class LoggerService
    {
        private readonly string _logFilePath;
        private readonly ILogger<LoggerService>? _logger;
        private readonly object _lockObject = new object();

        public LoggerService(ILogger<LoggerService>? logger = null)
        {
            _logger = logger;
            
            // Crear ruta del archivo de log en el directorio de datos de la app
            var appDataPath = FileSystem.AppDataDirectory;
            var logsDirectory = Path.Combine(appDataPath, "Logs");
            
            // Crear directorio si no existe
            if (!Directory.Exists(logsDirectory))
            {
                Directory.CreateDirectory(logsDirectory);
            }
            
            // Archivo de log con fecha
            var fileName = $"app_{DateTime.Now:yyyyMMdd}.log";
            _logFilePath = Path.Combine(logsDirectory, fileName);
        }

        public void LogInfo(string message)
        {
            Log("INFO", message);
        }

        public void LogWarning(string message)
        {
            Log("WARNING", message);
        }

        public void LogError(string message, Exception? ex = null)
        {
            var errorMessage = message;
            if (ex != null)
            {
                errorMessage += $"\nException: {ex.Message}\nStack Trace: {ex.StackTrace}";
            }
            Log("ERROR", errorMessage);
        }

        public void LogDebug(string message)
        {
            Log("DEBUG", message);
        }

        private void Log(string level, string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var logEntry = $"[{timestamp}] [{level}] {message}";
            
            // Escribir a consola
            System.Diagnostics.Debug.WriteLine(logEntry);
            
            // Escribir a archivo
            try
            {
                lock (_lockObject)
                {
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error escribiendo al log: {ex.Message}");
            }
            
            // Usar ILogger si está disponible
            _logger?.Log(
                level == "ERROR" ? LogLevel.Error :
                level == "WARNING" ? LogLevel.Warning :
                level == "DEBUG" ? LogLevel.Debug :
                LogLevel.Information,
                message
            );
        }

        public string GetLogFilePath()
        {
            return _logFilePath;
        }

        public async Task<string> ReadLogsAsync(int maxLines = 100)
        {
            try
            {
                if (!File.Exists(_logFilePath))
                {
                    return "No hay logs disponibles.";
                }

                var lines = await File.ReadAllLinesAsync(_logFilePath);
                var recentLines = lines.Length > maxLines 
                    ? lines.Skip(lines.Length - maxLines).ToArray()
                    : lines;
                
                return string.Join(Environment.NewLine, recentLines);
            }
            catch (Exception ex)
            {
                return $"Error leyendo logs: {ex.Message}";
            }
        }
    }
}

