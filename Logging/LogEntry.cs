// ============================================================================
// ARCHIVO 2: Logging/LogEntry.cs
// ============================================================================
using System;
using System.Collections.Generic;

namespace DynamicUI.Logging
{
    /// <summary>
    /// Representa una entrada individual de log
    /// </summary>
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public string Source { get; set; }
        public Dictionary<string, object> Context { get; set; }

        public LogEntry()
        {
            Timestamp = DateTime.UtcNow;
            Context = new Dictionary<string, object>();
        }
    }
}

