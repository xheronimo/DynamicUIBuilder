// ============================================================================
// ARCHIVO 7: Logging/FileLogTarget.cs
// ============================================================================
using System;
using System.IO;

namespace DynamicUI.Logging.Targets

{
    public class FileLogTarget : ILogTarget
    {
        private readonly string _filePath;

        public FileLogTarget(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        }

        public void Write(LogEntry entry)
        {
            File.AppendAllText(_filePath, entry.ToString() + Environment.NewLine);
        }
    }
}
