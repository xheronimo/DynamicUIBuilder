//DynamicUI.Logging.Targets / JsonFileLogTarget.cs
using System;
using System.IO;
using System.Text.Json;

namespace DynamicUI.Logging.Targets
{
    /// <summary>
    /// Escribe cada LogEntry como línea JSON (newline-delimited JSON).
    /// </summary>
    public class JsonFileLogTarget : ILogTarget, IDisposable
    {
        private readonly StreamWriter _writer;
        private readonly JsonSerializerOptions _opts = new() { WriteIndented = false };

        public JsonFileLogTarget(string filePath, bool append = true)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            _writer = new StreamWriter(new FileStream(filePath, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                AutoFlush = true
            };
        }

        public void Write(LogEntry entry)
        {
            try
            {
                var dto = new
                {
                    Timestamp = entry.Timestamp,
                    Level = entry.Level.ToString(),
                    entry.Source,
                    entry.Message,
                    Exception = entry.Exception?.ToString()
                };
                var json = JsonSerializer.Serialize(dto, _opts);
                _writer.WriteLine(json);
            }
            catch
            {
                // evitar propagar
            }
        }

        public void Dispose()
        {
            try { _writer?.Dispose(); } catch { }
        }
    }
}
