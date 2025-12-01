// ============================================================================
// ARCHIVO 2: Logging/LogEntry.cs
// ============================================================================
using System;

namespace DynamicUI.Logging
{
    public class LogEntry
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public string Source { get; set; }
        public DateTime Timestamp { get; }

        public LogEntry()
        {
            Timestamp = DateTime.UtcNow;
        }

        public override string ToString()
        {
            var exceptionText = Exception != null ? $" EX: {Exception.Message}" : "";
            return $"[{Timestamp:O}] [{Level}] {Source}: {Message}{exceptionText}";
        }
    }
}


