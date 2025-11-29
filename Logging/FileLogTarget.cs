// ============================================================================
// ARCHIVO 7: Logging/FileLogTarget.cs
// ============================================================================
using System;
using System.IO;
using System.Text;

namespace DynamicUI.Logging
{
    /// <summary>
    /// Target que escribe logs a un archivo
    /// </summary>
    public class FileLogTarget : ILogTarget
    {
        private readonly string _filePath;
        private readonly object _lock = new();
        private readonly bool _appendTimestamp;

        public FileLogTarget(string filePath, bool appendTimestamp = false)
        {
            _filePath = filePath;
            _appendTimestamp = appendTimestamp;

            // Crear directorio si no existe
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void Write(LogEntry entry)
        {
            lock (_lock)
            {
                try
                {
                    var filePath = _filePath;
                    
                    // Si se requiere timestamp en el nombre del archivo
                    if (_appendTimestamp)
                    {
                        var directory = Path.GetDirectoryName(_filePath);
                        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(_filePath);
                        var extension = Path.GetExtension(_filePath);
                        var timestamp = DateTime.Now.ToString("yyyyMMdd");
                        filePath = Path.Combine(directory ?? "", $"{fileNameWithoutExt}_{timestamp}{extension}");
                    }

                    var sb = new StringBuilder();
                    sb.AppendLine($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{entry.Level}] [{entry.Source}]");
                    sb.AppendLine($"  {entry.Message}");
                    
                    if (entry.Exception != null)
                    {
                        sb.AppendLine($"  Exception Type: {entry.Exception.GetType().Name}");
                        sb.AppendLine($"  Exception Message: {entry.Exception.Message}");
                        sb.AppendLine($"  StackTrace: {entry.Exception.StackTrace}");
                        
                        // Inner exceptions
                        var innerEx = entry.Exception.InnerException;
                        while (innerEx != null)
                        {
                            sb.AppendLine($"  Inner Exception: {innerEx.Message}");
                            innerEx = innerEx.InnerException;
                        }
                    }
                    
                    sb.AppendLine(); // Línea en blanco entre entradas

                    File.AppendAllText(filePath, sb.ToString());
                }
                catch (Exception ex)
                {
                    // Si falla la escritura al archivo, intentar escribir en consola
                    Console.WriteLine($"Error escribiendo log a archivo: {ex.Message}");
                }
            }
        }
    }
}