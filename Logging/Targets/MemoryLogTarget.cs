// ============================================================================
// ARCHIVO 8: Logging/MemoryLogTarget.cs
// ============================================================================
using System.Collections.Generic;

namespace DynamicUI.Logging.Targets

{
    public class MemoryLogTarget : ILogTarget
    {
        public List<LogEntry> Entries { get; } = new();

        public void Write(LogEntry entry)
        {
            Entries.Add(entry);
        }
    }
}
